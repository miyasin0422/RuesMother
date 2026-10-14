using System.Collections.Generic;
using UnityEngine;
using Gene;

public static class CraftingCalculator
{
    public static GameObject DetermineWeaponPrefab(List<GeneSO> baseGenes, List<Gene.WeaponRecipeSO> recipeDatabase)
    {
        if (baseGenes == null || baseGenes.Count == 0)
        {
            Debug.LogWarning("[Crafting] Base遺伝子がセットされていません。");
            return null;
        }

        if (recipeDatabase == null || recipeDatabase.Count == 0)
        {
            Debug.LogWarning("[Crafting] レシピデータベースが空です。");
            return null;
        }
        foreach(var recipe in recipeDatabase)
        {
            if(recipe != null && recipe.IsMatch(baseGenes))
            {
                return recipe.baseWeaponPrefab;
            }
        }
        return null;
    }
}
