using System.Collections;
using UnityEngine;

/// <summary>
/// 指定した敵が倒されたら、会話イベントを開始する。
/// 敵と一緒に消えない、シーン内のイベント用オブジェクトに付ける。
/// </summary>
public class EnemyDefeatDialogueTrigger : MonoBehaviour
{
    [Header("会話開始の対象となる敵")]
    [SerializeField] private EnemyHealth targetEnemy;

    [Header("撃破後に再生する会話イベント")]
    [SerializeField] private DialogueTutorialEvent dialogueEvent;

    // 同じ撃破通知から、会話を複数回開始しないためのフラグ。
    private bool triggered;

    private void OnEnable()
    {
        if (targetEnemy == null || dialogueEvent == null)
        {
            Debug.LogError(
                "Target EnemyとDialogue Eventを設定してください。",
                this
            );
            return;
        }

        // この敵の撃破通知だけを受け取る。
        targetEnemy.Died += HandleEnemyDied;
    }

    private void OnDisable()
    {
        // イベント用オブジェクトが無効になったら通知の購読を解除する。
        if (targetEnemy != null)
        {
            targetEnemy.Died -= HandleEnemyDied;
        }

        StopAllCoroutines();
    }

    private void HandleEnemyDied()
    {
        if (triggered)
        {
            return;
        }

        triggered = true;
        StartCoroutine(StartDialogueAfterDrop());
    }

    private IEnumerator StartDialogueAfterDrop()
    {
        // 現在のEnemyHealthは撃破通知のあとにアイテムを生成する。
        // 次のフレームまで待ち、ドロップ処理が終わってから会話を始める。
        yield return null;

        if (dialogueEvent == null)
        {
            yield break;
        }

        // 現在のDialogueTutorialEventはplayer引数を使用していないため、
        // プレイヤーの参照を渡さずに開始できる。
        dialogueEvent.StartTutorial(null);
    }
}