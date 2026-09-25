using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] UIManager uiManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Damaged(int damage)
    {
        PlayerStatus.playerHealth -= damage;
        uiManager.HPUpdate();
        //Debug.Log("playerHP：" + PlayerStatus.playerHealth);
        if (PlayerStatus.playerHealth <= 0)
        {
            StartCoroutine(ReStart());
        }
    }
    public void Refresh(int hpRefresh)
    {
        if((PlayerStatus.playerHealth + hpRefresh) <= 100)
        {
            PlayerStatus.playerHealth += hpRefresh;
        }
        else
        {
            PlayerStatus.playerHealth = PlayerStatus.MaxplayerHealth;
        }
        uiManager.HPUpdate();
        uiManager.RefreshItemUpdate();
    }
    IEnumerator ReStart()
    {
        yield return new WaitForSeconds(1);
        PlayerStatus.playerHealth = PlayerStatus.MaxplayerHealth;
        if(SaveManager.savedSceneName == "Stage1-1")
        {
            SceneMoveData.NextSpawnId = "Start";
        }
        else
        {
            SceneMoveData.NextSpawnId = "Left";
        }
        SceneManager.LoadScene(SaveManager.savedSceneName);
    }
}