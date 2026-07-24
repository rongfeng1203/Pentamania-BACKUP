using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public interface IngredientStack
{
    public IngredientDef GetAbstractDef();
    public IngredientTag[] GetTags();
    public IngredientStack CopyAbs();
}

[System.Serializable]
public struct ItemStack : IStackable<ItemStack>, IngredientStack
{
    [SerializeField]
    private ItemDef def;

    public ItemDef Def
    {
        get => def;
        private set => def = value;
    }

    public int amount;
    public List<IngredientTag> tags;

    public ItemStack(ItemDef def, int amount, List<IngredientTag> tags)
    {
        this.def = def;
        this.amount = amount;
        this.tags = tags;
    }
    
    public ItemStack(ItemDef def, int amount)
    {
        this.def = def;
        this.amount = amount;
        this.tags = new List<IngredientTag>();
    }

    public bool IsEmpty => Def == null || amount <= 0;

    /*
    public bool CanMerge(ItemStack other) =>
        Def == other.Def && !IsEmpty && !other.IsEmpty;
        */

    public bool CanMerge(ItemStack other)
    {
        
        bool tagEqual = (tags == null && other.tags == null) ||
                        (tags != null && other.tags != null && TagListsEqual(tags, other.tags));
        bool defEqual = Def == other.Def;
        bool notEmpty = !IsEmpty && !other.IsEmpty;
        //Debug.Log($"[Item Stack] Tags: {tagEqual}, Defs: {defEqual}, Not Empty: {notEmpty}");
        return defEqual && tagEqual && notEmpty;
    }

    public void Merge(ref ItemStack other)
    {
        if (!CanMerge(other)) return;
        int room = Def.GetMaxStack() - amount;
        int toMove = Mathf.Min(room, other.amount);
        amount += toMove;
        other.amount -= toMove;
    }

    public IngredientDef GetAbstractDef()
    {
        return Def;
    }
    
    public readonly ItemStack CopyWithAmount(int newAmount)
    {
        List<IngredientTag> newTags = null;

        if (tags != null)
        {
            newTags = new List<IngredientTag>(tags.Count);
            foreach (var tag in tags)
            {
                newTags.Add(new IngredientTag(tag));
            }
        }
        
        //var newTags = tags == null ? null : new List<IngredientTag>(tags);
        return new ItemStack(def, newAmount, newTags);
    }
    
    public readonly IngredientStack CopyAbs()
    {
        List<IngredientTag> newTags = null;

        if (tags != null)
        {
            newTags = new List<IngredientTag>(tags.Count);
            foreach (var tag in tags)
            {
                newTags.Add(new IngredientTag(tag));
            }
        }
        
        // var newTags = tags == null ? null : new List<IngredientTag>(tags);
        //Debug.Log("[Fluid Stack] " + newTags.Count);
        return new ItemStack(def, amount, newTags);
    }
    
    private static bool TagListsEqual(List<IngredientTag> a, List<IngredientTag> b)
    {
        if (ReferenceEquals(a, b))
        {
            //Debug.Log($"[Item Stack] Comparing tags: Reference equals");
            return true;
        }

        if (a == null || b == null)
        {
            //Debug.Log($"[Item Stack] Comparing tags: One of the tags not exists");
            return false;
        }

        if (a.Count != b.Count)
        {
            //Debug.Log($"[Item Stack] Comparing tags: Quantity different");
            return false;
        }

        //Debug.Log($"[Item Stack] Comparing tags: Final judgment");
        return !a.Except(b).Any() && !b.Except(a).Any();
    }

    public IngredientTag[] GetTags()
    {
        return tags.ToArray();
    }

    public static readonly ItemStack Empty = new ItemStack(null, 0);

}

[System.Serializable]
public struct FluidStack : IngredientStack, IStackable<FluidStack>
{
    [SerializeField]
    private FluidDef def;

    public FluidDef Def
    {
        get => def;
        private set => def = value;
    }

    public float volume;
    public List<IngredientTag> tags;

    public FluidStack(FluidDef def, float volume, List<IngredientTag> tags)
    {
        this.def = def;
        this.volume = volume;
        this.tags = tags;
    }

    public FluidStack(FluidDef def, float volume)
    {
        this.def = def;
        this.volume = volume;
        this.tags = new List<IngredientTag>();
    }

    public bool IsEmpty => Def == null || volume <= 0;

    public IngredientDef GetAbstractDef()
    {
        return Def;
    }

    /*
    public bool CanMerge(FluidStack other)
    {
        return other.Def.GetId().Equals(this.Def.GetId());
    }
    */
    
    public bool CanMerge(FluidStack other)
    {
        
        bool tagEqual = (tags == null && other.tags == null) ||
                        (tags != null && other.tags != null && TagListsEqual(tags, other.tags));
        bool defEqual = Def == other.Def;
        bool notEmpty = !IsEmpty && !other.IsEmpty;
        //Debug.Log($"[Fluid Stack] Tags: {tagEqual}, Defs: {defEqual}, Not Empty: {notEmpty}");
        return defEqual && tagEqual && notEmpty;
    }


    public void Merge(ref FluidStack other)
    {
        if (CanMerge(other))
        {
            volume += other.volume;
        }
        else
        {
            throw new ArgumentException("The other fluid stack must pass the CanMerge function to Merge with this fluid stack.");
        }
    }
    
    public readonly FluidStack CopyWithVolume(float newVol)
    {
        List<IngredientTag> newTags = null;

        if (tags != null)
        {
            newTags = new List<IngredientTag>(tags.Count);
            foreach (var tag in tags)
            {
                newTags.Add(new IngredientTag(tag));
            }
        }
        
        // var newTags = tags == null ? null : new List<IngredientTag>(tags);
        //Debug.Log("[Fluid Stack] " + newTags.Count);
        return new FluidStack(def, newVol, newTags);
    }
    
    public readonly FluidStack Copy()
    {
        List<IngredientTag> newTags = null;

        if (tags != null)
        {
            newTags = new List<IngredientTag>(tags.Count);
            foreach (var tag in tags)
            {
                newTags.Add(new IngredientTag(tag));
            }
        }
        
        // var newTags = tags == null ? null : new List<IngredientTag>(tags);
        //Debug.Log("[Fluid Stack] " + newTags.Count);
        return new FluidStack(def, volume, newTags);
    }
    
    private static bool TagListsEqual(List<IngredientTag> a, List<IngredientTag> b)
    {
        if (ReferenceEquals(a, b))
        {
            //Debug.Log($"[Item Stack] Comparing tags: Reference equals");
            return true;
        }

        if (a == null || b == null)
        {
            //Debug.Log($"[Item Stack] Comparing tags: One of the tags not exists");
            return false;
        }

        if (a.Count != b.Count)
        {
            //Debug.Log($"[Item Stack] Comparing tags: Quantity different");
            return false;
        }
        
        //Debug.Log($"[Item Stack] Comparing tags: Final judgment");
        //Debug.Log($"[Item Stack] First: {a[0].id}, Second: {b[0].id}");
        return !a.Except(b).Any() && !b.Except(a).Any();
    }

    public IngredientTag[] GetTags()
    {
        return tags.ToArray();
    }

    public IngredientStack CopyAbs()
    {
        List<IngredientTag> newTags = null;

        if (tags != null)
        {
            newTags = new List<IngredientTag>(tags.Count);
            foreach (var tag in tags)
            {
                newTags.Add(new IngredientTag(tag));
            }
        }
        
        // var newTags = tags == null ? null : new List<IngredientTag>(tags);
        //Debug.Log("[Fluid Stack] " + newTags.Count);
        return new FluidStack(def, volume, newTags);
    }

    public static readonly FluidStack Empty = new FluidStack(null, 0);
    
}