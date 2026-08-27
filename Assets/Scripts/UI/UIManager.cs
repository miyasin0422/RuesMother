using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image[] refreshItems;
    [SerializeField] private float targetFillAmount;
    [SerializeField] private GameObject CustomPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPUpdate();
        RefreshItemUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HPUpdate()
    {
        targetFillAmount = (float)PlayerStatus.playerHealth / PlayerStatus.MaxplayerHealth;
        hpBar.fillAmount = targetFillAmount;
        Debug.Log(targetFillAmount);
    }
    public void RefreshItemUpdate()
    {
        for(int i = 0; i < 3; i++)
        {
            if(i < PlayerStatus.refreshItemStock)
            {
                refreshItems[i].gameObject.SetActive(true);
            }
            else
            {
                refreshItems[i].gameObject.SetActive(false);
            }
            
        }
    }
    public void OpenCustomPanel()
    {
        Debug.Log("click");
        CustomPanel.SetActive(true);
    }
    public void CloseCustomPanel()
    {
        CustomPanel.SetActive(false);
    }

}
