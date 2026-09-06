using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    Transform Player;

    [SerializeField] InputAction CameraUp;
    [SerializeField] InputAction CameraDown;

    [SerializeField] float smoothTime = 0.2f;

    // 通常時のカメラ位置
    [SerializeField] Vector3 offset = new Vector3(0f, 1f, -10f);

    // 上下を見るときの追加オフセット
    [SerializeField] float cameraUpOffset = 3f;
    [SerializeField] float cameraDownOffset = -2f;

    Vector3 velocity;

    bool followX = true;
    float fixedX;

    private void OnEnable()
    {
        CameraUp.Enable();
        CameraDown.Enable();
    }

    private void OnDisable()
    {
        CameraUp.Disable();
        CameraDown.Disable();
    }

    public void SetPlayer(Transform newPlayer)
    {
        Player = newPlayer;
        transform.position = Player.position + offset;
    }

    // 通常追従に戻す
    public void StartFollowX()
    {
        followX = true;
    }

    // 指定したX座標に固定
    public void FixX(float x)
    {
        fixedX = x;
        followX = false;
        velocity.x = 0f;
    }

    void LateUpdate()
    {
        if (Player == null)
        {
            return;
        }

        float cameraY = offset.y;

        // 上を見る
        if (CameraUp.IsPressed())
        {
            cameraY += cameraUpOffset;
        }
        // 下を見る
        else if (CameraDown.IsPressed())
        {
            cameraY += cameraDownOffset;
        }

        Vector3 targetPosition = new Vector3(
            followX
                ? Player.position.x + offset.x
                : fixedX,

            Player.position.y + cameraY,

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