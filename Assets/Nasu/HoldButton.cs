using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("長押し設定")]
    [SerializeField, Tooltip("確定に必要な長押し時間（秒）")]
    private float holdDuration = 1.0f;

    [Header("UI演出設定")]
    [SerializeField, Tooltip("ゲージとして満ちていく Image (Image Type = Filled に設定)")]
    private Image gaugeImage;

    [Header("イベント")]
    [Tooltip("長押し完了時に実行する処理")]
    public UnityEvent OnHoldComplete;

    private float currentHoldTime = 0f;
    private bool isHolding = false;
    private bool isCompleted = false;

    private void Update()
    {
        if (!isHolding || isCompleted) return;

        currentHoldTime += Time.deltaTime;

        // ゲージの更新 (0.0 ～ 1.0)
        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = Mathf.Clamp01(currentHoldTime / holdDuration);
        }

        // 長押し完了判定
        if (currentHoldTime >= holdDuration)
        {
            isCompleted = true;
            isHolding = false;

            // 完了時の演出（ゲージ満タン保持）
            if (gaugeImage != null) gaugeImage.fillAmount = 1f;

            // 確定処理を実行
            OnHoldComplete?.Invoke();
            if (gaugeImage != null) gaugeImage.fillAmount = 0f;
        }
    }

    // 押した瞬間
    public void OnPointerDown(PointerEventData eventData)
    {
        // 左クリックのみ反応
        if (eventData.button != PointerEventData.InputButton.Left) return;

        ResetHold();
        isHolding = true;
    }

    // 離した瞬間
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!isCompleted)
        {
            ResetHold();
        }
    }

    // ボタンの外にカーソルが出た場合（キャンセル扱い）
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isCompleted)
        {
            ResetHold();
        }
    }

    /// <summary>
    /// 長押し状態とゲージをリセットする
    /// </summary>
    public void ResetHold()
    {
        isHolding = false;
        isCompleted = false;
        currentHoldTime = 0f;

        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = 0f;
        }
    }
}