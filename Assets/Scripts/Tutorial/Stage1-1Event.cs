using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Stage2Event : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] TutorialRaySpawner raySpawner;
    [SerializeField] PlayerSpawner playerSpawner;

    [SerializeField] GameObject ruePrefab;
    [SerializeField] Transform rueSpawnPoint;
    [Header("操作説明")]
    [SerializeField] private TutorialUIManager tutorialUIManager;
    [SerializeField] CameraController cameraController;
    [SerializeField] CapsuleBreak capsuleBreak;
    [Header("カプセルを壊す剣の演出")]
    [SerializeField] GameObject capsuleSwordPrefab;
    [SerializeField] Transform capsuleAttackPoint;
    [SerializeField] float capsuleHitDelay = 0.25f;

    private GameObject ray;
    private GameObject rue;

    private bool eventStarted = false;

    private void Start()
    {
        if (TutorialProgress.RueAwakened)
        {
            playerSpawner.SpawnPlayer();

            // カプセルも残したくないならここで非表示
            capsuleBreak.gameObject.SetActive(false);

            return;
        }

        // 初回だけレイ生成
        ray = raySpawner.SpawnRay();
    }

    public void StartCapsuleEvent()
    {
        if (eventStarted)
        {
            return;
        }

        eventStarted = true;

        StartCoroutine(CapsuleEvent());
    }

    private IEnumerator CapsuleEvent()
    {
        RayTutorialMove rayMovement =
        ray.GetComponent<RayTutorialMove>();

        rayMovement.enabled = false;

        cameraController.FixX(
            capsuleBreak.transform.position.x
        );

        DialogueLine[] openingDialogue =
        {
        new DialogueLine(
            DialogueSpeaker.Ray,
            "いたいた！　やっと見つけた！"
        ),new DialogueLine(
            DialogueSpeaker.Ray,
            "ルー、ちょっとだけ待ってろよ。\n今、ここから出してやる！"
        )
    };

        yield return dialogueManager.PlayDialogue(
            openingDialogue
        );

        // 会話終了後、剣を振るための操作説明を表示する。
        tutorialUIManager.Show(this, "左クリック：剣で攻撃");

        // プレイヤーが左クリックするまで待つ。
        yield return WaitForLeftClick();

        // 入力されたら説明を消し、続く剣の演出へ進む。
        tutorialUIManager.Hide(this);

        if (capsuleSwordPrefab == null || capsuleAttackPoint == null)
        {
            Debug.LogError("カプセル演出の剣Prefabと攻撃位置を設定してください。");
            yield break;
        }

        // 背景を削除する前に、ルーの出現位置を保存
        Vector3 ruePosition = rueSpawnPoint.position;
        Quaternion rueRotation = rueSpawnPoint.rotation;

        // 剣の演出を生成
        GameObject sword = Instantiate(
            capsuleSwordPrefab,
            capsuleAttackPoint.position,
            capsuleAttackPoint.rotation
        );

        AttackBehavior behavior = sword.GetComponent<AttackBehavior>();
        GewenController controller = sword.GetComponent<GewenController>();

        if (behavior == null || controller == null)
        {
            Debug.LogError("剣PrefabにAttackBehaviorとGewenControllerが必要です。");
            Destroy(sword);
            yield break;
        }

        // 演出なので攻撃力は0
        behavior.Initialize(
            new Gene.WeaponParam(0f, 1f, 5f, false, 0f)
        );

        // レイの姿を隠して、剣を振る
        ray.SetActive(false);
        controller.AttackPressed();

        // 剣が当たるタイミングで背景を削除
        yield return new WaitForSeconds(capsuleHitDelay);
        yield return capsuleBreak.PlayBreak();

        // 既存の攻撃終了処理で剣が消えるのを待つ
        while (sword != null)
        {
            yield return null;
        }

        // 会話用にレイを戻す
        ray.SetActive(true);

        // ルー生成
        rue = Instantiate(
            ruePrefab,
            ruePosition,
            rueRotation
        );

        // ルーは会話中なので操作不能
        PlayerControll playerMovement =
            rue.GetComponent<PlayerControll>();

        playerMovement.enabled = false;

        // ルーとレイの掛け合い
        DialogueLine[] afterBreakDialogue =
        {
    new DialogueLine(
        DialogueSpeaker.Rue,
        "ん……ここ、どこ……？"
    ),

    new DialogueLine(
        DialogueSpeaker.Ray,
        "おはよう、ルー！ずいぶんお寝坊したな！"
    ),
        new DialogueLine(
        DialogueSpeaker.Rue,
        "あなた……だれ？"
    ),

    new DialogueLine(
        DialogueSpeaker.Ray,
        "オレはレイ。ルーを迎えにきたんだ！"
    ),
    new DialogueLine(
        DialogueSpeaker.Ray,
        "お母さんに会いにいこう。ルーに会ってほしいんだ。"
    ),

    new DialogueLine(
        DialogueSpeaker.Rue,
        "お母さん……。どこにいるの？"
    ),

    new DialogueLine(
        DialogueSpeaker.Ray,
        "まずは、ここを出よう。オレもついてるからさ！"
    ),
    new DialogueLine(
        DialogueSpeaker.Rue,
        "……うん。いっしょに来てね。"
    ),
};

        yield return dialogueManager.PlayDialogue(
            afterBreakDialogue
        );

        // ここでルー覚醒済みにする
        TutorialProgress.RueAwakened = true;

        // 掛け合い終了後、カメラをルーへ戻す
        cameraController.SetPlayerSmooth(rue.transform);
        cameraController.StartFollowX();

        // ルー操作可能
        playerMovement.enabled = true;

        // チュートリアル用レイを削除
        Destroy(ray);
    }
    private IEnumerator WaitForLeftClick()
    {
        yield return null;

        while (
            Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame
        )
        {
            yield return null;
        }
    }
}