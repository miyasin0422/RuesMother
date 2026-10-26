using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image hpBar;
    [SerializeField] private UnityEngine.UI.Image[] refreshItems;
    [SerializeField] private float targetFillAmount;

    [Header("装備中の武器アイコン")]
    [SerializeField] private UnityEngine.UI.Image leftWeaponImage;
    [SerializeField] private UnityEngine.UI.Image rightWeaponImage;

    [Header("プレイ用Canvas。覚醒前は非表示、覚醒後に表示")]
    [SerializeField] private GameObject UIRoot;

    [Header("初回チュートリアル中の表示")]
    [SerializeField] private bool hideUntilRueAwakened = true;

    [System.Serializable]
    private class WeaponIconEntry
    {
        public GameObject weaponPrefab;
        public Sprite icon;
    }

    [Header("武器Prefabとアイコンの対応")]
    [SerializeField] private WeaponIconEntry[] weaponIcons;

    void Start()
    {
        HPUpdate();
        RefreshItemUpdate();
        AssignedWeaponsShow();
    }

    void Update()
    {
        AssignedWeaponsShow();
    }

    public void HPUpdate()
    {
        targetFillAmount =
            (float)PlayerStatus.playerHealth / PlayerStatus.MaxplayerHealth;

        hpBar.fillAmount = targetFillAmount;
    }

    public void RefreshItemUpdate()
    {
        for (int i = 0; i < 3; i++)
        {
            refreshItems[i].gameObject.SetActive(
                i < PlayerStatus.refreshItemStock
            );
        }
    }

    public void AssignedWeaponsShow()
    {
        Gene.PlayerLoadout loadout = Gene.PlayerLoadout.instance;

        bool visible =
            loadout != null &&
            (!hideUntilRueAwakened || TutorialProgress.RueAwakened);

        if (UIRoot != null &&
            UIRoot.activeSelf != visible)
        {
            UIRoot.SetActive(visible);
        }

        Sprite leftIcon = null;
        Sprite rightIcon = null;

        if (visible)
        {
            leftIcon = GetWeaponIcon(loadout.LeftWeaponData);
            rightIcon = GetWeaponIcon(loadout.RightWeaponData);
        }

        SetWeaponImage(leftWeaponImage, leftIcon);
        SetWeaponImage(rightWeaponImage, rightIcon);
    }

    private Sprite GetWeaponIcon(Gene.EquippedWeaponData weapon)
    {
        if (
            weapon == null ||
            weapon.weaponPrefab == null ||
            weaponIcons == null)
        {
            return null;
        }

        foreach (WeaponIconEntry entry in weaponIcons)
        {
            if (entry != null &&
                entry.weaponPrefab == weapon.weaponPrefab)
            {
                return entry.icon;
            }
        }

        return null;
    }

    private void SetWeaponImage(
        UnityEngine.UI.Image image,
        Sprite icon)
    {
        if (image == null)
        {
            return;
        }

        if (image.sprite != icon)
        {
            image.sprite = icon;
        }

        bool visible = icon != null;

        if (image.enabled != visible)
        {
            image.enabled = visible;
        }

        image.preserveAspect = true;
    }
}