using UnityEngine;

public class BasicEnemyAI : MonoBehaviour
{
    [SerializeField] EnemyController controller;
    [SerializeField] EnemySensor sensor;
    [SerializeField] EnemyMovement movement;
    [SerializeField] EnemyAttackController attackController;

    void Start()
    {
        PatrolState patrolState =
            new PatrolState(controller, sensor, movement);

        ChaseState chaseState =
            new ChaseState(controller, sensor, movement);

        AttackState attackState =
            new AttackState(
                controller,
                sensor,
                movement,
                attackController
            );

        patrolState.SetChaseState(chaseState);

        chaseState.SetPatrolState(patrolState);
        chaseState.SetAttackState(attackState);

        attackState.SetPatrolState(patrolState);
        attackState.SetChaseState(chaseState);

        controller.ChangeState(patrolState);
    }

    void Update()
    {
        sensor.SearchPlayer();
    }
}