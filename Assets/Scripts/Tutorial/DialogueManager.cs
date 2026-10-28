using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// 会話を1行ずつ表示し、クリックで次の行へ進める。
/// 画面固定のUIを使用するため、キャラクターの座標登録は不要。
public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private DialogueBubble dialogueBubble;

    public IEnumerator PlayDialogue(DialogueLine[] lines)
    {
        foreach (DialogueLine line in lines)
        {
            // 本文だけでなく、誰のセリフかも会話枠へ渡す。
            dialogueBubble.Show(line.speaker, line.text);

            // 時間経過ではなく、フレームごとにクリックを確認する。
            // ゲームの時間が停止していても、会話を進められる。
            while (!dialogueBubble.AdvanceRequested)
            {
                yield return null;
            }
        }

        // 枠を閉じると、枠に付けた停止処理も解除される。
        dialogueBubble.Hide();
    }
}