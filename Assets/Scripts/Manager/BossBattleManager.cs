using System.Collections;
using UnityEngine;

public class BossBattleManager : MonoBehaviour
{
    [SerializeField] FrogBossAI bossAI;
    [SerializeField] EnemyHealth bossHealth;
    [SerializeField] BossHealthBar bossHealthBar;
    [SerializeField] CameraController cameraController;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;

    void Start()
    {
        StartCoroutine(StartBattleCoroutine());
        leftWall.SetActive(false);
        rightWall.SetActive(false);
    }


    IEnumerator StartBattleCoroutine()
    {
        GameObject player = null;

        while (player == null)
        {
            player =
                GameObject.FindGameObjectWithTag("Player");

            yield return null;
        }

        bossHealth.Died += EndBattle;

        bossHealthBar.gameObject.SetActive(true);

        bossHealthBar.Initialize(bossHealth);

        bossAI.StartBattle(player.transform);

        Debug.Log("ボス戦開始");
        cameraController.SetFollowX(false);
        leftWall.SetActive(true);
        rightWall.SetActive(true);
    }

    void EndBattle()
    {
        Debug.Log("ボス戦終了");
        cameraController.SetFollowX(true);
        leftWall.SetActive(false);
        rightWall.SetActive(false);

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.Died -= EndBattle;
        }
    }
}