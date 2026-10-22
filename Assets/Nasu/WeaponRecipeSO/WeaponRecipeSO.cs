using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gene
{
    [CreateAssetMenu(fileName = "WeaponRecipe_", menuName = "Gene/Weapon Recipe")]
    public class WeaponRecipeSO : ScriptableObject
    {
        [Header("レシピ識別設定")]
        public string recipeName;                  // 例: "ナイフ", "剣", "弓"
        public List<GeneSO> requiredBaseGenes;     // 必要素材リスト（ベース遺伝子のみ）
        public GameObject baseWeaponPrefab;        // 実体化するPrefab

        [Header("基本テキスト設定")]
        [TextArea(2, 4)]
        [Tooltip("武器種の基本挙動（例: '敵に向けてまっすぐナイフを投擲する' など）")]
        public string baseDescription;

        /// <summary>
        /// 配置された遺伝子リストから「ベース遺伝子」だけを抽出してレシピと合致するか判定
        /// （変異因子が含まれていても無視してベース素材のみで一致判定を行います）
        /// </summary>
        public bool IsMatch(List<GeneSO> inputGenes)
        {
            if (requiredBaseGenes == null || inputGenes == null) return false;

            // 入力リストから null を除外し、ベース遺伝子（ElementType.Base）のみを抽出
            List<GeneSO> inputBaseGenes = inputGenes
                .Where(g => g != null)
                .Distinct()
                .ToList();

            // 必要ベース遺伝子の数と一致しない場合は不成立
            if (requiredBaseGenes.Count != inputBaseGenes.Count) return false;

            // 各ベース遺伝子の個数カウントを比較
            var requiredCounts = requiredBaseGenes
                .Where(g => g != null)
                .GroupBy(g => g)
                .ToDictionary(g => g.Key, g => g.Count());

            var inputCounts = inputBaseGenes
                .GroupBy(g => g)
                .ToDictionary(g => g.Key, g => g.Count());

            if (requiredCounts.Count != inputCounts.Count) return false;

            foreach (var kvp in requiredCounts)
            {
                if (!inputCounts.TryGetValue(kvp.Key, out int inputCount) || inputCount != kvp.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}