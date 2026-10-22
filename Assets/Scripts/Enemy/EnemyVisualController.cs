using UnityEngine;

public class EnemyVisualController : MonoBehaviour
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

    [SerializeField] EnemyController enemyController;
    [SerializeField] Rigidbody2D enemyRigidbody;
    [SerializeField] SpriteRenderer bodyRenderer;

    [Header("画像ごとの設定")]
    [SerializeField] PoseSettings idle = new PoseSettings();
    [SerializeField] PoseSettings move = new PoseSettings();
    [SerializeField] PoseSettings attack = new PoseSettings();

    [Header("移動判定")]
    [SerializeField] float movementThreshold = 0.1f;

    Vector3 basePosition;
    Vector3 baseScale;

    float attackPoseEndTime;

    public void ShowAttack(float duration)
    {
        attackPoseEndTime = Time.time + Mathf.Max(duration, 0f);
    }

    void Awake()
    {
        if (enemyController == null)
        {
            enemyController = GetComponent<EnemyController>();
        }

        if (enemyRigidbody == null)
        {
            enemyRigidbody = GetComponent<Rigidbody2D>();
        }

        if (bodyRenderer != null)
        {
            basePosition = bodyRenderer.transform.localPosition;
            baseScale = bodyRenderer.transform.localScale;
        }
    }

    void LateUpdate()
    {
        if (bodyRenderer == null)
        {
            return;
        }

        PoseSettings pose = idle;

        if (enemyController != null && enemyController.enabled)
        {
            if (Time.time < attackPoseEndTime)
            {
                pose = attack;
            }
            else if (enemyController.IsAttacking)
            {
                // 射程内での予備動作・待機中は静止画像
                pose = idle;
            }
            else if (
                enemyRigidbody != null &&
                enemyRigidbody.linearVelocity.sqrMagnitude >
                movementThreshold * movementThreshold)
            {
                pose = move;
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