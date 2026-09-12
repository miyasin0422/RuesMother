using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CraftingUI : MonoBehaviour
{
    public TMP_Text inventoryText;
    public GameObject recipeButtonPrefab;
    public Transform recipeContainer;
    public GameObject craftedWeaponButtonPrefab;
    public Transform craftedWeaponContainer;

    private WeaponRecipeSO selectedRecipe;
    public int selectedIndex = -1;
    private List<RecipeButtonUI> recipeButtons = new List<RecipeButtonUI>();
    private List<CraftedWeaponButtonUI> craftedWeaponButtons = new List<CraftedWeaponButtonUI>();
    public TMP_Text leftWeaponText;
    public TMP_Text rightWeaponText;

    void Start()
    {
        UpdateInventoryText();
        RefreshRecipeList();
        RefreshCraftedWeaponList();
        UpdateLoadoutText();
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

    void RefreshRecipeList()
    {
        recipeButtons.Clear();
        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (var recipe in CraftingManager.instance.recipe)
        {
            GameObject obj = Instantiate(recipeButtonPrefab, recipeContainer);
            RecipeButtonUI btn = obj.GetComponent<RecipeButtonUI>();
            btn.Setup(recipe, this);
            obj.GetComponent<UnityEngine.UI.Button>().interactable = CraftingManager.instance.CanCraft(recipe);
            recipeButtons.Add(btn);
        }
    }

    void RefreshCraftedWeaponList()
    {
        craftedWeaponButtons.Clear();
        foreach (Transform child in craftedWeaponContainer)
        {
            Destroy(child.gameObject);
        }
        int index = 0;
        foreach (var weapon in CraftingManager.instance.craftedWeapons)
        {
            int i = index;
            GameObject obj = Instantiate(craftedWeaponButtonPrefab, craftedWeaponContainer);
            CraftedWeaponButtonUI btn = obj.GetComponent<CraftedWeaponButtonUI>();
            btn.Setup(weapon, i, this);
            index++;
            craftedWeaponButtons.Add(btn);
        }
    }

    public void SelectRecipe(WeaponRecipeSO recipe)
    {
        selectedRecipe = recipe;
        foreach (var btn in recipeButtons)
        {
            btn.SetSelected(btn.recipe == recipe);
        }
    }

    public void SelectCraftedWeapon(int index)
    {
        selectedIndex = index;
        foreach (var btn in craftedWeaponButtons)
        {
            btn.SetSelected(btn.index == index);
        }
    }

    public void OnCraftButtonClicked()
    {
        if (selectedRecipe == null) return;
        CraftingManager.instance.Craft(selectedRecipe);
        UpdateInventoryText();
        RefreshRecipeList();
        RefreshCraftedWeaponList();
    }
    void UpdateLoadoutText()
    {
        leftWeaponText.text = "" + GetWeaponName(PlayerLoadout.instance.leftWeapon);
        rightWeaponText.text = "" + GetWeaponName(PlayerLoadout.instance.rightWeapon);
    }

    string GetWeaponName(GameObject prefab)
    {
        if (prefab == null) return "None";
        foreach (var weapon in CraftingManager.instance.craftedWeapons)
        {
            if (weapon.weaponPrefab == prefab)
            {
                return weapon.weaponName;
            }
        }
        return "Unknown";
    }

    public void AssignLeftWeapon()
    {
        if (selectedIndex < 0) return;
        PlayerLoadout.instance.leftWeapon = CraftingManager.instance.craftedWeapons[selectedIndex].weaponPrefab;
        UpdateLoadoutText();
    }

    public void AssignRightWeapon()
    {
        if (selectedIndex < 0) return;
        PlayerLoadout.instance.rightWeapon = CraftingManager.instance.craftedWeapons[selectedIndex].weaponPrefab;
        UpdateLoadoutText();
    }
}