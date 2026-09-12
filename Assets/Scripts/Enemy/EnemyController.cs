using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyState currentState;

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState.Enter();
    }

    void Update()
    {
        currentState?.UpdateState();
    }

    void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }
}