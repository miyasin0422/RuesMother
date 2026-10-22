using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public Dictionary<string, int> ItemInventoryDictionary = new Dictionary<string, int>();
    public Dictionary<GeneSO, int> GeneInventoryDictionary = new Dictionary<GeneSO, int>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ItemInventoryDictionary["wing"] = 0;
        ItemInventoryDictionary["block"] = 0;
        ItemInventoryDictionary["iron"] = 0;
        ItemInventoryDictionary["stone"] = 0;
    }

    void Update()
    {

    }

    public void AddGene(GeneSO gene, int count = 1)
    {
        if (gene == null) return;

        if (GeneInventoryDictionary.ContainsKey(gene))
        {
            GeneInventoryDictionary[gene] += count;
        }
        else
        {
            GeneInventoryDictionary[gene] = count;
        }
    }

    public bool ConsumeGene(GeneSO gene, int count = 1)
    {
        if (gene == null) return false;

        if (GeneInventoryDictionary.TryGetValue(gene, out int currentCount))
        {
            if (currentCount >= count)
            {
                GeneInventoryDictionary[gene] -= count;
                return true;
            }
        }
        return false; // 個数が足りない
    }

    
    public int GetGeneCount(GeneSO gene)
    {
        if (gene == null) return 0;

        if (GeneInventoryDictionary.TryGetValue(gene, out int count))
        {
            return count;
        }
        return 0; // 辞書に未登録なら0個
    }
}