using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    [System.Serializable]
    public class PoseSettings
    {
        public Sprite sprite;

        [Min(0.01f)]
        public float sizeMultiplier = 1f;

        public Vector2 positionOffset;

        public bool flipX;
    }

    [SerializeField] PlayerControll playerController;
    [SerializeField] SpriteRenderer bodyRenderer;

    [Header("画像ごとの設定")]
    [SerializeField] PoseSettings idle = new PoseSettings();
    [SerializeField] PoseSettings walk = new PoseSettings();
    [SerializeField] PoseSettings dash = new PoseSettings();
    [SerializeField] PoseSettings weaponReady = new PoseSettings();
    [SerializeField] PoseSettings crouch = new PoseSettings();

    Vector3 basePosition;
    Vector3 baseScale;

    void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerControll>();
        }

        if (bodyRenderer != null)
        {
            basePosition = bodyRenderer.transform.localPosition;
            baseScale = bodyRenderer.transform.localScale;
        }
    }

    void LateUpdate()
    {
        if (playerController == null || bodyRenderer == null)
        {
            return;
        }

        PoseSettings pose = idle;

        if (playerController.enabled)
        {
            if (playerController.IsDashing)
            {
                pose = dash;
            }
            else if (playerController.IsCrouching)
            {
                pose = crouch;
            }
            else if (playerController.IsHoldingWeapon)
            {
                pose = weaponReady;
            }
            else if (playerController.IsWalking)
            {
                pose = walk;
            }
        }

        if (pose == null || pose.sprite == null)
        {
            pose = idle;
        }

        if (pose == null || pose.sprite == null)
        {
            return;
        }

        bodyRenderer.sprite = pose.sprite;
        bodyRenderer.flipX = pose.flipX;

        bodyRenderer.transform.localScale =
            baseScale * Mathf.Max(pose.sizeMultiplier, 0.01f);

        bodyRenderer.transform.localPosition =
            basePosition + new Vector3(
                pose.positionOffset.x,
                pose.positionOffset.y,
                0f
            );
    }
}