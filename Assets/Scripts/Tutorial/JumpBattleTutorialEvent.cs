using UnityEngine;
using System.Collections;

public class JumpBattleTutorialEvent : TutorialEventBase
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] TutorialUIManager tutorialUIManager;
    [SerializeField] TutorialRaySpawner raySpawner;

    private GameObject rue;
    private GameObject ray;

    private bool started = false;
    private bool finished = false;

    public override void StartTutorial(GameObject player)
    {
        if (started)
        {
            return;
        }

        started = true;
        rue = player;

        StartCoroutine(JumpBattleTutorial());
    }

    public override void FinishTutorial()
    {
        if (!started || finished)
        {
            return;
        }

        finished = true;
        tutorialUIManager.Hide();
    }

    private IEnumerator JumpBattleTutorial()
    {
        PlayerControll playerMovement =
            rue.GetComponent<PlayerControll>();

        Rigidbody2D rb =
            rue.GetComponent<Rigidbody2D>();

        // ルー停止
        playerMovement.enabled = false;

        rb.linearVelocity = Vector2.zero;

        // ルーの近くにあるSpawnPointを取得
        Transform raySpawnPoint =
            rue.transform.Find("RayDialogueSpawnPoint");

        // レイ出現
        ray = raySpawner.SpawnRay(raySpawnPoint);
        RayMovement rayMovement =
             ray.GetComponent<RayMovement>();

        if (rayMovement != null)
        {
            rayMovement.enabled = false;
        }
        CharacterUIAnchor rayAnchor =
            ray.GetComponent<CharacterUIAnchor>();

        dialogueManager.RegisterSpeaker(
            DialogueSpeaker.Ray,
            rayAnchor.UIAnchor
        );

        DialogueLine[] lines =
        {
            new DialogueLine(
                DialogueSpeaker.Ray,
                "空中にも敵がいるな"
            ),

            new DialogueLine(
                DialogueSpeaker.Ray,
                "ジャンプしながら攻撃してみろ"
            )
        };

        yield return dialogueManager.PlayDialogue(lines);

        Destroy(ray);

        playerMovement.enabled = true;

        tutorialUIManager.Show(
            "Space + 右クリック：ジャンプ攻撃"
        );
    }
}
