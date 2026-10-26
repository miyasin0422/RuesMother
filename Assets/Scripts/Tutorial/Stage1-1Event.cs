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

        CharacterUIAnchor rayAnchor =
            ray.GetComponent<CharacterUIAnchor>();

        dialogueManager.RegisterSpeaker(
            DialogueSpeaker.Ray,
            rayAnchor.UIAnchor
        );
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
            "やっと見つけた"
        )
    };

        yield return dialogueManager.PlayDialogue(
            openingDialogue
        );

        // 左クリック待ち
        yield return WaitForLeftClick();

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

        // ルーのUIAnchorをDialogueManagerに登録
        CharacterUIAnchor rueAnchor =
            rue.GetComponent<CharacterUIAnchor>();

        dialogueManager.RegisterSpeaker(
            DialogueSpeaker.Rue,
            rueAnchor.UIAnchor
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
        "ここはどこ……？"
    ),

    new DialogueLine(
        DialogueSpeaker.Ray,
        "君は長い間眠っていたんだ"
    ),

    new DialogueLine(
        DialogueSpeaker.Ray,
        "君は母親と会わなければならない"
    ),

    new DialogueLine(
        DialogueSpeaker.Rue,
        "お母さん……？"
    ),

    new DialogueLine(
        DialogueSpeaker.Ray,
        "まずはここを抜け出さなければ"
    )
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