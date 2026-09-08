using UnityEngine;
using UnityEngine.UI;

public class AssignUI : MonoBehaviour
{
    [SerializeField]
    private AssignWeaponButton[] WeaponButton;
    [SerializeField]
    private Image leftSlot;
    [SerializeField]
    private Image rightSlot;

    public int selectedIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CanAssignCheck();
        CheckAssignedWeapons();
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
    public void CheckAssignedWeapons()
    {
        foreach (var weaponButton in WeaponButton)
        {
            bool isLeftWeapon =
                PlayerLoadout.instance.leftWeapon ==
                weaponButton.weaponPrefab;

            bool isRightWeapon =
                PlayerLoadout.instance.rightWeapon ==
                weaponButton.weaponPrefab;

            weaponButton.SetAssignedState(
                isLeftWeapon || isRightWeapon
            );
            if (isLeftWeapon)
            {
                leftSlot.sprite = weaponButton.weaponImage.sprite;
            }
            if (isRightWeapon)
            {
                rightSlot.sprite = weaponButton.weaponImage.sprite;
            }
        }
    }
}
