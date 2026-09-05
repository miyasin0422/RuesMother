using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform Player;

    [SerializeField] float smoothTime = 0.2f;
    [SerializeField] Vector3 offset = new Vector3(0f, 1f, -10f);

    Vector3 velocity;

    bool followX = true;
    bool followY = true;

    public void SetPlayer(Transform newPlayer)
    {
        Player = newPlayer;
        transform.position = Player.position + offset;
    }

    public void SetFollowX(bool follow)
    {
        followX = follow;

        // X追従を止める瞬間の位置で固定
        velocity.x = 0f;
    }

    public void SetFollowY(bool follow)
    {
        followY = follow;

        velocity.y = 0f;
    }

    void LateUpdate()
    {
        if (Player == null)
        {
            return;
        }

        Vector3 targetPosition = new Vector3(
            followX
                ? Player.position.x + offset.x
                : transform.position.x,

            followY
                ? Player.position.y + offset.y
                : transform.position.y,

            offset.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}