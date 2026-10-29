using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    private bool isStarting;

    void Awake()
    {
        PlayerActionBlocker.ResetForSceneChange();
    }

    public void PlayGame()
    {
        if (isStarting)
        {
            return;
        }

        isStarting = true;
        StartCoroutine(StartNewGame());
    }

    private IEnumerator StartNewGame()
    {
        // 前回プレイの、シーンをまたいで残るデータを削除
        var sessionObjects = new HashSet<GameObject>();

        if (Inventory.instance != null)
        {
            sessionObjects.Add(Inventory.instance.gameObject);
        }

        if (EnemyDictionary.instance != null)
        {
            sessionObjects.Add(EnemyDictionary.instance.gameObject);
        }

        if (Gene.PlayerLoadout.instance != null)
        {
            sessionObjects.Add(Gene.PlayerLoadout.instance.gameObject);
        }

        foreach (GameObject sessionObject in sessionObjects)
        {
            Destroy(sessionObject);
        }
        // 覚醒状態と、各チュートリアルの完了フラグをまとめて初期化する。
        TutorialProgress.ResetAll();

        // 削除が完了してから、次のシーンへ進む
        yield return null;

        TutorialProgress.RueAwakened = false;
        SceneMoveData.NextSpawnId = "Start";

        PlayerStatus.MaxplayerHealth = 100;
        PlayerStatus.playerHealth = 100;
        PlayerStatus.refreshItemStock = 2;

        SaveManager.savedSceneName = "Stage1-1";

        PlayerActionBlocker.ResetForSceneChange();

        SceneManager.LoadScene("Stage1-0");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}