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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        CanAssignCheck();
        CheckAssignedWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        leftSlot.sprite = null;
        rightSlot.sprite = null;
        foreach (var weaponButton in WeaponButton)
        {
            bool isLeftWeapon = PlayerLoadout.instance.leftWeapon == weaponButton.weaponPrefab;

            bool isRightWeapon = PlayerLoadout.instance.rightWeapon == weaponButton.weaponPrefab;

            weaponButton.SetAssignedState(
                isLeftWeapon || isRightWeapon
            );
            if (isLeftWeapon)
            {
                SetSlotWeapon(leftSlot, weaponButton);
            }
            if (isRightWeapon)
            {
                SetSlotWeapon(rightSlot, weaponButton);
            }
        }
    }
    public void SetSlotWeapon(Image slotImage, AssignWeaponButton weaponButton)
    {
        Image sourceImage = weaponButton.DragObjectPrefab.transform.Find("WeaponImage").GetComponentInChildren<Image>();
        slotImage.sprite = sourceImage.sprite;

        RectTransform sourceRect = sourceImage.GetComponent<RectTransform>();

        RectTransform slotRect = slotImage.GetComponent<RectTransform>();

        slotRect.sizeDelta = sourceRect.sizeDelta;
        slotRect.localScale = sourceRect.localScale;
    }

}
