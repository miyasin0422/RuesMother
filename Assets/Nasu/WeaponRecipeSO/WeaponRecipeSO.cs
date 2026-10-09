using UnityEngine;
using System.Collections.Generic;

namespace Gene
{
    [CreateAssetMenu(menuName = "Weapon/Gene")]
    public class WeaponRecipeSO : ScriptableObject
    {
        public string weaponName;
        public List<GeneSO> requirementGenes;
        public GameObject baseWeaponPrefab;
    }
}

