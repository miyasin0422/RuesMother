using System.Collections.Generic;
using UnityEngine;

namespace Gene
{
    [System.Serializable]
    public class EquippedWeaponData
    {
        public GameObject weaponPrefab;
        public WeaponParam param;
        public string weaponName;
        public string description;
        public List<GeneSO> usedGenes; // ★ 錬成に使用された遺伝子リストを追加

        public EquippedWeaponData(
            GameObject prefab,
            WeaponParam param,
            string name,
            string desc,
            List<GeneSO> genes = null)
        {
            this.weaponPrefab = prefab;
            this.param = param;
            this.weaponName = name;
            this.description = desc;
            // リストの参照切り（Deep Copy）のためにコピーして保持
            this.usedGenes = genes != null ? new List<GeneSO>(genes) : new List<GeneSO>();
        }
    }
}