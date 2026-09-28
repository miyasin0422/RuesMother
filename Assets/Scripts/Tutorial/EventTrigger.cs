using UnityEngine;

public class CapsuleEventTrigger : MonoBehaviour
{
    [SerializeField] Stage2Event stage2Event;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggerに入った：" + other.name);

        RayMovement rayMovement =
            other.GetComponentInParent<RayMovement>();

        if (rayMovement == null)
        {
            Debug.Log("RayMovementが見つかりません");
            return;
        }

        if (triggered)
        {
            return;
        }

        triggered = true;

        Debug.Log("カプセルイベント開始");

        stage2Event.StartCapsuleEvent();
    }
}