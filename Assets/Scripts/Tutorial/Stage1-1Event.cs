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
        RayMovement rayMovement =
        ray.GetComponent<RayMovement>();

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

        // カプセル破壊
        yield return capsuleBreak.PlayBreak();

        // ルー生成
        rue = Instantiate(
            ruePrefab,
            rueSpawnPoint.position,
            rueSpawnPoint.rotation
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