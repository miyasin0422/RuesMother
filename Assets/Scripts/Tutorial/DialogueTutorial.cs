using System.Collections;
using UnityEngine;
public enum DialogueEventType
{
    GeneDrop,   // 遺伝子ドロップ時の会話
    SavePoint,  // セーブ地点の会話
    BeforeBoss  // ボス部屋前の会話
}
/// 開始トリガーから呼び出され、会話だけを再生するイベント。
/// 操作説明や終了トリガーは使用しない。
public class DialogueTutorialEvent : TutorialEventBase
{
    [Header("共通の会話UI")]
    [SerializeField] private DialogueManager dialogueManager;
    [Header("会話開始時に消す操作説明")]
    [SerializeField] private TutorialUIManager tutorialUIManager;
    [Header("上から順番に再生する会話")]
    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private DialogueEventType eventType;

    // この会話に対応する完了フラグを確認する。
    private bool IsCompleted()
    {
        switch (eventType)
        {
            case DialogueEventType.GeneDrop:
                return TutorialProgress.GeneComplete;

            case DialogueEventType.SavePoint:
                return TutorialProgress.SaveComplete;

            case DialogueEventType.BeforeBoss:
                return TutorialProgress.BossComplete;

            default:
                return false;
        }
    }

    // この会話に対応する完了フラグを立てる。
    private void MarkCompleted()
    {
        switch (eventType)
        {
            case DialogueEventType.GeneDrop:
                TutorialProgress.GeneComplete = true;
                break;

            case DialogueEventType.SavePoint:
                TutorialProgress.SaveComplete = true;
                break;

            case DialogueEventType.BeforeBoss:
                TutorialProgress.BossComplete = true;
                break;
        }
    }
    // 同じシーン内で、会話が繰り返し始まることを防ぐ。
    private bool started;
    private bool finished;

    public override void StartTutorial(GameObject player)
    {
        // 今回すでに開始済み、または以前に完了済みなら再生しない。
        if (started || finished || IsCompleted())
        {
            return;
        }

        // 設定不足の場合は、開始済みにせずエラーを知らせる。
        if (dialogueManager == null ||
            dialogueLines == null ||
            dialogueLines.Length == 0)
        {
            Debug.LogError(
                "Dialogue Managerと会話文を設定してください。",
                this
            );
            return;
        }

        // 空の会話要素によるエラーを防ぐ。
        foreach (DialogueLine line in dialogueLines)
        {
            if (line == null)
            {
                Debug.LogError(
                    "会話リストに未設定の要素があります。",
                    this
                );
                return;
            }
        }

        started = true;
        StartCoroutine(PlayEvent());
    }

    private IEnumerator PlayEvent()
    {
        // 前のチュートリアルの操作説明を消してから会話を始める。
        // 操作説明UIを使わないシーンでは、未設定でも会話を再生できる。
        if (tutorialUIManager != null)
        {
            tutorialUIManager.Hide();
        }

        // 会話中の操作禁止は、会話枠のPlayerActionBlockerに任せる。
        yield return dialogueManager.PlayDialogue(dialogueLines);

        // 最後のセリフを読み終えたらイベントを完了する。
        FinishTutorial();
    }

    public override void FinishTutorial()
    {
        // 基底クラスで必要なメソッド。
        // FinishTriggerからではなく、会話終了時に呼び出す。
        if (!started || finished)
        {
            return;
        }

        finished = true;

        // 最後まで会話を読んだので、完了を記録する。
        MarkCompleted();
    }
}