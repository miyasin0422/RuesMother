using System.Net.NetworkInformation;
using System.Collections;
using UnityEngine;

public class FrogBossAI : MonoBehaviour
{
    [SerializeField] EnemyController controller;
    [SerializeField] FrogBossMovement movement;
    [SerializeField] FrogBossAttackController attackController;

    [SerializeField] int hopsBeforeAttack = 2;
    [SerializeField] float hopInterval = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] float tongueAttackRate = 0.5f;

    public Transform Player { get; private set; }

    public int HopsBeforeAttack => hopsBeforeAttack;
    public float HopInterval => hopInterval;
    public float TongueAttackRate => tongueAttackRate;

    bool battleStarted;

    HopState hopState;
    AttackSelectState attackSelectState;
    TongueAttackState tongueAttackState;
    StompAttackState stompAttackState;
    void Awake()
    {
        hopState =
            new HopState(controller, this, movement);

        attackSelectState =
            new AttackSelectState(controller, this);

        tongueAttackState =
            new TongueAttackState(
                controller,
                this,
                movement,
                attackController
            );

        stompAttackState =
            new StompAttackState(
                controller,
                this,
                movement,
                attackController
            );

        hopState.SetNextState(attackSelectState);

        attackSelectState.SetAttackStates(
            tongueAttackState,
            stompAttackState
        );

        tongueAttackState.SetNextState(hopState);

        stompAttackState.SetNextState(hopState);
    }

    public void StartBattle(Transform player)
    {
        if (battleStarted)
        {
            return;
        }

        Player = player;
        battleStarted = true;

        Debug.Log("HopState開始");

        controller.ChangeState(hopState);
    }

}