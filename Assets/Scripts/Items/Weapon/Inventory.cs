using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public Dictionary<string, int> ItemInventoryDictionary = new Dictionary<string, int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        ItemInventoryDictionary["wing"] = 5;
        ItemInventoryDictionary["block"] = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
