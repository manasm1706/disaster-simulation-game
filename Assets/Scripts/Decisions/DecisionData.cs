using System;

[Serializable]
public class DecisionEvent
{
    public string id;
    public bool isCorrect;
    public int score;
    public string feedback;
}

[Serializable]
public class DecisionDatabase
{
    public DecisionEvent[] events;
}