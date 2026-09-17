using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Pro
}

[CreateAssetMenu(fileName = "DifficultyConfig", menuName = "Scriptable Objects/DifficultyConfig")]
public class DifficultyConfig : ScriptableObject
{
    [Header("Basic Info")]
    public Difficulty difficulty;
    public string displayName;

    [Header("Gameplay Parameters")]
    public float gameSpeed = 1.0f;
    public int scoreMultiplier = 1;
    public float timeLimit = 60f;

    public float bucketChangeInterval = 3.0f;
}