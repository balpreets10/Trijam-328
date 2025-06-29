using System.Collections.Generic;

using UnityEngine;

public class CreateLevelDataAsset
{
    [ContextMenu("Create Level Data Asset")]
    public void CreateLevelDataAssets()
    {
        //        LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
        //        newLevelData.levelNumber = levelToGenerate;
        //        newLevelData.levelName = $"Level {levelToGenerate}";

        //        // Initialize with default values
        //        newLevelData.generationFormula = new LevelGenerationFormula();
        //        newLevelData.obstaclePatterns = new List<ObstaclePattern>();

        //#if UNITY_EDITOR
        //        UnityEditor.AssetDatabase.CreateAsset(newLevelData, $"Assets/LevelData_Level{levelToGenerate}.asset");
        //        UnityEditor.AssetDatabase.SaveAssets();
        //#endif
    }
}
