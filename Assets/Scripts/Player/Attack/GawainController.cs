using UnityEngine;

public class GewenController : MonoBehaviour
{
    [SerializeField] float destroyDelay = 0.3f;
    AttackBehavior attackBehavior;
    public bool IsMovementLocked => attackBehavior.IsMovementLocked;
    void Awake()
    {
        attackBehavior = GetComponent<AttackBehavior>();
    }
    public bool FollowMouseFacing
    {
        get
        {
            return attackBehavior.FollowMouseFacing;
        }
    }
    public void AttackPressed()
    {
        attackBehavior.AttackPressed();
    }

    public void AttackHeld()
    {
        attackBehavior.AttackHeld();
    }

    public void AttackReleased()
    {
        attackBehavior.AttackReleased();
    }
    public void EndAttack()
    {
        Destroy(gameObject, destroyDelay);
    }
}