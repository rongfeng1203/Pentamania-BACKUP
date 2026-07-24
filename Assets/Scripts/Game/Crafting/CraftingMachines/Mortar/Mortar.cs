using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;

[Serializable]
public class Mortar
{
    private readonly List<MortarRecipe> recipes;
    private readonly List<ItemStack> solids = new();
    private readonly FluidStorage fluidStorage;
    private readonly FluidDef mixedDustDef;
    private readonly IngredientTag dustTag;
    private const float PROGRESS_DECAY = 0.7f;
    private float grindProgress;

    public event Action OnInventoryChanged;
    public event Action<float> OnProgressChanged;
    public event Action<bool> OnCraftComplete;

    public Mortar(IEnumerable<MortarRecipe> recipeSrc, FluidDef mixedDust, IngredientTag tag, int tankCap = 9999)
    {
        recipes = new List<MortarRecipe>(recipeSrc);
        mixedDustDef = mixedDust;
        fluidStorage = new FluidStorage(1, tankCap);
        grindProgress = 0f;
        dustTag = tag;
    }

    public float Progress => grindProgress;
    public IReadOnlyList<ItemStack> SolidsView => solids;
    public IReadOnlyList<FluidStack> FluidsView => fluidStorage.View();

    public void InsertSolid(ItemStack incoming)
    {
        if (incoming.IsEmpty)
            return;
        int idx = solids.FindIndex(s => !s.IsEmpty && s.Def == incoming.Def);
        if (idx >= 0)
        {
            var s = solids[idx];
            s.amount += incoming.amount; solids[idx] = s;
        }
        else
            solids.Add(incoming);
        Debug.Log($"[Mortar] InsertSolid {incoming.Def.GetId()} x{incoming.amount}");
        DecayProgress();
        OnInventoryChanged?.Invoke();
    }
    
    public void RemoveSolid(ItemStack outgoing)
    {
        if (outgoing.IsEmpty)
            return;

        int idx = solids.FindIndex(s => !s.IsEmpty && s.Def == outgoing.Def);
        if (idx < 0)
            return;

        var cur = solids[idx];
        cur.amount -= outgoing.amount;
        if (cur.amount <= 0)
            solids.RemoveAt(idx);
        else
            solids[idx] = cur;

        Debug.Log($"[Mortar] RemoveSolid {outgoing.Def.GetId()} x{outgoing.amount}");
        DecayProgress();
        OnInventoryChanged?.Invoke();
    }

    public float ScoopLiquid(FluidDef def, float volReq)
    {
        FluidStack drained = fluidStorage.Drain(f => f.Def == def, volReq);
        float real = Mathf.Min(volReq, drained.volume);
        if (real < drained.volume) 
            fluidStorage.Fill(drained.CopyWithVolume(drained.volume - real));
        Debug.Log($"[Mortar] ScoopLiquid {def.GetId()} requested={volReq} got={real}");
        OnInventoryChanged?.Invoke();
        return real;
    }
    
    public float InsertLiquid(FluidStack stack)
    {
        if (stack.IsEmpty)
            return 0;
        float remain = fluidStorage.Fill(stack);
        if (remain > 0)
            Debug.Log("[Mortar] Exceeds (Liquid)");
        OnInventoryChanged?.Invoke();
        return remain;
    }

    public FluidStack TakeFluid(FluidStack stack)
    {
        if (stack.IsEmpty)
            return new FluidStack(null, 0);
        
        FluidStack drained = fluidStorage.Drain(f => f.Def == stack.Def, stack.volume);
        float got = Mathf.Min(stack.volume, drained.volume);
        if (got < drained.volume)
        {
            InsertLiquid(new FluidStack(stack.Def, drained.volume - got));
        }
        OnInventoryChanged?.Invoke();

        stack.volume = got;
        return stack;
    }

    public bool TryGetOnlyFluid(out FluidStack found)
    {
        found = new FluidStack();
        foreach (var tank in fluidStorage.View())
        {
            if (tank.IsEmpty) continue;
            
            if (found.IsEmpty)
            {
                found = tank.Copy();
            }
            else if (!found.CanMerge(tank))
            {
                //Debug.Log("Can not merge");
                return false;
            }
            else
            {
                found.volume += tank.volume;
            }
        }

        if (found.IsEmpty)
        {
            Debug.Log("[Mortar] No Fluid inside");
            return false;
        }
        //Debug.Log($"[Mortar] {found.tags.Count}");
        return true;
    }

    public void PestleHit(float baseInc)
    {
        float totalMass = solids.Sum(s => s.amount);
        grindProgress = Mathf.Clamp01(grindProgress + baseInc / Mathf.Max(1f, totalMass));
        Debug.Log($"[Mortar] PestleHit progress={grindProgress}");
        OnProgressChanged?.Invoke(grindProgress);
    }

    private void DecayProgress()
    {
        grindProgress *= PROGRESS_DECAY;
        if (grindProgress < 0.0001f)
            grindProgress = 0f;
        Debug.Log($"[Mortar] Progress decayed to {grindProgress}");
        OnProgressChanged?.Invoke(grindProgress);
    }

    public bool TryCraft()
    {
        if (grindProgress < 0.999f)
            return false;
        var amountMap = solids.GroupBy(s => (IngredientDef)s.Def).ToDictionary(g => g.Key, g => (float)g.Sum(x => x.amount));
        var inputSet = new HashSet<IngredientDef>(amountMap.Keys);
        var candidates = recipes.Where(r => new HashSet<IngredientDef>(r.reactants.Select(x => x.ingredient)).SetEquals(inputSet)).ToList();
        if (candidates.Count == 0)
        {
            Debug.Log("[Mortar] No candidate recipe, produce by-product");
            MakeByproduct(amountMap);
            return true;
        }
        float BestScore(MortarRecipe r)
        {
            float totalInput = amountMap.Values.Sum();
            int totalRecipe = r.reactants.Sum(x => x.amount);
            float scale = totalInput / totalRecipe;
            float diff = 0f;
            foreach (var ra in r.reactants)
            {
                amountMap.TryGetValue(ra.ingredient, out float have);
                diff += Mathf.Abs(have - ra.amount * scale);
            }
            float score = 1f - diff / totalInput;
            return Mathf.Clamp01(score);
        }
        var best = candidates.OrderByDescending(BestScore).First();
        float bestSim = BestScore(best);
        Debug.Log($"[Mortar] BestSim={bestSim}");
        if (bestSim < 0.80f)
        {
            Debug.Log("[Mortar] Similarity < 80%, produce by-product");
            MakeByproduct(amountMap);
            return true;
        }
        ProduceRecipe(best, amountMap);
        return true;
    }

    private void ProduceRecipe(MortarRecipe recipe, Dictionary<IngredientDef, float> amountMap)
    {
        int totalIn = amountMap.Values.Sum(x => (int)x);
        int totalNeed = recipe.reactants.Sum(x => x.amount);
        int factor = Mathf.Max(1, totalIn / totalNeed);
        float outVol = recipe.product.volume * factor;

        FluidStack outStack = new FluidStack(recipe.product.Def, outVol)
        {
            tags = recipe.product.tags != null ? new List<IngredientTag>(recipe.product.tags) : null
        };
        outStack = recipe.product.CopyWithVolume(outVol);
        Debug.Log($"[Mortar] ProduceRecipe {outStack.Def.GetId()} volume={outStack.volume}");
        
        List<IngredientTag> tagMap = BuildTagMap();
        foreach (var tag in tagMap)
        {
            outStack.tags.Add(new IngredientTag(tag));
        }

        
        fluidStorage.Fill(outStack);

        solids.Clear();
        grindProgress = 0f;
        OnProgressChanged?.Invoke(grindProgress);
        OnInventoryChanged?.Invoke();
        OnCraftComplete?.Invoke(true);
    }

    private void MakeByproduct(Dictionary<IngredientDef, float> amountMap)
    {
        float totalVol = amountMap.Values.Sum();
        if (totalVol <= 0f)
            return;

        List<IngredientTag> tagMap = BuildTagMap();
        FluidStack mixed = new FluidStack(mixedDustDef, totalVol) { tags = new List<IngredientTag>() };
        foreach (var tag in tagMap)
        {
            mixed.tags.Add(new IngredientTag(tag));
        }
        mixed.tags.Add(dustTag);
        
        Debug.Log($"[Mortar] MakeByproduct mixed_dust volume={totalVol} tagCount={tagMap.Count}");
        fluidStorage.Fill(mixed);

        solids.Clear();
        grindProgress = 0f;
        OnProgressChanged?.Invoke(grindProgress);
        OnInventoryChanged?.Invoke();
        OnCraftComplete?.Invoke(false);
    }

    private List<IngredientTag> BuildTagMap()
    {
        var dict = new Dictionary<IngredientTagDef, float>();

        foreach (var st in solids)
        {
            if (st.IsEmpty || st.tags == null) continue;

            foreach (var t in st.tags)
            {
                float add = t.value * st.amount;

                if (dict.TryGetValue(t.ingredientTagDef, out var cur))
                    dict[t.ingredientTagDef] = cur + add;
                else
                    dict[t.ingredientTagDef] = add;
            }
        }

        return dict.Select(kv => new IngredientTag(kv.Key, kv.Value)).ToList();
    }

    
    //DEBUG
    
    public List<ItemStack> GetItemStorage()
    {
        return solids;
    }
    
    public FluidStorage GetFluidStorage()
    {
        return fluidStorage;
    }
}