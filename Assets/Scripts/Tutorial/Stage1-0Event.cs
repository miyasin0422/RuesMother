using System.Collections;
using UnityEngine;

public class Stage1Event : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] TutorialRaySpawner raySpawner;
    [SerializeField] PlayerSpawner playerSpawner;

    IEnumerator Start()
    {
        // ルー覚醒済みなら通常Playerで開始
        if (TutorialProgress.RueAwakened)
        {
            playerSpawner.SpawnPlayer();
            yield break;
        }

        // 初回だけレイで開始
        GameObject ray = raySpawner.SpawnRay();

        CharacterUIAnchor characterUIAnchor =
            ray.GetComponent<CharacterUIAnchor>();

        if (characterUIAnchor == null)
        {
            Debug.LogError(
                "RayPlayerにCharacterUIAnchorがありません"
            );
            yield break;
        }

        dialogueManager.RegisterSpeaker(
            DialogueSpeaker.Ray,
            characterUIAnchor.UIAnchor
        );

        DialogueLine[] lines =
        {
            new DialogueLine(
                DialogueSpeaker.Ray,
                "この先にルーが眠っているカプセルがあるはず……"
            )
        };

        yield return dialogueManager.PlayDialogue(lines);
    }
}