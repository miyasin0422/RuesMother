using System.Collections;
using UnityEngine;

public class Stage1Event : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] TutorialRaySpawner raySpawner;
    [SerializeField] PlayerSpawner playerSpawner;
    [SerializeField] TutorialUIManager tutorialUIManager;

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

        DialogueLine[] lines =
        {
            new DialogueLine(
                DialogueSpeaker.Ray,
                "ルーが眠ってるカプセル、\nこの先のはずなんだけど……"
            ),
            new DialogueLine(
                DialogueSpeaker.Ray,
                "よーし、待ってろよ。\n今、迎えにいくからな！"
            )
        };

        yield return dialogueManager.PlayDialogue(lines);
        tutorialUIManager.Show();
    }
}