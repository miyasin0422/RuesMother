using UnityEngine;

public class SaveSpawnPoint : MonoBehaviour
{
    [SerializeField] private string SceneName;

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
    }
}
