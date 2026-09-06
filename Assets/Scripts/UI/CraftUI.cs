using TMPro;
using UnityEngine;

public class CraftUI : MonoBehaviour
{
    [SerializeField]
    private CraftWeaponButton[] WeaponButton;
    [SerializeField]
    private TMP_Text descriptionText;
    public int selectedIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is crea
    void OnEnable()
    {
        CraftedCheck();
        CanCraftDisplay();
        SelectWeapon(0, WeaponButton[0].weaponRecipe);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CraftedCheck()
    {
        foreach(var weapon in WeaponButton)
        { 
            weapon.SetCraftedState(CraftingManager.instance.IsCrafted(weapon.weaponRecipe));
        }
    }
    public void CanCraftDisplay()
    {
        foreach (var weapon in WeaponButton)
        {
            if (!CraftingManager.instance.IsCrafted(weapon.weaponRecipe))
            {
                weapon.SetCanCraftState(CraftingManager.instance.CanCraft(weapon.weaponRecipe));
            }
        }
    }
    public void SelectWeapon(int index, WeaponRecipeSO recipe)
    {
        selectedIndex = index;
        descriptionText.text = "【ぶきのせつめい】\n\n"+ recipe.weaponDescription;
    }
    public void CraftWeapon()
    {
        if (CraftingManager.instance.CanCraft(WeaponButton[selectedIndex].weaponRecipe))
        {
            CraftingManager.instance.Craft(WeaponButton[selectedIndex].weaponRecipe);
            WeaponButton[selectedIndex].SetCraftedState(true);
            CanCraftDisplay();
        }
    }
    
}
