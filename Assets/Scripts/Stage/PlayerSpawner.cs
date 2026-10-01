using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private bool spawnOnStart = true;

    private void Start()
    {
        if (!spawnOnStart)
        {
            return;
        }

        SpawnPlayer();
    }

    public GameObject SpawnPlayer()
    {
        SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(
            FindObjectsSortMode.None
        );

        SpawnPoint selectedSpawnPoint = null;

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnId == SceneMoveData.NextSpawnId)
            {
                selectedSpawnPoint = spawnPoint;
                break;
            }
        }

        if (selectedSpawnPoint == null)
        {
            Debug.LogError(
                "SpawnPointが見つかりません：" +
                SceneMoveData.NextSpawnId
            );

            return null;
        }

        GameObject player = Instantiate(
            playerPrefab,
            selectedSpawnPoint.transform.position,
            selectedSpawnPoint.transform.rotation
        );

        cameraController.SetPlayer(player.transform);

        return player;
    }
}