using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponDropSlot : MonoBehaviour,
    IDropHandler
{
    public enum SlotType
    {
        Left,
        Right
    }
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private AssignUI assignUI;
    [SerializeField]
    private SlotType slotType;
    public void OnDrop(PointerEventData eventData)
    {
        AssignWeaponButton weaponButton = eventData.pointerDrag?.GetComponent<AssignWeaponButton>();

        if (weaponButton == null)
        {
            return;
        }

        if (slotType == SlotType.Left)
        {
            PlayerLoadout.instance.leftWeapon = weaponButton.weaponPrefab;
            if(PlayerLoadout.instance.rightWeapon == weaponButton.weaponPrefab)
            {
                PlayerLoadout.instance.rightWeapon = null;
            }
            assignUI.CheckAssignedWeapons();
        }
        else if (slotType == SlotType.Right)
        {
            PlayerLoadout.instance.rightWeapon = weaponButton.weaponPrefab;
            if (PlayerLoadout.instance.leftWeapon == weaponButton.weaponPrefab)
            {
                PlayerLoadout.instance.leftWeapon = null;
            }
            assignUI.CheckAssignedWeapons();
            uiManager.AssignedWeaponsShow();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
