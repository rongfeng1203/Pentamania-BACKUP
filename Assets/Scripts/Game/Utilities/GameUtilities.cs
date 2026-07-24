using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public static class Utilities
    {
        public static class Crafting
        {
            public static bool TagListsEqual(List<IngredientTag> a, List<IngredientTag> b)
            {
                if (ReferenceEquals(a, b)) return true;
                if (a == null || b == null) return false;
                if (a.Count != b.Count) return false;
                return !a.Except(b).Any() && !b.Except(a).Any();
            }
        }

    }
}
