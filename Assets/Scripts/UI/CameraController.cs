using UnityEngine;

public class CameraController　: MonoBehaviour
{
    Transform Player;
    [SerializeField] float smoothTime = 0.2f;
    [SerializeField] Vector3 offset = new Vector3(0f, 1f, -10f);

    Vector3 velocity;

    public void SetPlayer(Transform newPlayer)
    {
        Player = newPlayer;
        transform.position = Player.position + offset;
    }

    void LateUpdate()
    {
        if (Player == null)
        {
            return;
        }
        Vector3 targetPosition = Player.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

}
