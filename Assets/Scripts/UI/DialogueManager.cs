using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] DialogueBubble dialogueBubble;

    public IEnumerator PlayDialogue(DialogueLine[] lines)
    {
        foreach (DialogueLine line in lines)
        {
            dialogueBubble.Show(
                line.speakerAnchor,
                line.text
            );

            yield return WaitForClick();
        }

        dialogueBubble.Hide();
    }

    IEnumerator WaitForClick()
    {
        yield return null;

        while (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            yield return null;
        }
    }
}