using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyState currentState;
    public bool IsAttacking =>
    currentState is AttackState ||
    currentState is TongueAttackState ||
    currentState is StompAttackState;
    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState.Enter();
    }

    void Update()
    {
        // 会話やポーズで時間が停止している間は、
        // 敵の行動判断・状態遷移も進めない。
        if (Time.timeScale == 0f)
        {
            return;
        }

        currentState?.UpdateState();
    }

    void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }
}