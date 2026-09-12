using TMPro;
using UnityEngine;

public class RecipeButtonUI : MonoBehaviour
{
    public TMP_Text buttonText;
    public WeaponRecipeSO recipe;
    public CraftingUI craftingUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Setup(WeaponRecipeSO recipe, CraftingUI craftingUI)
    {
        this.recipe = recipe;
        this.craftingUI = craftingUI;
        buttonText.text = recipe.weaponName;
    }
    public void OnClick()
    {
        craftingUI.SelectRecipe(recipe);
    }
    public void SetSelected(bool selected)
    {
        GetComponent<UnityEngine.UI.Image>().color = selected ? Color.yellow : Color.white;
    }
}
