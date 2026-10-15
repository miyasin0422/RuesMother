using UnityEngine;
using System.Collections.Generic;

public class CreateUI : MonoBehaviour
{
    [SerializeField] private Transform dragLayer;
    [SerializeField] private Transform baseGeneContent;
    [SerializeField] private Transform mutationGeneContent;
    [SerializeField] private List<GeneDropSlot> allSlots;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    private void OnEnable()
    {
        RefreshGeneList();
    }

    public void RefreshGeneList()
    {
        ClearButtons();
        if (Inventory.instance == null) return;

        Dictionary<GeneSO, int> stagedCounts = GetStagedGeneCounts();

        // 辞書に登録されている（＝一度でも獲得したことのある）全遺伝子をループ
        foreach (KeyValuePair<GeneSO, int> pair in Inventory.instance.GeneInventoryDictionary)
        {
            GeneSO gene = pair.Key;
            int actualCount = pair.Value; // インベントリの実所持数（0の可能性あり）

            if (gene == null) continue; // ★ actualCount <= 0 の条件を削除！

            Transform targetParent = (gene.element == ElementType.Base)
                ? baseGeneContent
                : mutationGeneContent;

            if (targetParent == null) continue;

            GameObject newButton = Instantiate(gene.buttonPrefab, targetParent);
            spawnedButtons.Add(newButton);

            if (newButton.TryGetComponent<GeneDrag>(out var geneDrag))
            {
                stagedCounts.TryGetValue(gene, out int stagedCount);

                // 計算後の実効所持数を算出
                int remaining = actualCount - stagedCount;
                int displayRemaining = Mathf.Max(0, remaining);

                string displayText = FormatCountText(actualCount, stagedCount);

                // ★ geneDrag に「現在のドラッグ可能残数（displayRemaining）」も渡してセットアップ
                geneDrag.Setup(gene, displayRemaining, displayText, dragLayer);
            }
        }
    }

    private string FormatCountText(int actualCount, int stagedCount)
    {
        int remaining = actualCount - stagedCount;
        int displayRemaining = Mathf.Max(0, remaining);

        if (stagedCount > 0)
        {
            // 例: 「x0 → 1」や「x3 → 2」のように変化を表示
            return $"x{actualCount} → {displayRemaining}";
        }
        else
        {
            return $"x{actualCount}";
        }
    }

    private Dictionary<GeneSO, int> GetStagedGeneCounts()
    {
        Dictionary<GeneSO, int> counts = new Dictionary<GeneSO, int>();
        foreach (var slot in allSlots)
        {
            if (slot != null && slot.CurrentGene != null)
            {
                if (counts.ContainsKey(slot.CurrentGene))
                {
                    counts[slot.CurrentGene]++;
                }
                else
                {
                    counts[slot.CurrentGene] = 1;
                }
            }
        }
        return counts;
    }

    private void ClearButtons()
    {
        foreach (var btn in spawnedButtons)
        {
            if (btn != null) Destroy(btn);
        }
        spawnedButtons.Clear();
    }
}