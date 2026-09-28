using System.Collections;
using UnityEngine;

public class Stage1Event : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] TutorialRaySpawner raySpawner;

    IEnumerator Start()
    {
        Debug.Log("Stage1Event開始");

        // レイ生成
        GameObject ray = raySpawner.SpawnRay();

        Debug.Log("Ray生成完了");

        // UIAnchor取得
        CharacterUIAnchor characterUIAnchor =
            ray.GetComponent<CharacterUIAnchor>();

        if (characterUIAnchor == null)
        {
            Debug.LogError(
                "RayPlayerにCharacterUIAnchorがありません"
            );
            yield break;
        }

        // DialogueManagerへレイを登録
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

        // 会話開始
        yield return dialogueManager.PlayDialogue(lines);

        Debug.Log("Stage1会話終了");
    }
}