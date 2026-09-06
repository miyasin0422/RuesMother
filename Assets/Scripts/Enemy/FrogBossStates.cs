using UnityEngine;


// 跳ねながら移動
public class HopState : EnemyState
{
    FrogBossAI boss;
    FrogBossMovement movement;

    EnemyState nextState;

    int hopCount;

    float waitTimer;

    bool requestHop;
    bool jumpStarted;
    bool wasAirborne;

    public HopState(
        EnemyController controller,
        FrogBossAI boss,
        FrogBossMovement movement)
        : base(controller)
    {
        this.boss = boss;
        this.movement = movement;
    }

    public void SetNextState(EnemyState state)
    {
        nextState = state;
    }

    public override void Enter()
    {
        hopCount = 0;

        waitTimer = boss.HopInterval;

        requestHop = false;
        jumpStarted = false;
        wasAirborne = false;
    }

    public override void UpdateState()
    {
        if (boss.Player == null)
        {
            return;
        }

        if (!jumpStarted && !requestHop)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                requestHop = true;
            }
        }

        if (!jumpStarted)
        {
            return;
        }

        if (!movement.IsGrounded())
        {
            wasAirborne = true;
        }

        if (wasAirborne && movement.IsGrounded())
        {
            movement.Stop();

            hopCount++;

            jumpStarted = false;
            wasAirborne = false;

            if (hopCount >= boss.HopsBeforeAttack)
            {
                controller.ChangeState(nextState);
                return;
            }

            waitTimer = boss.HopInterval;
        }
    }

    public override void FixedUpdateState()
    {
        if (!requestHop || boss.Player == null)
        {
            return;
        }

        movement.HopToward(boss.Player);

        requestHop = false;
        jumpStarted = true;
    }
}


// 攻撃選択
public class AttackSelectState : EnemyState
{
    FrogBossAI boss;

    EnemyState tongueAttackState;
    EnemyState stompAttackState;

    public AttackSelectState(
        EnemyController controller,
        FrogBossAI boss)
        : base(controller)
    {
        this.boss = boss;
    }

    public void SetAttackStates(
        EnemyState tongue,
        EnemyState stomp)
    {
        tongueAttackState = tongue;
        stompAttackState = stomp;
    }

    public override void Enter()
    {
        float random = Random.value;

        if (random < boss.TongueAttackRate)
        {
            controller.ChangeState(tongueAttackState);
        }
        else
        {
            controller.ChangeState(stompAttackState);
        }
    }
}


// 舌攻撃
public class TongueAttackState : EnemyState
{
    FrogBossAI boss;
    FrogBossMovement movement;
    FrogBossAttackController attackController;

    EnemyState nextState;

    bool attackStarted;

    public TongueAttackState(
        EnemyController controller,
        FrogBossAI boss,
        FrogBossMovement movement,
        FrogBossAttackController attackController)
        : base(controller)
    {
        this.boss = boss;
        this.movement = movement;
        this.attackController = attackController;
    }

    public void SetNextState(EnemyState state)
    {
        nextState = state;
    }

    public override void Enter()
    {
        attackStarted = false;
    }

    public override void UpdateState()
    {
        if (boss.Player == null)
        {
            return;
        }

        if (!attackStarted)
        {

            attackController.TongueAttack(boss.Player);

            attackStarted = true;

            return;
        }

        if (!attackController.IsAttacking)
        {
            controller.ChangeState(nextState);
        }
    }

    public override void FixedUpdateState()
    {
        movement.Stop();
    }
}


// 踏みつけ
public class StompAttackState : EnemyState
{
    FrogBossAI boss;
    FrogBossMovement movement;
    FrogBossAttackController attackController;

    EnemyState nextState;

    bool requestJump;
    bool jumpStarted;
    bool wasAirborne;
    bool impactStarted;

    public StompAttackState(
        EnemyController controller,
        FrogBossAI boss,
        FrogBossMovement movement,
        FrogBossAttackController attackController)
        : base(controller)
    {
        this.boss = boss;
        this.movement = movement;
        this.attackController = attackController;
    }

    public void SetNextState(EnemyState state)
    {
        nextState = state;
    }

    public override void Enter()
    {
        requestJump = true;

        jumpStarted = false;
        wasAirborne = false;
        impactStarted = false;

    }

    public override void UpdateState()
    {
        if (!jumpStarted)
        {
            return;
        }

        if (!movement.IsGrounded())
        {
            wasAirborne = true;
        }

        if (wasAirborne &&
            movement.IsGrounded() &&
            !impactStarted)
        {
            movement.Stop();

            attackController.StompImpact();

            impactStarted = true;

            return;
        }

        if (impactStarted &&
            !attackController.IsAttacking)
        {
            controller.ChangeState(nextState);
        }
    }

    public override void FixedUpdateState()
    {
        if (!requestJump || boss.Player == null)
        {
            return;
        }

        movement.JumpToPlayer(boss.Player);

        requestJump = false;
        jumpStarted = true;
    }
}