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

        public EquippedWeaponData(GameObject prefab, WeaponParam weaponParam, string name, string desc)
        {
            weaponPrefab = prefab;
            param = weaponParam;
            weaponName = name;
            description = desc;
        }
    }
}