using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void ShowResult(string result, int score)
    {
        resultText.gameObject.SetActive(true);

        resultText.text =
            "Final Score: " + score + "\n" +
            "Preparedness: " + result;

        instructionText.gameObject.SetActive(false);
    }
}