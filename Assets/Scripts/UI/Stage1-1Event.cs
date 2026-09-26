using System.Collections;
using UnityEngine;

public class Stage2Event : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] DialogueLine[] dialogueLines;

    IEnumerator Start()
    {
        yield return dialogueManager.PlayDialogue(dialogueLines);

        Debug.Log("Stage2の会話終了");
    }
}