using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;

    [SerializeField]
    private string nextSpawnId;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneMoveData.NextSpawnId = nextSpawnId;
            SceneManager.LoadSceneAsync(nextSceneName);
        }
    }
}