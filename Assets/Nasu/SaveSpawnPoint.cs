using System.Collections;
using UnityEngine;

public class SaveSpawnPoint : MonoBehaviour
{
    [SerializeField] private string SceneName;
    [SerializeField] private GameObject SaveCompletePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SaveClick()
    {
        SaveManager.savedSceneName = SceneName;
        StartCoroutine(SaveDisplay());
    }
    IEnumerator SaveDisplay()
    {
        SaveCompletePanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SaveCompletePanel.SetActive(false);
    }
}
