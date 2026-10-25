using UnityEngine;

public class StraightShotAttackBehavior : AttackBehavior
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform attackPoint;
    [SerializeField] float speed = 10f;

    public override void AttackPressed()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            attackPoint.position,
            Quaternion.identity
        );
        InitializeHitboxes(projectile);
        int direction = transform.localScale.x > 0 ? 1 : -1;

        StraightProjectileMovement movement =
            projectile.GetComponent<StraightProjectileMovement>();

        movement.Shoot(
            direction,
            speed,
            Mathf.Max(0f, Parameters.range)
        );

        GetComponent<GewenController>().EndAttack();
    }
}