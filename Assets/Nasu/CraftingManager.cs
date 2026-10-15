using System.Collections.Generic;
using System.Linq;
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

        [Header("レシピデータベース")]
        [SerializeField] private List<Gene.WeaponRecipeSO> recipeDatabase;

        [Header("テスト・デバッグ用")]
        [SerializeField] private HoldButton holdButton;

        // 現在選択中の武器が元々保持していた遺伝子リスト（差分計算用）
        private List<GeneSO> originalWeaponGenes = new List<GeneSO>();

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
        /// 選択されたスロット（左手/右手）の装備データから遺伝子構成を完全に復元して表示
        /// </summary>
        private void LoadEquippedWeaponToSlots(int slotId)
        {
            // 1. スロットのクリア
            foreach (var slot in geneSlots)
            {
                if (slot != null) slot.ClearSlotWithoutRefresh();
            }
            originalWeaponGenes.Clear();

            // 2. PlayerLoadout から該当スロットの装備データを取得
            if (PlayerLoadout.instance != null)
            {
                EquippedWeaponData currentData = (slotId == 0)
                    ? PlayerLoadout.instance.LeftWeaponData
                    : PlayerLoadout.instance.RightWeaponData;

                if (currentData != null && currentData.usedGenes != null && currentData.usedGenes.Count > 0)
                {
                    // 元の遺伝子リストを保存（再錬成時の返還計算用）
                    originalWeaponGenes = new List<GeneSO>(currentData.usedGenes);

                    // 順にスロットへアタッチ
                    for (int i = 0; i < currentData.usedGenes.Count && i < geneSlots.Count; i++)
                    {
                        if (geneSlots[i] != null && currentData.usedGenes[i] != null)
                        {
                            geneSlots[i].SetGeneDirectly(currentData.usedGenes[i]);
                        }
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

        /// <summary>
        /// 錬成完了時の処理（差分比較によるインベントリへの消費／返還）
        /// </summary>
        public void OnHoldCraftComplete()
        {
            List<GeneSO> newGenes = GetAllGenesFromSlots(geneSlots);

            if (newGenes.Count == 0)
            {
                Debug.LogWarning("[CraftingManager] スロットに遺伝子がセットされていません！");
                if (holdButton != null) holdButton.ResetHold();
                return;
            }

            EquippedWeaponData craftedWeapon = CraftingCalculator.ProcessCrafting(newGenes, recipeDatabase);

            if (craftedWeapon != null && craftedWeapon.weaponName != "未完成の構造")
            {
                Debug.Log($"<color=green>【錬成完了】{craftedWeapon.weaponName} を錬成しました！</color>");

                // A. インベントリの消費・返還（差分計算）処理
                ProcessGeneInventoryDifference(originalWeaponGenes, newGenes);

                // B. Loadout への書き換え保存
                if (PlayerLoadout.instance != null)
                {
                    PlayerLoadout.instance.SetWeapon(EquipmentCard.SelectedSlotId, craftedWeapon);
                }

                // C. スロットの初期化と画面更新
                foreach (var slot in geneSlots)
                {
                    if (slot != null) slot.ClearSlot();
                }

                if (createUI != null) createUI.RefreshGeneList();

                // 次回操作用に現在の構成を更新
                originalWeaponGenes = new List<GeneSO>(newGenes);
            }
            else
            {
                Debug.LogWarning("<color=red>【錬成失敗】未完成の組み合わせです。</color>");
            }

            if (holdButton != null) holdButton.ResetHold();
            UpdateCraftPreview();
        }

        /// <summary>
        /// 元の遺伝子構成と新しい遺伝子構成を比較し、差分をインベントリに返還／消費する
        /// </summary>
        private void ProcessGeneInventoryDifference(List<GeneSO> oldList, List<GeneSO> newList)
        {
            if (Inventory.instance == null) return;

            // 各遺伝子の個数をカウント
            var oldCounts = oldList.GroupBy(g => g).ToDictionary(g => g.Key, g => g.Count());
            var newCounts = newList.GroupBy(g => g).ToDictionary(g => g.Key, g => g.Count());

            // すべてのユニークな遺伝子種別を取得
            HashSet<GeneSO> allGeneTypes = new HashSet<GeneSO>(oldCounts.Keys.Concat(newCounts.Keys));

            foreach (var gene in allGeneTypes)
            {
                int oldCount = oldCounts.TryGetValue(gene, out int oCount) ? oCount : 0;
                int newCount = newCounts.TryGetValue(gene, out int nCount) ? nCount : 0;

                int diff = newCount - oldCount;

                if (diff > 0)
                {
                    // 新たに追加・使用された分をインベントリから消費
                    Inventory.instance.ConsumeGene(gene, diff);
                    Debug.Log($"[Inventory] {gene.geneName} を {diff} 個消費しました。");
                }
                else if (diff < 0)
                {
                    // 取り外されて余った分をインベントリへ返還（個数増加）
                    int refundAmount = Mathf.Abs(diff);
                    Inventory.instance.AddGene(gene, refundAmount);
                    Debug.Log($"<color=yellow>[Inventory] {gene.geneName} が {refundAmount} 個インベントリに返還されました！</color>");
                }
            }
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