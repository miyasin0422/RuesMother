using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image[] refreshItems;
    [SerializeField] private float targetFillAmount;
    [SerializeField] private Image[] weaponImages;
    [SerializeField] private Image leftWeaponImage;
    [SerializeField] private Image rightWeaponImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPUpdate();
        RefreshItemUpdate();
        AssignedWeaponsShow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HPUpdate()
    {
        targetFillAmount = (float)PlayerStatus.playerHealth / PlayerStatus.MaxplayerHealth;
        hpBar.fillAmount = targetFillAmount;
        Debug.Log(targetFillAmount);
    }
    public void RefreshItemUpdate()
    {
        for(int i = 0; i < 3; i++)
        {
            if(i < PlayerStatus.refreshItemStock)
            {
                refreshItems[i].gameObject.SetActive(true);
            }
            else
            {
                refreshItems[i].gameObject.SetActive(false);
            }
            
        }
    }
    public void AssignedWeaponsShow()
    {
        leftWeaponImage.sprite = null;
        rightWeaponImage.sprite = null;
        foreach (Image weaponImage in weaponImages)
        {
            bool isLeftWeapon = PlayerLoadout.instance.leftWeapon == weaponImage.GetComponent<AssignWeaponButton>().weaponPrefab;

            bool isRightWeapon = PlayerLoadout.instance.rightWeapon == weaponImage.GetComponent<AssignWeaponButton>().weaponPrefab; ;

            if (isLeftWeapon)
            {
               leftWeaponImage.sprite = weaponImage.sprite;
               RectTransform sourceRect = weaponImage.GetComponent<RectTransform>();

               RectTransform slotRect = leftWeaponImage.GetComponent<RectTransform>();

               slotRect.sizeDelta = sourceRect.sizeDelta;
               slotRect.localScale = sourceRect.localScale;
            }
            if (isRightWeapon)
            {
                rightWeaponImage.sprite = weaponImage.sprite;
                RectTransform sourceRect = weaponImage.GetComponent<RectTransform>();

                RectTransform slotRect = rightWeaponImage.GetComponent<RectTransform>();

                slotRect.sizeDelta = sourceRect.sizeDelta;
                slotRect.localScale = sourceRect.localScale;
            }
        }
    }
}
