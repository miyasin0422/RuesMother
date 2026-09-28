using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private DialogueBubble dialogueBubble;

    private Dictionary<DialogueSpeaker, Transform> speakerAnchors
        = new Dictionary<DialogueSpeaker, Transform>();

    // 生成されたキャラクターのAnchorを登録
    public void RegisterSpeaker(
        DialogueSpeaker speaker,
        Transform anchor
    )
    {
        speakerAnchors[speaker] = anchor;
    }

    // 会話を再生
    public IEnumerator PlayDialogue(DialogueLine[] lines)
    {
        foreach (DialogueLine line in lines)
        {
            if (!speakerAnchors.TryGetValue(
                line.speaker,
                out Transform anchor
            ))
            {
                Debug.LogError(
                    $"{line.speaker} のUIAnchorが登録されていません"
                );

                continue;
            }

            dialogueBubble.Show(
                anchor,
                line.text
            );

            yield return WaitForClick();
        }

        dialogueBubble.Hide();
    }

    IEnumerator WaitForClick()
    {
        // 会話表示と同じフレームのクリックを拾わない
        yield return null;

        while (
            Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame
        )
        {
            yield return null;
        }
    }
}