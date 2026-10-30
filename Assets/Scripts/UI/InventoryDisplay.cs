using TMPro;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public TMP_Text inventoryText;
    public TMP_Text stockText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        UpdateInventoryText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void UpdateInventoryText()
    {
        string names = "";
        string counts = "";

        if (Inventory.instance != null)
        {
            foreach (var item in Inventory.instance.GeneInventoryDictionary)
            {
                GeneSO gene = item.Key;

                if (gene == null)
                {
                    continue;
                }

                names += gene.geneName + "の遺伝子\n";
                counts += item.Value + "\n";
            }
        }

        inventoryText.text = names;
        stockText.text = counts;
    }
}
