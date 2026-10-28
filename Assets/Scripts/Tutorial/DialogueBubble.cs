using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// 画面下部の会話枠を表示し、枠内の左クリックを受け取る。
public class DialogueBubble : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI dialogueText;

    // 現在の文章が表示されたフレーム。
    private int shownFrame;

    // 会話枠がクリックされたかどうか。
    public bool AdvanceRequested { get; private set; }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    /// 文章を更新し、この文章に対するクリック待ちを開始する。
    public void Show(string text)
    {
        AdvanceRequested = false;
        shownFrame = Time.frameCount;

        dialogueText.text = text;
        gameObject.SetActive(true);
    }

    /// 会話枠内で左クリックされたときに呼び出される。
    /// 画面の別の場所をクリックしても、この処理は呼ばれない。
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        // 表示開始と同じフレームのクリックでは進めない。
        if (Time.frameCount <= shownFrame)
        {
            return;
        }

        AdvanceRequested = true;
    }

    /// 会話枠を閉じる。
    public void Hide()
    {
        AdvanceRequested = false;
        gameObject.SetActive(false);
    }
}