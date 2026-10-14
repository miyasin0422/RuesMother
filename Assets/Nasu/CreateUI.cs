using UnityEngine;
using System.Collections.Generic;

public class CreateUI : MonoBehaviour
{
    //ドラッグ用レイヤー
    [SerializeField] private Transform dragLayer;
    [SerializeField] private Transform baseGeneContent; //ベース遺伝子表示エリア
    [SerializeField] private Transform mutationGeneContent; //変異因子表示エリア
    [SerializeField] private List<GeneDropSlot> allSlots; // スロット
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

        foreach(KeyValuePair<GeneSO, int> pair in Inventory.instance.GeneInventoryDictionary)
        {
            GeneSO gene = pair.Key;
            int actualCount = pair.Value;

            if (gene == null || actualCount <= 0) continue;

            Transform targetParent = (gene.element == ElementType.Base)
                ? baseGeneContent
                : mutationGeneContent;

            if (targetParent == null) continue;

            GameObject newButton = Instantiate(gene.buttonPrefab, targetParent);
            spawnedButtons.Add(newButton);

            if (newButton.TryGetComponent<GeneDrag>(out var geneDrag))
            {
                stagedCounts.TryGetValue(gene, out int stagedCount);

                string displayText = FormatCountText(actualCount, stagedCount);

                geneDrag.Setup(gene, displayText, dragLayer);
            }
        }
    }

    private string FormatCountText(int actualCount, int stagedCount)
    {
        if (stagedCount > 0)
        {
            int remaining = actualCount - stagedCount;
            // 0以下になる場合は 0 と表示
            int displayRemaining = Mathf.Max(0, remaining);
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
