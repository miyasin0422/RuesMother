using System.Collections.Generic;
using System.Linq;
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

        // ベース遺伝子が1つもない場合は不成立
        if (baseGenes.Count == 0)
        {
            return new EquippedWeaponData(
                null,
                new WeaponParam(),
                "未完成の構造",
                "ベース遺伝子が含まれていません。武器の核となる素材（剣・弓など）を配置してください。"
            );
        }

        // 2. ステータス計算
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

        // 同種ベース遺伝子3つ特化ボーナス（例: 剣×3 でステータス1.3倍）
        bool isSpecialized = baseGenes.Count >= 3 && baseGenes.All(g => g == baseGenes[0]);
        if (isSpecialized)
        {
            baseAtk *= 1.3f;
            baseSpeed *= 1.2f;
        }

        bool isAcid = mutationGenes.Any(m => m.element == ElementType.Acid);
        ElementType mainElem = mutationGenes.Count > 0 ? mutationGenes[0].element : ElementType.Base;

        // 3. 自動命名 ＆ 説明文生成
        string name = BuildWeaponName(baseGenes, mutationGenes, isSpecialized);
        string desc = BuildDescription(baseGenes, mutationGenes, isSpecialized);

        // 4. Prefabの特定
        GameObject prefab = DetermineWeaponPrefab(baseGenes, recipeDatabase);

        WeaponParam param = new WeaponParam(baseAtk, baseSpeed, baseRange, isAcid, elemDmg, mainElem);
        return new EquippedWeaponData(prefab, param, name, desc);
    }

    /// <summary>
    /// 素材構成からの自動命名
    /// </summary>
    private static string BuildWeaponName(List<GeneSO> baseGenes, List<GeneSO> mutationGenes, bool isSpecialized)
    {
        string prefix = mutationGenes.Count > 0 ? mutationGenes[0].weaponPrefix : "";
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
    /// 説明文の動的自動構築
    /// </summary>
    private static string BuildDescription(List<GeneSO> baseGenes, List<GeneSO> mutationGenes, bool isSpecialized)
    {
        string desc = "";
        if (isSpecialized)
        {
            desc += $"{baseGenes[0].geneName}の遺伝子を極限まで高めた精巧な逸品。";
        }
        else
        {
            desc += $"{baseGenes[0].geneName}の性質をベースにした標準構造。";
        }

        if (mutationGenes.Count > 0 && !string.IsNullOrEmpty(mutationGenes[0].elementDescription))
        {
            desc += $" {mutationGenes[0].elementDescription}";
        }

        return desc;
    }

    public static GameObject DetermineWeaponPrefab(List<GeneSO> baseGenes, List<Gene.WeaponRecipeSO> recipeDatabase)
    {
        if (baseGenes == null || baseGenes.Count == 0 || recipeDatabase == null) return null;
        foreach (var recipe in recipeDatabase)
        {
            if (recipe != null && recipe.IsMatch(baseGenes)) return recipe.baseWeaponPrefab;
        }
        return null;
    }
}