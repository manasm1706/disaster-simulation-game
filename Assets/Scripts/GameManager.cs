using UnityEngine;

public class GameManager : MonoBehaviour
{
   public void EndGame()
    {
        Debug.Log("Game Finished");

        DecisionManager dm = FindFirstObjectByType<DecisionManager>();

        int finalScore = 0;

        if (dm != null)
        {
            finalScore = dm.GetScore();   // 🔥 get score from decision system
        }

        string result = GetPreparednessLevel(finalScore);

        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui != null)
        {
            ui.ShowResult(result, finalScore);
        }
    }

    string GetPreparednessLevel(int score)
    {
        if (score >= 30) return "HIGH";
        if (score >= 15) return "MEDIUM";
        return "LOW";
    }
}