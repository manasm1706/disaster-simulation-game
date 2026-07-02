using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string interactionID;

    [Header("Highlight")]
    public Color highlightColor = Color.yellow;

    private Renderer objectRenderer;
    private Color originalColor;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalColor =
                objectRenderer.material.color;
        }
    }

    public void Highlight()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color =
                highlightColor;
        }
    }

    public void UnHighlight()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color =
                originalColor;
        }
    }

    public virtual void Interact()
    {
        DecisionManager dm =
            FindObjectOfType<DecisionManager>();

        if (dm != null)
        {
            dm.EvaluateDecision(interactionID);
        }
    }
}