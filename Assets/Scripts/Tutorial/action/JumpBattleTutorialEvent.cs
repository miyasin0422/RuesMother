using UnityEngine;
using System.Collections;

public class JumpBattleTutorialEvent : TutorialEventBase
{
    [Header("画面下部に表示する操作説明")]
    [SerializeField] private string tutorialMessage = "空中で左クリック：ジャンプ攻撃";
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] TutorialUIManager tutorialUIManager;


    private GameObject rue;

    private bool started = false;
    private bool finished = false;

    public override void StartTutorial(GameObject player)
    {
        if (started || TutorialProgress.JumpBattleComplete)
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
        // 自分の説明が表示されている場合だけ消す。
        tutorialUIManager.Hide(this);
        TutorialProgress.JumpBattleComplete = true;
    }

    private IEnumerator JumpBattleTutorial()
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
            DialogueSpeaker.Ray,
            "あんな高いところじゃ、攻撃が届かないよ。"
        ),
        new DialogueLine(
            DialogueSpeaker.Ray,
            "それなら、ジャンプしながら攻撃だ！　空中でも振れるぞ！"
        ),
        new DialogueLine(
            DialogueSpeaker.Ray,
            "跳んでから、攻撃……だね！"
        )
    };

        yield return dialogueManager.PlayDialogue(lines);

        // 会話終了後は、説明を見ながら操作できるようにする。
        playerMovement.enabled = true;
        tutorialUIManager.Show(this, tutorialMessage);
    }
}
