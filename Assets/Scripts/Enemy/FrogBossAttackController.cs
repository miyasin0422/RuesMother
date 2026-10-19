using System.Collections;
using UnityEngine;

public class FrogBossAttackController : MonoBehaviour
{
    // 舌
    [SerializeField] FrogTongueAttack tonguePrefab;
    [SerializeField] Transform tonguePoint;
    [SerializeField] int tonguePower = 1;

    [SerializeField] float tongueDelay = 0.5f;
    [SerializeField] float tongueRecovery = 0.7f;

    // 踏みつけ
    [SerializeField] EnemyAttack stompAttackPrefab;
    [SerializeField] Transform stompPoint;
    [SerializeField] int stompPower = 2;

    // 衝撃波
    [SerializeField] FrogShockWave shockWavePrefab;
    [SerializeField] Transform shockWavePoint;
    [SerializeField] int shockWavePower = 1;

    [SerializeField] float stompRecovery = 1f;
    [Header("攻撃画像の表示時間")]
    [SerializeField] float tongueSpriteDuration = 0.3f;
    [SerializeField] float stompSpriteDuration = 0.4f;

    EnemyVisualController enemyVisual;

    void Awake()
    {
        enemyVisual =
            GetComponentInChildren<EnemyVisualController>(true);

        if (enemyVisual == null)
        {
            Debug.LogWarning(
                "ボスにEnemyVisualControllerが設定されていません。",
                this
            );
        }
    }

    public bool IsAttacking { get; private set; }

    public void TongueAttack(Transform target)
    {
        if (IsAttacking || target == null)
        {
            return;
        }

        StartCoroutine(TongueAttackCoroutine(target));
    }

    IEnumerator TongueAttackCoroutine(Transform target)
    {
        IsAttacking = true;

        Debug.Log("舌攻撃予備動作");

        yield return new WaitForSeconds(tongueDelay);

        if (target != null)
        {
            if (enemyVisual != null)
            {
                enemyVisual.ShowAttack(tongueSpriteDuration);
            }

            FrogTongueAttack attack =
                        Instantiate(
                    tonguePrefab,
                    tonguePoint.position,
                    tonguePoint.rotation
                );

            attack.Initialize(
                tonguePower,
                target.position
            );
        }

        yield return new WaitForSeconds(tongueRecovery);

        IsAttacking = false;
    }

    public void StompImpact()
    {
        if (IsAttacking)
        {
            return;
        }

        StartCoroutine(StompCoroutine());
    }

    IEnumerator StompCoroutine()
    {
        IsAttacking = true;

        Debug.Log("踏みつけ");
        if (enemyVisual != null)
        {
            enemyVisual.ShowAttack(stompSpriteDuration);
        }

        // 着地点そのもの
        if (stompAttackPrefab != null)
        {
            EnemyAttack stomp =
                Instantiate(
                    stompAttackPrefab,
                    stompPoint.position,
                    stompPoint.rotation
                );

            stomp.Initialize(
                stompPower,
                stompPoint.position
            );
        }

        // 左
        FrogShockWave left =
            Instantiate(
                shockWavePrefab,
                shockWavePoint.position,
                Quaternion.identity
            );

        left.Initialize(shockWavePower, -1);

        // 右
        FrogShockWave right =
            Instantiate(
                shockWavePrefab,
                shockWavePoint.position,
                Quaternion.identity
            );

        right.Initialize(shockWavePower, 1);

        yield return new WaitForSeconds(stompRecovery);

        IsAttacking = false;
    }
}