// Finish Line Behavior - Completes the level
using UnityEngine;

[System.Serializable]
public class FinishLineBehavior : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private GameObject celebrationEffect;
    [SerializeField] private AudioClip victorySound;

    public void Initialize(Obstacle obstacle)
    {
        obstacle.isDestructible = false;
        obstacle.destructionValue = 0f;
    }

    public void OnPlayerCollision(Collider playerCollider)
    {
        CompleteLevel(playerCollider);
    }

    public void OnPlayerTrigger(Collider playerCollider)
    {
        CompleteLevel(playerCollider);
    }

    public void UpdateBehavior()
    {
        // Finish line visual effects (optional)
    }

    private void CompleteLevel(Collider playerCollider)
    {
        if (playerCollider.CompareTag("Player"))
        {
            // Spawn celebration effect
            if (celebrationEffect != null)
            {
                Instantiate(celebrationEffect, transform.position, Quaternion.identity);
            }

            // Play victory sound
            if (victorySound != null)
            {
                AudioSource.PlayClipAtPoint(victorySound, transform.position);
            }

            // Trigger level completion
            //Obstacle.OnLevelCompleted?.Invoke();
        }
    }
}