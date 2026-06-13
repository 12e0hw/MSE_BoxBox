using UnityEngine;

// Store stage-specific settings as a ScriptableObject.
[CreateAssetMenu(fileName = "StageConfig", menuName = "BoxBox/Stage Config")]
public class StageConfig : ScriptableObject
{
    [Header("Stage Info")]
    [SerializeField] private int stageId = 1;
    [SerializeField] private string stageName = "Stage 1";

    [Header("Stage Goal")]
    [SerializeField] private int targetScore = 100;

    [Header("Time")]
    [SerializeField] private float timeLimit = 180f;

    [Header("Spawn")]
    [SerializeField] private float boxSpawnInterval = 3f;

    public int StageId => stageId;
    public string StageName => stageName;
    public int TargetScore => targetScore;
    public float TimeLimit => timeLimit;
    public float BoxSpawnInterval => boxSpawnInterval;
}