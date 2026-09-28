using UnityEngine;

public abstract class TutorialEventBase : MonoBehaviour
{
    public abstract void StartTutorial(GameObject player);
    public abstract void FinishTutorial();
}