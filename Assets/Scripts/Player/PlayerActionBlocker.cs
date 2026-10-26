using System.Collections.Generic;
using UnityEngine;

public class PlayerActionBlocker : MonoBehaviour
{
    [Header("表示中にゲームを一時停止する")]
    [SerializeField] private bool pauseGame = false;

    private static readonly HashSet<PlayerActionBlocker> pauseBlockers =
        new HashSet<PlayerActionBlocker>();

    private static float previousTimeScale = 1f;
    private static readonly HashSet<PlayerActionBlocker> blockers =
        new HashSet<PlayerActionBlocker>();

    private static int releasedFrame = -1;

    // 画面を閉じたフレームも、操作禁止を維持する
    public static bool IsBlocked =>
        blockers.Count > 0 ||
        releasedFrame == Time.frameCount;

    [RuntimeInitializeOnLoadMethod(
    RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        blockers.Clear();
        pauseBlockers.Clear();

        releasedFrame = -1;
        previousTimeScale = 1f;
        Time.timeScale = 1f;
    }

    void OnEnable()
    {
        blockers.Add(this);

        if (pauseGame && pauseBlockers.Add(this))
        {
            // 最初の一時停止画面を開いたときだけ時間を保存
            if (pauseBlockers.Count == 1)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
        }
    }

    void OnDisable()
    {
        blockers.Remove(this);
        releasedFrame = Time.frameCount;

        // 最後の一時停止画面を閉じたら再開
        if (pauseBlockers.Remove(this) && pauseBlockers.Count == 0)
        {
            Time.timeScale = previousTimeScale;
        }
    }

    // UIを開く最初のクリックも、攻撃に使わせない
    public static bool IsPointerOverUI()
    {
        var eventSystem =
            UnityEngine.EventSystems.EventSystem.current;

        var mouse = UnityEngine.InputSystem.Mouse.current;

        if (eventSystem == null || mouse == null)
        {
            return false;
        }

        var pointer =
            new UnityEngine.EventSystems.PointerEventData(eventSystem);

        pointer.position = mouse.position.ReadValue();

        var results =
            new List<UnityEngine.EventSystems.RaycastResult>();

        eventSystem.RaycastAll(pointer, results);

        foreach (var result in results)
        {
            if (result.module is UnityEngine.UI.GraphicRaycaster)
            {
                return true;
            }
        }

        return false;
    }
}