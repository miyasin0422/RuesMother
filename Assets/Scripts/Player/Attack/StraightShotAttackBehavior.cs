using UnityEngine;

public class StraightShotAttackBehavior : AttackBehavior
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform attackPoint;
    [SerializeField] float speed = 10f;
    [SerializeField] float maxDistance = 5f;

    public override void AttackPressed()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            attackPoint.position,
            Quaternion.identity
        );

        int direction = transform.localScale.x > 0 ? 1 : -1;

        StraightProjectileMovement movement =
            projectile.GetComponent<StraightProjectileMovement>();

        movement.Shoot(direction, speed, maxDistance);

        GetComponent<GewenController>().EndAttack();
    }
}