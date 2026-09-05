using UnityEngine;

public abstract class AttackBehavior : MonoBehaviour
{
    public virtual bool IsMovementLocked => false;
    public virtual bool FollowMouseFacing => false;
    public virtual void AttackPressed()
    {
    }

    public virtual void AttackHeld()
    {
    }

    public virtual void AttackReleased()
    {
    }
}