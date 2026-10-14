using System.Collections.Generic;
using UnityEngine;

namespace Gene
{
    public class CraftingManager : MonoBehaviour
    {
        [Header("UI & スロット参照")]
        [SerializeField] private CreateUI createUI;
        [SerializeField] private List<GeneDropSlot> geneSlots;     

        [Header("レシピデータベース")]
        [SerializeField] private List<Gene.WeaponRecipeSO> recipeDatabase; // 作成した WeaponRecipeSO 群をアタッチ

        [Header("テスト・デバッグ用")]
        [SerializeField] private HoldButton holdButton;

        /// <summary>
        /// HoldButton の長押し完了イベント（OnHoldComplete）から呼び出される処理
        /// </summary>
        public void OnHoldCraftComplete()
        {
            // 1. スロットから配置されている Base遺伝子を抽出
            List<GeneSO> baseGenes = GetBaseGenesFromSlots(geneSlots);

            if (baseGenes.Count == 0)
            {
                Debug.LogWarning("[CraftingManager] Baseスロットに遺伝子がセットされていません！");
                if (holdButton != null) holdButton.ResetHold();
                return;
            }

            // 2. CraftingCalculator を使ってレシピ照合＆武器Prefabの特定
            GameObject selectedWeaponPrefab = CraftingCalculator.DetermineWeaponPrefab(baseGenes, recipeDatabase);

            if (selectedWeaponPrefab != null)
            {
                Debug.Log($"<color=green>【錬成成功】選択された武器: {selectedWeaponPrefab.name}</color>");

                // ※ 次のステップで以下を実装します：
                // 3. 変異因子を含めた WeaponParam の計算
                // 4. PlayerLoadout.Instance.SetEquippedWeapon(...) への反映
                // 5. Inventory からの正式消費と createUI.RefreshGeneList()
            }
            else
            {
                Debug.LogWarning("<color=red>【錬成失敗】該当するレシピが存在しません。</color>");
            }

            // 長押しゲージをリセット
            if (holdButton != null)
            {
                holdButton.ResetHold();
            }
        }

        /// <summary>
        /// 指定されたスロットリストから現在セットされている GeneSO を抽出する
        /// </summary>
        private List<GeneSO> GetBaseGenesFromSlots(List<GeneDropSlot> slots)
        {
            List<GeneSO> genes = new List<GeneSO>();
            foreach (var slot in slots)
            {
                if (slot != null && slot.CurrentGene != null)
                {
                    if(slot.CurrentGene.element == ElementType.Base)
                    {
                        genes.Add(slot.CurrentGene);
                    }
                }
            }
            return genes;
        }
    }
}