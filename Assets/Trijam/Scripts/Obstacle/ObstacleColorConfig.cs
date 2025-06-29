using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleColorConfig", menuName = "Game/Obstacle Color Config")]
public class ObstacleColorConfig : ScriptableObject
{
    [Header("Obstacle Colors by Difficulty")]
    [Tooltip("Color for easily beatable obstacles (ratio < 0.5)")]
    public Color easyColor = Color.green;

    [Tooltip("Color for medium difficulty obstacles (ratio 0.5-0.8)")]
    public Color mediumColor = Color.yellow;

    [Tooltip("Color for challenging obstacles (ratio 0.8-1.1)")]
    public Color challengingColor = Color.magenta;

    [Tooltip("Color for very hard obstacles (ratio > 1.1)")]
    public Color hardColor = Color.red;
    [Tooltip("Color for finish line")]
    public Color finishColor = Color.black;

    [Header("Color Thresholds")]
    [Range(0f, 1f)]
    public float easyThreshold = 0.5f;

    [Range(0f, 2f)]
    public float mediumThreshold = 0.8f;

    [Range(0f, 2f)]
    public float challengingThreshold = 1.1f;

    public Color GetObstacleColor(float destructionValue, float recommendedForce)
    {
        float ratio = destructionValue / recommendedForce;

        if (ratio < easyThreshold) return easyColor;
        if (ratio < mediumThreshold) return mediumColor;
        if (ratio < challengingThreshold) return challengingColor;
        return hardColor;
    }
}