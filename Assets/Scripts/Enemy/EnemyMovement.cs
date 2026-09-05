using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour
{
    public abstract void Patrol();

    public abstract void MoveToward(Transform target);

    public abstract void Stop();

    public abstract void FaceTarget(Transform target);
}