using UnityEngine;

public class SaveRefresh : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RefreshClick()
    {
        PlayerStatus.playerHealth = PlayerStatus.MaxplayerHealth;
        PlayerStatus.refreshItemStock = 3;
        uiManager.HPUpdate();
        uiManager.RefreshItemUpdate();
    }
}
