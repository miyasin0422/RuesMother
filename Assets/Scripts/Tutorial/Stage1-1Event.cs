using System.Collections;
using UnityEngine;

public class Stage2Event : MonoBehaviour
{
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] TutorialRaySpawner raySpawner;

    [SerializeField] GameObject ruePrefab;
    [SerializeField] Transform rueSpawnPoint;

    [SerializeField] CameraController cameraController;

    IEnumerator Start()
    {
        // レイ生成
        GameObject ray = raySpawner.SpawnRay();

        CharacterUIAnchor rayAnchor =
            ray.GetComponent<CharacterUIAnchor>();

        dialogueManager.RegisterSpeaker(
            DialogueSpeaker.Ray,
            rayAnchor.UIAnchor
        );

        // ルー生成
        GameObject rue = Instantiate(
            ruePrefab,
            rueSpawnPoint.position,
            rueSpawnPoint.rotation
        );

        CharacterUIAnchor rueAnchor =
            rue.GetComponent<CharacterUIAnchor>();

        dialogueManager.RegisterSpeaker(
            DialogueSpeaker.Rue,
            rueAnchor.UIAnchor
        );

        // 移動スクリプト取得
        RayMovement rayMovement =
            ray.GetComponent<RayMovement>();

        PlayerControll playerMovement =
            rue.GetComponent<PlayerControll>();

        // 会話中は両方操作不能
        rayMovement.enabled = false;
        playerMovement.enabled = false;

        DialogueLine[] lines =
        {
            new DialogueLine(
                DialogueSpeaker.Ray,
                "やっと見つけた"
            ),

            new DialogueLine(
                DialogueSpeaker.Rue,
                "ここはどこ……？"
            ),

            new DialogueLine(
                DialogueSpeaker.Ray,
                "君は長い間眠っていたんだ"
            )
        };

        yield return dialogueManager.PlayDialogue(lines);

        // カメラの追従対象をレイからルーへ変更
        cameraController.SetPlayerSmooth(rue.transform);

        // ルーを操作可能にする
        playerMovement.enabled = true;

        // 単体レイを削除
        Destroy(ray);

        Debug.Log("Stage2会話終了：ルーへ操作切替");
    }
}