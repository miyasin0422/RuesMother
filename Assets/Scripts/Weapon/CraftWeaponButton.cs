using UnityEngine;
using UnityEngine.UI;

public class CraftWeaponButton : MonoBehaviour
{
    [SerializeField]
    private CraftUI craftUI;
    [SerializeField]
    private GameObject checkMarkObject;
    [SerializeField]
    private Button button;
    public int index;
    public WeaponRecipeSO weaponRecipe;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Selected()
    {
        craftUI.SelectWeapon(index, weaponRecipe);
    }
    public void Craft()
    {
        craftUI.CraftWeapon();
    }
    public void SetCraftedState(bool isCrafted)
    {
        if (checkMarkObject != null)
        {
            checkMarkObject.SetActive(isCrafted);
        }
        if(button != null)
        {
            button.image.color = Color.gray;
        }
    }
    public void SetCanCraftState(bool CanCraft)
    {
        if (button != null)
        {
            if (CanCraft)
            {
                button.image.color = Color.white;
            }
            else
            {
                button.image.color = Color.gray;
            }
        }
    }
}
