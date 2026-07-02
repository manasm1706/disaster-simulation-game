using UnityEngine;
using System.Collections.Generic;

public class DecisionManager : MonoBehaviour
{
    public TextAsset jsonFile;

    private Dictionary<string, DecisionEvent> decisionMap;
    private int totalScore = 0;

    public UIManager uiManager;

    public int GetScore()
    {
        return totalScore;
    }

    void Start()
    {
        LoadData();
    }

    void LoadData()
    {
        DecisionDatabase db = JsonUtility.FromJson<DecisionDatabase>(jsonFile.text);

        decisionMap = new Dictionary<string, DecisionEvent>();

        foreach (var e in db.events)
        {
            decisionMap[e.id] = e;
        }
    }

    public void EvaluateDecision(string interactionID)
    {
        if (decisionMap.ContainsKey(interactionID))
        {
            DecisionEvent e = decisionMap[interactionID];

            totalScore += e.score;

            uiManager.UpdateScore(totalScore); // ✅ FIXED

            Debug.Log("Feedback: " + e.feedback);
            Debug.Log("Current Score: " + totalScore);
        }
        else
        {
            Debug.LogWarning("No decision found for: " + interactionID);
        }
    }
}