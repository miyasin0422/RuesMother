using TMPro;
using UnityEngine;

/// 操作説明パネルの文章と表示状態を管理する。
/// 非表示にするパネルとは別の、常時有効なオブジェクトに付ける。
public class TutorialUIManager : MonoBehaviour
{
    [Header("操作説明の表示先")]
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private TextMeshProUGUI tutorialText;

    // 現在の説明を表示したイベント。
    // 通常のチュートリアルとステージ固有イベントの両方を扱う。
    private MonoBehaviour currentOwner;

    private void Awake()
    {
        if (tutorialUI == null)
        {
            Debug.LogError(
                "TutorialUIManagerのTutorial UIを設定してください。",
                this
            );
            return;
        }

        // ゲーム開始時は操作説明を非表示にする。
        tutorialUI.SetActive(false);
    }

    /// 説明文と表示元を登録し、操作説明を表示する。
    public void Show(MonoBehaviour owner, string message)
    {
        if (owner == null || tutorialUI == null || tutorialText == null)
        {
            Debug.LogError(
                "表示元、Tutorial UI、Tutorial Textを確認してください。",
                this
            );
            return;
        }

        currentOwner = owner;
        tutorialText.text = message;
        tutorialUI.SetActive(true);
    }

    /// 現在の表示元と一致する場合だけ説明を消す。
    /// 別のイベントが表示した説明を誤って消すことを防ぐ。
    public void Hide(MonoBehaviour owner)
    {
        if (owner == null || currentOwner != owner)
        {
            return;
        }

        Hide();
    }

    /// 既存コードとの互換用。
    /// 設定済みの文章を変更せずに表示する。
    public void Show()
    {
        if (tutorialUI == null)
        {
            return;
        }

        currentOwner = null;
        tutorialUI.SetActive(true);
    }

    /// 表示元に関係なく説明を消す。
    /// 新しい会話を開始するときなどに使用する。
    public void Hide()
    {
        currentOwner = null;

        if (tutorialUI != null)
        {
            tutorialUI.SetActive(false);
        }
    }
}