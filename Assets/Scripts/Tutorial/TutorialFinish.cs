using UnityEngine;

public class TutorialFinishTrigger : MonoBehaviour
{
    [SerializeField] TutorialEventBase tutorialEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerControll playerMovement =
            other.GetComponentInParent<PlayerControll>();

        if (playerMovement == null)
        {
            return;
        }

        tutorialEvent.FinishTutorial();
    }
}