using UnityEngine;

public enum FishType
{
    Olive,
    Blue,
    Bronze,
    Silver
}

[CreateAssetMenu(fileName = "FishSO", menuName = "Scriptable Objects/FishSO")]
public class FishSO : ScriptableObject
{
    [Header("Fish prefab")]
    [SerializeField] private FishMovement _prefab;

    [Header("Parameters")]
    public FishType Type; 
    public float SwimSpeed = 1.5f;
    public float FallSpeed = 4f;
    public int ScoreValue = 10;

    public FishMovement Prefab => _prefab;
}