using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public Camera playerCamera;

    private GameObject lastHitObject; // ✅ MOVE IT HERE
    private bool mobileInteract = false;

    public void OnMobileInteract()
    {
        mobileInteract = true;
    }

    void Update()
    {
        HandleInteraction();
    }

    void HandleInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.gameObject != lastHitObject)
            {
                lastHitObject = hit.collider.gameObject;
                Debug.Log("Looking at: " + hit.collider.name);
            }

            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null && (Input.GetKeyDown(KeyCode.E) || mobileInteract))
            {
                interactable.Interact();
                mobileInteract = false;
            }
        }
        else
        {
            lastHitObject = null;
        }
    }
}