using System.Collections;
using UnityEngine;

public class JumpTutorialEvent : TutorialEventBase
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

        StartCoroutine(JumpTutorial());
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

    private IEnumerator JumpTutorial()
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

        // 会話
        DialogueLine[] lines =
        {
            new DialogueLine(
                DialogueSpeaker.Ray,
                "高いところはジャンプをすればいい"
            )
        };

        yield return dialogueManager.PlayDialogue(lines);

        // 会話用レイを消す
        Destroy(ray);

        // ルー操作再開
        playerMovement.enabled = true;

        // 操作説明
        tutorialUIManager.Show(
            "Space：ジャンプ"
        );
    }

}