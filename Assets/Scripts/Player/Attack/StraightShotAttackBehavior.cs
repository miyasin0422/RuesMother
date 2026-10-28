using UnityEngine;

public class StraightShotAttackBehavior : AttackBehavior
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform attackPoint;
    [SerializeField] float speed = 10f;
    [SerializeField] private AudioClip AttackSE;
    [SerializeField] private AudioSource audioSource;

    public override void AttackPressed()
    {
        audioSource.PlayOneShot(AttackSE);
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