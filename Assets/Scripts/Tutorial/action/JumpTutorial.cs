using System.Collections;
using UnityEngine;

public class JumpTutorialEvent : TutorialEventBase
{
    [Header("画面下部に表示する操作説明")]
    [SerializeField] private string tutorialMessage = "スペース：ジャンプ";
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

        StartCoroutine(JumpTutorial());
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

    private IEnumerator JumpTutorial()
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
            "この段差、登れるかな……？"
        ),
        new DialogueLine(
            DialogueSpeaker.Ray,
            "ジャンプで、ぴょんっ！　だ。ルーなら届くぞ！"
        ),
        new DialogueLine(
            DialogueSpeaker.Rue,
            "ぴょんっ、だね。やってみる！"
        )
    };

        yield return dialogueManager.PlayDialogue(lines);

        // 会話終了後は、説明を見ながら操作できるようにする。
        playerMovement.enabled = true;
        tutorialUIManager.Show(this, tutorialMessage);
    }

}