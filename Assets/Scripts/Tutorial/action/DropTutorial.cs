using System.Collections;
using UnityEngine;

public class FallDropTutorialEvent : TutorialEventBase
{
    [Header("画面下部に表示する操作説明")]
    [SerializeField] private string tutorialMessage = "Sキー：下を見る";
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] TutorialUIManager tutorialUIManager;


    private GameObject rue;

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

        StartCoroutine(DropTutorial());
    }

    public override void FinishTutorial()
    {
        if (!started || finished)
        {
            return;
        }

        finished = true;
        // 自分の説明が表示されている場合だけ消す。
        tutorialUIManager.Hide(this);
    }

    private IEnumerator DropTutorial()
    {
        // 前の操作説明を消し、会話と重ならないようにする。
        tutorialUIManager.Hide();

        PlayerControll playerMovement =
            rue.GetComponent<PlayerControll>();

        Rigidbody2D rb =
            rue.GetComponent<Rigidbody2D>();

        // 会話中はプレイヤーの操作を止め、移動速度もリセットする。
        playerMovement.enabled = false;
        rb.linearVelocity = Vector2.zero;

        // レイの実体は生成せず、画面下部に会話だけを表示する。
        DialogueLine[] lines =
        {
        new DialogueLine(
            DialogueSpeaker.Rue,
            "下にも道があるのかな？"
        ),
        new DialogueLine(
            DialogueSpeaker.Ray,
            "しゃがんで、そーっとのぞいてみな。\n下の様子が見えるぞ！！"
        ),
        new DialogueLine(
            DialogueSpeaker.Rue,
            "うん。降りる前に、ちゃんと見ておくね。"
        )
    };

        yield return dialogueManager.PlayDialogue(lines);

        // 会話終了後は、説明を見ながら操作できるようにする。
        playerMovement.enabled = true;
        tutorialUIManager.Show(this, tutorialMessage);
    }

}