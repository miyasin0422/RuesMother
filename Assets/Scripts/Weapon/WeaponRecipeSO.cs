using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "Weapon/Recipe")]
public class WeaponRecipeSO : ScriptableObject
{
    public string weaponName;
    public List<RecipeRequirement> requirements;
    public GameObject weaponPrefab;
    [TextArea(3, 10)]
    public string weaponDescription;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
}
