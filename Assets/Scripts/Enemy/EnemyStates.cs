using UnityEngine;

public abstract class EnemyState
{
    protected EnemyController controller;

    public EnemyState(EnemyController controller)
    {
        this.controller = controller;
    }

    public virtual void Enter() { }

    public virtual void UpdateState() { }

    public virtual void FixedUpdateState() { }

    public virtual void Exit() { }
}


// 巡回
public class PatrolState : EnemyState
{
    EnemySensor sensor;
    EnemyMovement movement;

    EnemyState chaseState;

    public PatrolState(
        EnemyController controller,
        EnemySensor sensor,
        EnemyMovement movement)
        : base(controller)
    {
        this.sensor = sensor;
        this.movement = movement;
    }

    public void SetChaseState(EnemyState chaseState)
    {
        this.chaseState = chaseState;
    }

    public override void UpdateState()
    {
        if (sensor.Player != null)
        {
            Debug.Log("巡回→追跡");
            controller.ChangeState(chaseState);
        }
    }

    public override void FixedUpdateState()
    {
        movement.Patrol();
    }
}


// 追跡
public class ChaseState : EnemyState
{
    EnemySensor sensor;
    EnemyMovement movement;

    EnemyState patrolState;
    EnemyState attackState;

    public ChaseState(
        EnemyController controller,
        EnemySensor sensor,
        EnemyMovement movement)
        : base(controller)
    {
        this.sensor = sensor;
        this.movement = movement;
    }

    public void SetPatrolState(EnemyState state)
    {
        patrolState = state;
    }

    public void SetAttackState(EnemyState state)
    {
        attackState = state;
    }

    public override void UpdateState()
    {
        if (sensor.Player == null)
        {
            controller.ChangeState(patrolState);
            return;
        }

        if (sensor.IsPlayerInAttackRange())
        {
            controller.ChangeState(attackState);
        }
    }

    public override void FixedUpdateState()
    {
        if (sensor.Player == null)
        {
            movement.Stop();
            return;
        }

        movement.MoveToward(sensor.Player);
    }
}


// 攻撃
public class AttackState : EnemyState
{
    EnemySensor sensor;
    EnemyMovement movement;
    EnemyAttackController attackController;

    EnemyState patrolState;
    EnemyState chaseState;

    public AttackState(
        EnemyController controller,
        EnemySensor sensor,
        EnemyMovement movement,
        EnemyAttackController attackController)
        : base(controller)
    {
        this.sensor = sensor;
        this.movement = movement;
        this.attackController = attackController;
    }

    public void SetPatrolState(EnemyState state)
    {
        patrolState = state;
    }

    public void SetChaseState(EnemyState state)
    {
        chaseState = state;
    }

    public override void UpdateState()
    {
        if (sensor.Player == null)
        {
            controller.ChangeState(patrolState);
            return;
        }

        if (!sensor.IsPlayerInAttackRange())
        {
            controller.ChangeState(chaseState);
            return;
        }

        attackController.Attack(sensor.Player);
    }

    public override void FixedUpdateState()
    {
        movement.Stop();

        if (sensor.Player != null)
        {
            movement.FaceTarget(sensor.Player);
        }
    }
}