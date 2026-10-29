using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// ビルド版の入力とデータ状態をログへ記録する一時的な調査用。
/// ゲームの操作やデータは変更しない。
/// </summary>
public class BuildDiagnostics : MonoBehaviour
{
    private static BuildDiagnostics instance;

    private void Awake()
    {
        // タイトルへ戻っても調査用オブジェクトが重複しないようにする。
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (instance == this)
        {
            instance = null;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReportAfterInitialization());
    }

    private IEnumerator ReportAfterInitialization()
    {
        // シーン内のStartと、重複オブジェクトの削除が終わるのを待つ。
        yield return null;
        yield return null;

        Report("シーン開始");
    }

    private void Update()
    {
        // ゲーム側が攻撃を中断していても、入力自体が届いたか確認する。
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Report("左クリック検知");
        }

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Report(
                "Esc検知 / P押下=" +
                Keyboard.current.pKey.isPressed
            );
        }
    }

    private void Report(string reason)
    {
        Gene.PlayerLoadout loadout = Gene.PlayerLoadout.instance;

        bool hasLeftWeapon =
            loadout != null &&
            loadout.LeftWeaponData != null &&
            loadout.LeftWeaponData.weaponPrefab != null;

        // 有効なタイトル復帰コンポーネントが存在するか調べる。
        ReturnToTitle[] returnHandlers =
            FindObjectsByType<ReturnToTitle>(FindObjectsSortMode.None);

        int activeReturnHandlers = 0;

        foreach (ReturnToTitle handler in returnHandlers)
        {
            if (handler.isActiveAndEnabled)
            {
                activeReturnHandlers++;
            }
        }

        Debug.Log(
            $"[BuildCheck] {reason}" +
            $" | Scene={SceneManager.GetActiveScene().name}" +
            $" | Loadout存在={loadout != null}" +
            $" | 左武器あり={hasLeftWeapon}" +
            $" | 復帰処理数={activeReturnHandlers}" +
            $" | ルー覚醒済み={TutorialProgress.RueAwakened}" +
            $" | 操作禁止={PlayerActionBlocker.IsBlocked}" +
            $" | UI上={PlayerActionBlocker.IsPointerOverUI()}" +
            $" | TimeScale={Time.timeScale}"
        );
    }
}