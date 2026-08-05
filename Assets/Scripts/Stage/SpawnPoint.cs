using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private string spawnId;

    public string SpawnId
    {
        get
        {
            return spawnId;
        }
    }
}