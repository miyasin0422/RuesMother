using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("カメラに表示してよい範囲")]
    [SerializeField] Transform boundsMin; // 左下
    [SerializeField] Transform boundsMax; // 右上

    Camera viewCamera;
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
    void Awake()
    {
        viewCamera = GetComponent<Camera>();
    }
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

        if (Player == null)
        {
            return;
        }

        velocity = Vector3.zero;

        transform.position = ClampCameraPosition(
            Player.position + offset
        );
    }

    public void SetPlayerSmooth(Transform newPlayer)
    {
        Player = newPlayer;
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

        if (CameraUp.IsPressed())
        {
            cameraY += cameraUpOffset;
        }
        else if (CameraDown.IsPressed())
        {
            cameraY += cameraDownOffset;
        }

        Vector3 targetPosition = new Vector3(
            followX ? Player.position.x + offset.x : fixedX,
            Player.position.y + cameraY,
            offset.z
        );

        targetPosition = ClampCameraPosition(targetPosition);

        Vector3 nextPosition = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // 追従途中の位置も制限する
        Vector3 clampedPosition = ClampCameraPosition(nextPosition);

        if (clampedPosition.x != nextPosition.x)
        {
            velocity.x = 0f;
        }

        if (clampedPosition.y != nextPosition.y)
        {
            velocity.y = 0f;
        }

        transform.position = clampedPosition;
    }
    Vector3 ClampCameraPosition(Vector3 position)
    {
        if (
            boundsMin == null ||
            boundsMax == null ||
            viewCamera == null ||
            !viewCamera.orthographic)
        {
            return position;
        }

        float halfHeight = viewCamera.orthographicSize;
        float halfWidth = halfHeight * viewCamera.aspect;

        float left = Mathf.Min(boundsMin.position.x, boundsMax.position.x);
        float right = Mathf.Max(boundsMin.position.x, boundsMax.position.x);
        float bottom = Mathf.Min(boundsMin.position.y, boundsMax.position.y);
        float top = Mathf.Max(boundsMin.position.y, boundsMax.position.y);

        // 背景の端から、画面の半分だけ内側に中心を制限する
        position.x = right - left >= halfWidth * 2f
            ? Mathf.Clamp(position.x, left + halfWidth, right - halfWidth)
            : (left + right) * 0.5f;

        position.y = top - bottom >= halfHeight * 2f
            ? Mathf.Clamp(position.y, bottom + halfHeight, top - halfHeight)
            : (bottom + top) * 0.5f;

        return position;
    }
}