using UnityEngine;

[System.Serializable]
public class HealthOrbBehavior : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private float healAmount = 25f;
    [SerializeField] private GameObject healEffect;
    [SerializeField] private AudioClip healSound;

    public void Initialize(Obstacle obstacle)
    {
        obstacle.isDestructible = true;
        obstacle.destructionValue = 0f; // Health orbs don't require force to collect
    }

    public void OnPlayerCollision(Collider playerCollider)
    {
        HealPlayer(playerCollider);
    }

    public void OnPlayerTrigger(Collider playerCollider)
    {
        HealPlayer(playerCollider);
    }

    public void UpdateBehavior()
    {
        // Float up and down for visual appeal
        float newY = Mathf.Sin(Time.time * 2f) * 0.5f;
        transform.position = new Vector3(transform.position.x, transform.position.y + newY * Time.deltaTime, transform.position.z);
    }

    private void HealPlayer(Collider playerCollider)
    {
        var playerHealth = playerCollider.GetComponent<IPlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);

            // Spawn heal effect
            if (healEffect != null)
            {
                Instantiate(healEffect, transform.position, Quaternion.identity);
            }

            // Play heal sound
            if (healSound != null)
            {
                AudioSource.PlayClipAtPoint(healSound, transform.position);
            }

            // Destroy self
            GetComponent<Obstacle>().TryDestroy();
        }
    }
}