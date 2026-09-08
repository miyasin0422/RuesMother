using UnityEngine;

public class AssignUI : MonoBehaviour
{
    [SerializeField]
    private AssignWeaponButton[] WeaponButton;
    public int selectedIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SelectWeapon(int index)
    {
        selectedIndex = index;

        Debug.Log("選択した武器のIndex：" + selectedIndex);
    }
    public void CanAssignCheck()
    {
        foreach(var weapon in WeaponButton)
        {
            weapon.CanAssignState(CraftingManager.instance.IsCrafted(weapon.weaponRecipe));
        }
    }
    /*
    public void IsAssign()
    {
        foreach (var weapon in WeaponButton)
        {
            weapon.AssignWeapon();
        }
    }
    */
}
