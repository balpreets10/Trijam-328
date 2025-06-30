using UnityEngine;

[System.Serializable]
public class WallBehavior : MonoBehaviour, IObstacleBehavior
{
    public void Initialize(Obstacle obstacle)
    {
        // Wall is just a basic destructible obstacle - no special behavior needed
    }

    public void OnPlayerCollision(Collider playerCollider)
    {
        // Handled by main obstacle collision logic
    }

    public void OnPlayerTrigger(Collider playerCollider)
    {
        // Walls don't use triggers
    }

    public void UpdateBehavior()
    {
        // No continuous behavior for walls
    }
}
