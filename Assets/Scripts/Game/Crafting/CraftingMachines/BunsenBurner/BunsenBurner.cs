using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BunsenBurner
{
    [Serializable]
    private class ProcessEntry
    {
        public SolidObject solid;
        public ItemStack inStack;
        public float timeLeft;
        public bool matched;
        public BunsenBurnerRecipe recipe;
    }

    private readonly List<BunsenBurnerRecipe> recipes;
    private readonly IngredientTagDef burnedTagDef;
    private readonly float fallbackCookTime;

    private readonly List<ProcessEntry> processes = new();

    public event Action<SolidObject> OnCookFinish;
    public event Action OnInventoryChanged;

    public BunsenBurner(IEnumerable<BunsenBurnerRecipe> recipeSrc,
                        IngredientTagDef burnedTagDef,
                        float fallbackCookTime = 3f)
    {
        recipes = new List<BunsenBurnerRecipe>(recipeSrc);
        this.burnedTagDef = burnedTagDef;
        this.fallbackCookTime = Mathf.Max(0.1f, fallbackCookTime);
    }

    #region API
    public void InsertSolid(SolidObject so)
    {
        if (!so) return;

        ItemStack stack = (ItemStack)so.GetIngredient();
        if (stack.IsEmpty) return;

        if (HasBurnedTag(stack)) return;

        BunsenBurnerRecipe recipe = recipes
            .FirstOrDefault(r => r.input.Def == stack.Def);

        float cookTime = recipe ? recipe.burnTime : fallbackCookTime;

        processes.Add(new ProcessEntry
        {
            solid = so,
            inStack = stack.CopyWithAmount(stack.amount),
            matched = recipe != null,
            recipe = recipe,
            timeLeft = cookTime
        });

        Debug.Log($"[Bunsen Burner] Inserted {stack.Def.GetId()}  cookTime = {cookTime}s");
        OnInventoryChanged?.Invoke();
    }

    public void RemoveSolid(SolidObject so)
    {
        processes.RemoveAll(p => p.solid == so);
        OnInventoryChanged?.Invoke();
    }
    #endregion

    #region Drive
    public void Tick(float dt)
    {
        if (processes.Count == 0) return;

        for (int i = processes.Count - 1; i >= 0; i--)
        {
            var p = processes[i];
            p.timeLeft -= dt;

            if (p.timeLeft > 0f) continue;

            if (p.matched)
                ApplyRecipe(p);
            /*
            else
                ApplyBurnLogic(p);
                */

            OnCookFinish?.Invoke(p.solid);
            processes.RemoveAt(i);
            OnInventoryChanged?.Invoke();
        }
    }
    #endregion

    #region Process
    private void ApplyRecipe(ProcessEntry p)
    {
        ItemStack prod = p.recipe.output.CopyWithAmount(p.inStack.amount);
        ItemStack st = p.inStack.CopyWithAmount(p.inStack.amount);
        
        if (!st.tags.Any())
            st.tags.Add(new IngredientTag(burnedTagDef, 0.001f));
        else
        {
            IngredientTag maxTag = st.tags.OrderByDescending(t => t.value).First();
            foreach (var t in st.tags)
                if (t != maxTag) t.value *= 2f;

            st.tags.Add(new IngredientTag(burnedTagDef, 0.001f));
        }

        foreach (var tag in st.tags)
        {
            prod.tags.Add(tag);
        }
        
        // foreach (Transform t in p.solid.GetComponentsInChildren<Transform>(true)) if (t != p.solid.transform && t.name == "Color Affordance") GameObject.Destroy(t.gameObject);
        p.solid.SetIngredient(prod);
        //p.solid.GetComponent<Renderer>().sharedMaterial.SetColor("_BaseColor", new Color(0,0,0,1));
        Debug.Log($"[Bunsen Burner] Recipe found: {prod.Def.GetId()}");
    }

    private void ApplyBurnLogic(ProcessEntry p)
    {
        ItemStack st = p.inStack.CopyWithAmount(p.inStack.amount);

        if (st.tags == null)
            st.tags = new List<IngredientTag>();

        if (!st.tags.Any())
            st.tags.Add(new IngredientTag(burnedTagDef, 0.001f));
        else
        {
            IngredientTag maxTag = st.tags.OrderByDescending(t => t.value).First();
            foreach (var t in st.tags)
                if (t != maxTag) t.value *= 2f;

            st.tags.Add(new IngredientTag(burnedTagDef, 0.001f));
        }

        p.solid.SetIngredient(st);
        Debug.Log("[Bunsen Burner] Burned, tags adjusted & burned added");
    }
    #endregion

    private bool HasBurnedTag(ItemStack s) =>
        s.tags != null && s.tags.Any(t => t.ingredientTagDef == burnedTagDef);
}