using UnityEngine;
using System.Collections;

public class EarthquakeController : MonoBehaviour
{
    public float duration = 5f;
    public float magnitude = 0.2f;

    public Transform playerCamera;

    private Vector3 originalPos;

    void Start()
    {
        originalPos = playerCamera.localPosition;
        Invoke("StartEarthquake", 3f);
    }

    public void StartEarthquake()
    {
        StartCoroutine(Shake());
        ActivateFallingObjects();
    }

    IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            playerCamera.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.localPosition = originalPos;

        Debug.Log("EARTHQUAKE ENDED");
    }

    void ActivateFallingObjects()
    {
        Rigidbody[] bodies = FindObjectsOfType<Rigidbody>();

        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = false;
            rb.AddForce(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
    }
}