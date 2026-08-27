using UnityEngine;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance;
    public List<WeaponRecipeSO> recipe;
    public List<WeaponRecipeSO> craftedWeapons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanCraft(WeaponRecipeSO weaponRecipeSO)
    {
        foreach (RecipeRequirement requirement in weaponRecipeSO.requirements)
        {
            if (Inventory.instance.ItemInventoryDictionary[requirement.itemName] < requirement.amount)
            {
                return false;
            }
        }
        return true;
    }
    public void Craft(WeaponRecipeSO weaponRecipeSO)
    {
        if (CanCraft(weaponRecipeSO))
        {
            foreach (RecipeRequirement requirement in weaponRecipeSO.requirements)
            {
                Inventory.instance.ItemInventoryDictionary[requirement.itemName] -= requirement.amount;
            }
            craftedWeapons.Add(weaponRecipeSO);
        }
    }
}
