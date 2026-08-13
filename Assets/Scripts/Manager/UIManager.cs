using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image[] refreshItems;
    [SerializeField] private float targetFillAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPUpdate();
        RefreshItem();
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
    public void RefreshItem()
    {
        for(int i = 0; i < PlayerStatus.refreshItemStock; i++)
        {
            refreshItems[i].gameObject.SetActive(true);
        }
    }

}
