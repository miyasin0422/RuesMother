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
        string text_i = "";
        string text_s = "";
        foreach (var item in Inventory.instance.ItemInventoryDictionary)
        {
            text_i += item.Key + "\n";
            text_s += item.Value + "\n";
        }
        inventoryText.text = text_i;
        stockText.text = text_s;
    }
}
