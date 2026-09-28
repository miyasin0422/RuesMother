using UnityEngine;

public class TutorialRaySpawner : MonoBehaviour
{
    [SerializeField] GameObject rayPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] private CameraController cameraController;

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
}