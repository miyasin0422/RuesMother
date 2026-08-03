using UnityEngine;

public class CameraController　: MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float smoothTime = 0.2f;

    Vector3 velocity;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - Player.position;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = Player.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

}
