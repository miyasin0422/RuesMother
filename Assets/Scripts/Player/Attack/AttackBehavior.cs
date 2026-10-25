using UnityEngine;

public abstract class AttackBehavior : MonoBehaviour
{
    // この攻撃を開始した時点の武器性能
    public Gene.WeaponParam Parameters { get; private set; }

    public virtual bool IsMovementLocked => false;
    public virtual bool FollowMouseFacing => false;

    public void Initialize(Gene.WeaponParam parameters)
    {
        Parameters = parameters;
    }

    // 生成した当たり判定や弾にも性能を渡す
    protected void InitializeHitboxes(GameObject attackObject)
    {
        SwordAttack[] hitboxes =
            attackObject.GetComponentsInChildren<SwordAttack>(true);

        foreach (SwordAttack hitbox in hitboxes)
        {
            hitbox.Initialize(Parameters);
        }
    }

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