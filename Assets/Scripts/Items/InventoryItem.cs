using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    private string itemName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Inventory.instance.ItemInventoryDictionary[itemName] += 1;
            Destroy(gameObject);
        }
    }
}
