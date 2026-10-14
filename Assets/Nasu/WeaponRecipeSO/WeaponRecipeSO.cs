using UnityEngine;
using System.Collections.Generic;

namespace Gene
{
    [CreateAssetMenu(menuName = "Weapon/Gene")]
    public class WeaponRecipeSO : ScriptableObject
    {
        public string weaponName;
        public List<GeneSO> requiredGenes;
        public GameObject baseWeaponPrefab;

        public bool IsMatch(List<GeneSO> inputBaseGenes)
        {
            if (inputBaseGenes == null || requiredGenes == null) return false;

            // 1. 入力された遺伝子の「種類（ユニークなHashSet）」を抽出
            HashSet<GeneSO> uniqueInput = new HashSet<GeneSO>(inputBaseGenes);
            // 2. レシピ側の必要遺伝子も「種類」に変換
            HashSet<GeneSO> uniqueRequired = new HashSet<GeneSO>(requiredGenes);

            // 3. 種類の数が異なれば不一致（例: 剣のみのレシピに「剣＋弓」が入っている場合など）
            if (uniqueInput.Count != uniqueRequired.Count) return false;

            // 4. 全ての種類が含まれているかチェック
            return uniqueInput.SetEquals(uniqueRequired);
        }
    }
}

