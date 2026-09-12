using UnityEngine;

public class PanelOpen : MonoBehaviour
{
    public GameObject targetPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenPanel()
    {
        targetPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        targetPanel.SetActive(false);
    }
}
