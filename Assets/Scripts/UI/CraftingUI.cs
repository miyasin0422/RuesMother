using UnityEngine;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    public TMP_Text inventoryText;
    public TMP_Text craftedWeaponsText;
    public WeaponRecipeSO targetRecipe;
    public int selectedIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateInventoryText();
        UpdateCraftedWeaponsText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void UpdateInventoryText()
    {
        string text = "";
        foreach (var item in Inventory.instance.ItemInventoryDictionary)
        {
            text += item.Key + ": " + item.Value + "\n";
        }
        inventoryText.text = text;
    }
    void UpdateCraftedWeaponsText()
    {
        string text = "";
        foreach(var item in CraftingManager.instance.craftedWeapons)
        {
            text += item.weaponName + "\n";
        }
        craftedWeaponsText.text = text;
    }
    public void OnCraftButtonClicked()
    {
        CraftingManager.instance.Craft(targetRecipe);
        UpdateInventoryText();
        UpdateCraftedWeaponsText();
    }
    public void AssignLeftWeapon()
    {
        if (CraftingManager.instance.craftedWeapons.Count == 0) return;
        PlayerLoadout.instance.leftWeapon = CraftingManager.instance.craftedWeapons[selectedIndex].weaponPrefab;
    }

    public void AssignRightWeapon()
    {
        if (CraftingManager.instance.craftedWeapons.Count == 0) return;
        PlayerLoadout.instance.rightWeapon = CraftingManager.instance.craftedWeapons[selectedIndex].weaponPrefab;
    }
}
