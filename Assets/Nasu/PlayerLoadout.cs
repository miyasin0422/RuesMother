using UnityEngine;
using Gene;

namespace Gene
{
    public class PlayerLoadout : MonoBehaviour
    {
        public static PlayerLoadout instance;

        [Header("初期装備 Prefab 参照")]
        [SerializeField] private GameObject defaultSwordPrefab;

        [Header("現在の装備データ")]
        [SerializeField] private EquippedWeaponData leftWeaponData;
        [SerializeField] private EquippedWeaponData rightWeaponData;

        public EquippedWeaponData LeftWeaponData => leftWeaponData;
        public EquippedWeaponData RightWeaponData => rightWeaponData;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                SetInitialEquipment();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// ゲーム初回起動時の初期装備（通常の剣）をセット
        /// </summary>
        private void SetInitialEquipment()
        {
            WeaponParam defaultParam = new WeaponParam(10f, 1.0f, 5f, false, 0f, ElementType.Base);
            rightWeaponData = new EquippedWeaponData(
                defaultSwordPrefab,
                defaultParam,
                "通常の剣",
                "標準的な切れ味を持つシンプルな剣。"
            );
            leftWeaponData = null;
        }

        public void SetWeapon(int slotIndex, EquippedWeaponData data)
        {
            if (slotIndex == 0)
            {
                leftWeaponData = data;
                Debug.Log($"[PlayerLoadout] 左手に装備: {data.weaponName}");
            }
            else
            {
                rightWeaponData = data;
                Debug.Log($"[PlayerLoadout] 右手に装備: {data.weaponName}");
            }
        }
    }
}