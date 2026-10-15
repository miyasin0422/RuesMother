using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Gene
{
    public class CraftingManager : MonoBehaviour
    {
        [Header("UI & スロット参照")]
        [SerializeField] private CreateUI createUI;
        [SerializeField] private List<GeneDropSlot> geneSlots;
        [SerializeField] private TMP_Text weaponNameText;
        [SerializeField] private TMP_Text weaponDescriptionText;

        [Header("初期化用遺伝子参照")]
        [SerializeField] private GeneSO defaultSwordGene;
        [SerializeField] private GeneSO defaultBowGene;

        [Header("レシピデータベース")]
        [SerializeField] private List<Gene.WeaponRecipeSO> recipeDatabase;

        [Header("テスト・デバッグ用")]
        [SerializeField] private HoldButton holdButton;

        private void OnEnable()
        {
            EquipmentCard.OnCardSelected += HandleCardSelected;
        }

        private void OnDisable()
        {
            EquipmentCard.OnCardSelected -= HandleCardSelected;
        }

        private void Start()
        {
            LoadEquippedWeaponToSlots(EquipmentCard.SelectedSlotId);
        }

        private void HandleCardSelected(int slotId)
        {
            LoadEquippedWeaponToSlots(slotId);
        }

        /// <summary>
        /// 現在装備中の武器に応じた遺伝子をスロットに初期配置
        /// </summary>
        private void LoadEquippedWeaponToSlots(int slotId)
        {
            foreach (var slot in geneSlots)
            {
                if (slot != null) slot.ClearSlotWithoutRefresh();
            }

            if (PlayerLoadout.instance != null)
            {
                EquippedWeaponData currentData = (slotId == 0)
                    ? PlayerLoadout.instance.LeftWeaponData
                    : PlayerLoadout.instance.RightWeaponData;

                if (currentData != null)
                {
                    if (currentData.weaponName.Contains("剣") && defaultSwordGene != null)
                    {
                        if (geneSlots.Count > 0) geneSlots[0].SetGeneDirectly(defaultSwordGene);
                    }
                    else if (currentData.weaponName.Contains("弓") && defaultBowGene != null)
                    {
                        if (geneSlots.Count > 0) geneSlots[0].SetGeneDirectly(defaultBowGene);
                    }
                }
            }

            if (createUI != null) createUI.RefreshGeneList();
            UpdateCraftPreview();
        }

        /// <summary>
        /// プレビュー表示の更新
        /// </summary>
        public void UpdateCraftPreview()
        {
            List<GeneSO> currentGenes = GetAllGenesFromSlots(geneSlots);

            if (currentGenes.Count == 0)
            {
                if (weaponNameText != null) weaponNameText.text = "---";
                if (weaponDescriptionText != null) weaponDescriptionText.text = "遺伝子スロットに素材を配置してください。";
                return;
            }

            EquippedWeaponData previewData = CraftingCalculator.ProcessCrafting(currentGenes, recipeDatabase);

            if (previewData != null)
            {
                if (weaponNameText != null) weaponNameText.text = previewData.weaponName;
                if (weaponDescriptionText != null) weaponDescriptionText.text = previewData.description;
            }
        }

        public void OnHoldCraftComplete()
        {
            List<GeneSO> allGenes = GetAllGenesFromSlots(geneSlots);

            if (allGenes.Count == 0)
            {
                Debug.LogWarning("[CraftingManager] スロットに遺伝子がセットされていません！");
                if (holdButton != null) holdButton.ResetHold();
                return;
            }

            EquippedWeaponData craftedWeapon = CraftingCalculator.ProcessCrafting(allGenes, recipeDatabase);

            if (craftedWeapon != null && craftedWeapon.weaponName != "未完成の構造")
            {
                Debug.Log($"<color=green>【錬成完了】{craftedWeapon.weaponName} を錬成しました！</color>");

                if (PlayerLoadout.instance != null)
                {
                    PlayerLoadout.instance.SetWeapon(EquipmentCard.SelectedSlotId, craftedWeapon);
                }

                if (Inventory.instance != null)
                {
                    foreach (var gene in allGenes)
                    {
                        Inventory.instance.ConsumeGene(gene, 1);
                    }
                }

                foreach (var slot in geneSlots)
                {
                    if (slot != null) slot.ClearSlot();
                }

                if (createUI != null) createUI.RefreshGeneList();
            }
            else
            {
                Debug.LogWarning("<color=red>【錬成失敗】未完成の組み合わせです。</color>");
            }

            if (holdButton != null) holdButton.ResetHold();
            UpdateCraftPreview();
        }

        private List<GeneSO> GetAllGenesFromSlots(List<GeneDropSlot> slots)
        {
            List<GeneSO> genes = new List<GeneSO>();
            foreach (var slot in slots)
            {
                if (slot != null && slot.CurrentGene != null) genes.Add(slot.CurrentGene);
            }
            return genes;
        }
    }
}