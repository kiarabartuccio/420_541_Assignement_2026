using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Level2";

    private bool activated;
    private void OnTriggerEnter(Collider other)
    {
        if (activated)
        {
            return;
        }

        CharacterMovement player = other.GetComponentInParent<CharacterMovement>();

        if (player == null)
        {
            return;
        }

        activated = true;
        GameManager.Instance.CompleteLevel(nextSceneName);
    }
}
