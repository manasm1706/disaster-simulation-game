using UnityEngine;
using System.Collections;

public class DoorInteractable : InteractableObject
{
    [Header("Door Settings")]
    public Transform[] doorPivots;

    public float openAngle = 120f;
    public float speed = 2f;

    [Header("Optional")]
    public Collider[] doorColliders;

    private bool isOpen = false;
    private bool isMoving = false;

    public override void Interact()
    {
        if (isMoving)
            return;

        Debug.Log("Door Interact called");

        base.Interact();

        isOpen = !isOpen;

        StopAllCoroutines();
        StartCoroutine(RotateDoors());
    }

    IEnumerator RotateDoors()
    {
        isMoving = true;

        Quaternion[] startRotations = new Quaternion[doorPivots.Length];
        Quaternion[] targetRotations = new Quaternion[doorPivots.Length];

        for (int i = 0; i < doorPivots.Length; i++)
        {
            startRotations[i] = doorPivots[i].localRotation;

            float angle = openAngle;

            // Make second door open opposite direction
            if (i % 2 == 1)
                angle = -openAngle;

            if (isOpen)
                targetRotations[i] = Quaternion.Euler(0, angle, 0);
            else
                targetRotations[i] = Quaternion.identity;
        }

        // Disable colliders while open
        if (doorColliders != null)
        {
            foreach (Collider col in doorColliders)
            {
                if (col != null)
                    col.enabled = !isOpen;
            }
        }

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * speed;

            for (int i = 0; i < doorPivots.Length; i++)
            {
                if (doorPivots[i] != null)
                {
                    doorPivots[i].localRotation =
                        Quaternion.Lerp(
                            startRotations[i],
                            targetRotations[i],
                            t);
                }
            }

            yield return null;
        }

        isMoving = false;
    }
}