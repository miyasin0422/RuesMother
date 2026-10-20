using UnityEngine;
using System.Collections.Generic;
using Gene;

public class CreateUI : MonoBehaviour
{
    [SerializeField] private Transform dragLayer;
    [SerializeField] private Transform baseGeneContent;
    [SerializeField] private Transform mutationGeneContent;
    [SerializeField] private List<GeneDropSlot> allSlots;
    private List<GameObject> spawnedButtons = new List<GameObject>();
    public bool CanClickHoldButton = false;

    private void OnEnable()
    {
        RefreshGeneList();
    }

    public void RefreshGeneList()
    {
        ClearButtons();
        if (Inventory.instance == null) return;

        Dictionary<GeneSO, int> stagedCounts = GetStagedGeneCounts();
        Dictionary<GeneSO, int> allGeneList = AllGeneList();
        Dictionary<GeneSO, int> oppositeGeneList = NoSelectedSlotGeneList();

        bool hasAnyChange = false;

        // 辞書に登録されている（＝一度でも獲得したことのある）全遺伝子をループ
        foreach (KeyValuePair<GeneSO, int> pair in Inventory.instance.GeneInventoryDictionary)
        {
            GeneSO gene = pair.Key;
            int actualCount = pair.Value; // インベントリの実所持数（0の可能性あり）

            if (gene == null) continue;

            Transform targetParent = (gene.element == ElementType.Base)
                ? baseGeneContent
                : mutationGeneContent;

            if (targetParent == null) continue;

            GameObject newButton = Instantiate(gene.buttonPrefab, targetParent);
            spawnedButtons.Add(newButton);

            if (newButton.TryGetComponent<GeneDrag>(out var geneDrag))
            {
                stagedCounts.TryGetValue(gene, out int stagedCount);
                allGeneList.TryGetValue(gene, out int allCount);
                oppositeGeneList.TryGetValue(gene, out int oppositeCount);

                // 計算後の実効所持数を算出
                int remaining = allCount - stagedCount - oppositeCount;
                int displayRemaining = Mathf.Max(0, remaining);

                if (actualCount != displayRemaining)
                {
                    hasAnyChange = true;
                }

                string displayText = FormatCountText(actualCount, displayRemaining);

                geneDrag.Setup(gene, displayRemaining, displayText, dragLayer);
            }
        }

        // 「1つでも素材に変更がある」かつ「スロットにベース遺伝子（核）が1つ以上セットされている」場合のみ長押し可能
        CanClickHoldButton = hasAnyChange && HasBaseGeneInSlots(stagedCounts);
    }

    private string FormatCountText(int actualCount, int displayRemaining)
    {
        if (actualCount != displayRemaining)
        {
            return $"x{actualCount} → {displayRemaining}";
        }
        else
        {
            return $"x{actualCount}";
        }
    }

    private Dictionary<GeneSO, int> AllGeneList()
    {
        Dictionary<GeneSO, int> allGeneList = new Dictionary<GeneSO, int>();

        if (Inventory.instance != null && Inventory.instance.GeneInventoryDictionary != null)
        {
            foreach (KeyValuePair<GeneSO, int> pair in Inventory.instance.GeneInventoryDictionary)
            {
                if (pair.Key != null)
                {
                    allGeneList[pair.Key] = pair.Value;
                }
            }
        }

        // PlayerLoadout および武器データが存在する場合のみ加算処理を行う
        if (Gene.PlayerLoadout.instance != null)
        {
            // 左手データ
            if (Gene.PlayerLoadout.instance.LeftWeaponData != null && Gene.PlayerLoadout.instance.LeftWeaponData.usedGenes != null)
            {
                foreach (GeneSO gene in Gene.PlayerLoadout.instance.LeftWeaponData.usedGenes)
                {
                    if (gene == null) continue;
                    if (allGeneList.ContainsKey(gene)) allGeneList[gene] += 1;
                    else allGeneList[gene] = 1;
                }
            }

            // 右手データ
            if (Gene.PlayerLoadout.instance.RightWeaponData != null && Gene.PlayerLoadout.instance.RightWeaponData.usedGenes != null)
            {
                foreach (GeneSO gene in Gene.PlayerLoadout.instance.RightWeaponData.usedGenes)
                {
                    if (gene == null) continue;
                    if (allGeneList.ContainsKey(gene)) allGeneList[gene] += 1;
                    else allGeneList[gene] = 1;
                }
            }
        }

        return allGeneList;
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

    private Dictionary<GeneSO, int> NoSelectedSlotGeneList()
    {
        Dictionary<GeneSO, int> GeneDic = new Dictionary<GeneSO, int>();

        if (Gene.PlayerLoadout.instance != null)
        {
            // 選択されていない側の装備データを安全に取得
            EquippedWeaponData oppositeData = (EquipmentCard.SelectedSlotId == 0)
                ? Gene.PlayerLoadout.instance.RightWeaponData
                : Gene.PlayerLoadout.instance.LeftWeaponData;

            if (oppositeData != null && oppositeData.usedGenes != null)
            {
                foreach (GeneSO gene in oppositeData.usedGenes)
                {
                    if (gene == null) continue;
                    if (GeneDic.ContainsKey(gene)) GeneDic[gene] += 1;
                    else GeneDic[gene] = 1;
                }
            }
        }

        return GeneDic;
    }

    private bool HasBaseGeneInSlots(Dictionary<GeneSO, int> stagedCounts)
    {
        if (stagedCounts == null) return false;
        foreach (var pair in stagedCounts)
        {
            if (pair.Key != null && pair.Key.element == ElementType.Base)
            {
                return true;
            }
        }
        return false;
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