using UnityEngine;

public class TutorialRaySpawner : MonoBehaviour
{
    [SerializeField] GameObject rayPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] CameraController cameraController;

    // Stage1・2用
    public GameObject SpawnRay()
    {
        GameObject ray = Instantiate(
            rayPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        cameraController.SetPlayer(ray.transform);

        return ray;
    }

    // Stage3以降の会話用
    public GameObject SpawnRay(Transform targetSpawnPoint)
    {
        GameObject ray = Instantiate(
            rayPrefab,
            targetSpawnPoint.position,
            targetSpawnPoint.rotation
        );

        return ray;
    }
}