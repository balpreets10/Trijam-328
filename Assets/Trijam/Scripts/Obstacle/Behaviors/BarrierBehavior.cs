using UnityEngine;

[System.Serializable]
public class BarrierBehavior : MonoBehaviour, IObstacleBehavior
{
    public void Initialize(Obstacle obstacle)
    {
        obstacle.isDestructible = false;
    }

    public void OnPlayerCollision(Collider playerCollider)
    {
        // Block player movement - handled by physics
    }

    public void OnPlayerTrigger(Collider playerCollider)
    {
        // Barriers don't use triggers
    }

    public void UpdateBehavior()
    {
        // No continuous behavior for barriers
    }
}