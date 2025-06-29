using UnityEngine;

[CreateAssetMenu(fileName = "LevelSystemConfig", menuName = "Game/Level System Config")]
public class LevelSystemConfig : ScriptableObject
{
    [Header("Prefab References")]
    public GameObject[] obstaclePrefabs;

    [Header("Default Level Data")]
    public LevelData defaultLevelData;

    [Header("Level Data Assets")]
    public LevelData[] levelDataAssets;

    [Header("Scene References")]
    public string levelParentName = "LevelParent";
    public string PooledObjectsHolderName = "PooledObjectHolder";
}