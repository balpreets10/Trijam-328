using UnityEngine;

[System.Serializable]
public class SpikeBehavior : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private bool canRetractSpikes = false;
    [SerializeField] private float retractTime = 2f;

    private float lastDamageTime;
    private bool spikesExtended = true;

    public void Initialize(Obstacle obstacle)
    {
        obstacle.isDestructible = false; // Spikes are usually indestructible
        obstacle.healthReductionValue = damageAmount;
    }

    public void OnPlayerCollision(Collider playerCollider)
    {
        if (spikesExtended && Time.time - lastDamageTime > damageInterval)
        {
            // Damage player
            var playerHealth = playerCollider.GetComponent<IPlayerHealth>();
            playerHealth?.TakeDamage(damageAmount);
            lastDamageTime = Time.time;
        }
    }

    public void OnPlayerTrigger(Collider playerCollider)
    {
        OnPlayerCollision(playerCollider);
    }

    public void UpdateBehavior()
    {
        if (canRetractSpikes)
        {
            // Simple spike retraction logic
            if (Time.time % (retractTime * 2f) > retractTime)
            {
                spikesExtended = false;
            }
            else
            {
                spikesExtended = true;
            }
        }
    }
}