using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gene;

public class ReStart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReStartFromDie()
    {
        PlayerStatus.playerHealth = PlayerStatus.MaxplayerHealth;
        PlayerStatus.refreshItemStock = 3;
        SceneMoveData.NextSpawnId = "Left";
        SceneManager.LoadScene(SaveManager.savedSceneName);
    }

    public void StartGame()
    {
        PlayerStatus.playerHealth = PlayerStatus.MaxplayerHealth;
        PlayerStatus.refreshItemStock = 2;
        Gene.PlayerLoadout.instance.SetInitialEquipment();
        Inventory.instance.GeneInventoryDictionary.Clear();
        SceneMoveData.NextSpawnId = "Start";
        SceneManager.LoadScene("Title");
    }
}
