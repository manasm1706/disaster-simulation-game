using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.GetComponent<CharacterController>() != null)
        {
            hasTriggered = true;

            Debug.Log("PLAYER REACHED EXIT");

            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.EndGame();
            }
        }
    }
}