using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gene;

public static class CraftingCalculator
{
    public static EquippedWeaponData ProcessCrafting(List<GeneSO> inputGenes, List<Gene.WeaponRecipeSO> recipeDatabase)
    {
        if (inputGenes == null || inputGenes.Count == 0) return null;

        // 1. ベース遺伝子と変異因子の分離
        List<GeneSO> baseGenes = inputGenes.Where(g => g != null && g.element == ElementType.Base).ToList();
        List<GeneSO> mutationGenes = inputGenes.Where(g => g != null && g.element != ElementType.Base).ToList();

        if (baseGenes.Count == 0)
        {
            return new EquippedWeaponData(
                null,
                new WeaponParam(),
                "未完成の構造",
                "ベース遺伝子が含まれていません。武器の核となる素材（剣・弓など）を配置してください。"
            );
        }

        // 2. レシピおよび Prefab の判定（Gene.WeaponRecipeSO を明示指定）
        Gene.WeaponRecipeSO matchedRecipe = FindMatchingRecipe(baseGenes, recipeDatabase);
        GameObject prefab = matchedRecipe != null ? matchedRecipe.baseWeaponPrefab : null;

        // 3. ステータス計算
        float baseAtk = 5f;
        float baseSpeed = 0.9f;
        float baseRange = 5f;
        float elemDmg = 0f;

        foreach (var gene in inputGenes)
        {
            if (gene == null) continue;
            baseAtk += gene.atkBonus;
            baseSpeed += gene.atkSpeedBonus;
            baseRange += gene.rangeBonus;
            elemDmg += gene.elementDmgBonus;
        }

        // 同種ベース遺伝子3つ特化ボーナス
        bool isSpecialized = baseGenes.Count >= 3 && baseGenes.All(g => g == baseGenes[0]);
        if (isSpecialized)
        {
            baseAtk *= 1.3f;
            baseSpeed *= 1.2f;
        }

        int acidCount = mutationGenes.Count(
            m => m.element == ElementType.Acid
        );

        bool isAcid = acidCount > 0;
        float acidChance = Mathf.Clamp01(acidCount * 0.25f);
        ElementType mainElem = mutationGenes.Count > 0 ? mutationGenes[0].element : ElementType.Base;

        // 4. 自動命名 ＆ 説明文の多層構築
        string name = BuildWeaponName(baseGenes, mutationGenes, isSpecialized);
        string desc = BuildDescription(baseGenes, mutationGenes, isSpecialized, name, matchedRecipe);

        WeaponParam param = new WeaponParam(
            baseAtk,
            baseSpeed,
            baseRange,
            isAcid,
            elemDmg,
            mainElem,
            acidChance
        );
        return new EquippedWeaponData(prefab, param, name, desc, inputGenes); 
    }

    /// <summary>
    /// 素材構成に一致する Gene.WeaponRecipeSO の検索
    /// </summary>
    private static Gene.WeaponRecipeSO FindMatchingRecipe(List<GeneSO> baseGenes, List<Gene.WeaponRecipeSO> recipeDatabase)
    {
        if (baseGenes == null || baseGenes.Count == 0 || recipeDatabase == null) return null;

        foreach (var recipe in recipeDatabase)
        {
            if (recipe != null && recipe.IsMatch(baseGenes)) return recipe;
        }
        return null;
    }

    /// <summary>
    /// 素材構成からの自動命名（変異個数に応じた接頭辞の強化対応）
    /// </summary>
    private static string BuildWeaponName(List<GeneSO> baseGenes, List<GeneSO> mutationGenes, bool isSpecialized)
    {
        string prefix = GetMutationPrefix(mutationGenes);
        string mainName = "";

        var geneGroups = baseGenes.GroupBy(g => g.geneName).ToList();

        if (geneGroups.Count == 1) // 単一系統（剣のみ、弓のみ等）
        {
            string baseName = geneGroups[0].Key;
            if (isSpecialized) mainName = $"とても強い{baseName}";
            else if (baseGenes.Count == 2) mainName = $"強{baseName}";
            else mainName = $"通常の{baseName}";
        }
        else // 複合系統（剣＋弓 ➔ ナイフ 等）
        {
            bool hasSword = baseGenes.Any(g => g.geneName.Contains("剣"));
            bool hasBow = baseGenes.Any(g => g.geneName.Contains("弓"));
            bool hasAxe = baseGenes.Any(g => g.geneName.Contains("斧"));
            bool hasSpear = baseGenes.Any(g => g.geneName.Contains("槍"));

            if (hasSword && hasBow)
            {
                int swordCount = baseGenes.Count(g => g.geneName.Contains("剣"));
                int bowCount = baseGenes.Count(g => g.geneName.Contains("弓"));

                if (swordCount > bowCount) mainName = "速投ナイフ";
                else if (bowCount > swordCount) mainName = "遠投ナイフ";
                else mainName = "投擲ナイフ";
            }
            else if (hasSword && hasAxe) mainName = "斬撃大斧";
            else if (hasSpear && hasBow) mainName = "投擲用投槍";
            else mainName = "合成兵器";
        }

        return $"{prefix}{mainName}";
    }

    /// <summary>
    /// 変異因子の「種類」と「個数」から接頭辞を動的生成（1個:酸の / 2個:強酸の）
    /// </summary>
    private static string GetMutationPrefix(List<GeneSO> mutationGenes)
    {
        if (mutationGenes == null || mutationGenes.Count == 0) return "";

        var topMutation = mutationGenes.GroupBy(m => m.element)
                                       .OrderByDescending(g => g.Count())
                                       .FirstOrDefault();

        if (topMutation == null) return "";

        int count = topMutation.Count();
        string basePrefix = topMutation.First().weaponPrefix;

        if (string.IsNullOrEmpty(basePrefix)) return "";

        if (count >= 2)
        {
            return $"強{basePrefix}";
        }

        return basePrefix;
    }

    /// <summary>
    /// 説明文の多層自動構築（レシピ基本文章 + 派生特性文 + 特化強度文 + 属性文）
    /// </summary>
    private static string BuildDescription(
        List<GeneSO> baseGenes,
        List<GeneSO> mutationGenes,
        bool isSpecialized,
        string mainName,
        Gene.WeaponRecipeSO matchedRecipe)
    {
        StringBuilder sb = new StringBuilder();

        // A. レシピ（Gene.WeaponRecipeSO）から基本文章を取得
        string baseFuncText = matchedRecipe != null ? matchedRecipe.baseDescription : "";
        if (string.IsNullOrEmpty(baseFuncText))
        {
            baseFuncText = $"{baseGenes[0].geneName}の性質をベースにした構造。";
        }

        sb.Append(baseFuncText);
        if (!baseFuncText.EndsWith("。")) sb.Append("。");

        // B. 派生名（速投/遠投など）に応じた補足文
        string subText = GetNameModifierDescription(mainName);
        if (!string.IsNullOrEmpty(subText))
        {
            sb.Append($" {subText}");
        }

        // C. 特化・強度の文言補足
        if (isSpecialized)
        {
            sb.Append($" {baseGenes[0].geneName}の遺伝子を極限まで極めた精巧な逸品。");
        }
        else if (baseGenes.Count == 2 && baseGenes.All(g => g == baseGenes[0]))
        {
            sb.Append(" 少し威力の高い構造を持つ。");
        }

        // D. 変異因子の属性説明
        if (mutationGenes.Count > 0 && !string.IsNullOrEmpty(mutationGenes[0].elementDescription))
        {
            sb.Append($" {mutationGenes[0].elementDescription}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// 名前のキーワードに基づく特定効果の補足テキスト
    /// </summary>
    private static string GetNameModifierDescription(string weaponName)
    {
        if (weaponName.Contains("速投ナイフ")) return "短い間隔でナイフを投げられる。";
        if (weaponName.Contains("遠投ナイフ")) return "より長い距離まで投げられる。";
        if (weaponName.Contains("斬撃大斧")) return "広い範囲を一気に薙ぎ払う。";

        return "";
    }

    // 既存互換用
    public static GameObject DetermineWeaponPrefab(List<GeneSO> baseGenes, List<Gene.WeaponRecipeSO> recipeDatabase)
    {
        Gene.WeaponRecipeSO recipe = FindMatchingRecipe(baseGenes, recipeDatabase);
        return recipe != null ? recipe.baseWeaponPrefab : null;
    }
}