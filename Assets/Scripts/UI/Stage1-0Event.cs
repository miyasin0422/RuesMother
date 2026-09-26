using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Stage1Event : MonoBehaviour
{
    [SerializeField] DialogueBubble dialogueBubble;
    [SerializeField] Transform rayUIAnchor;

    IEnumerator Start()
    {
        dialogueBubble.Show(
            rayUIAnchor,
            "この先にルーが眠っているカプセルがあるはず……"
        );

        yield return WaitForClick();

        dialogueBubble.Hide();

        Debug.Log("会話終了");
    }

    IEnumerator WaitForClick()
    {
        // 会話を表示した瞬間のクリックを拾わないため1フレーム待つ
        yield return null;

        while (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            yield return null;
        }
    }
}