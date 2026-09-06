using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    public abstract void Initialize(int attackPower, Vector2 targetPosition);
}