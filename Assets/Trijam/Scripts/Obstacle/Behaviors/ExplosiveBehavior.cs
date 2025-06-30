// Explosive Behavior - Damages player and destroys itself
using UnityEngine;

[System.Serializable]
public class ExplosiveBehavior : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionSound;

    public void Initialize(Obstacle obstacle)
    {
        obstacle.isDestructible = true;
    }

    public void OnPlayerCollision(Collider playerCollider)
    {
        Explode(playerCollider.transform.position);
    }

    public void OnPlayerTrigger(Collider playerCollider)
    {
        // Explosives can also trigger on proximity
        Explode(playerCollider.transform.position);
    }

    public void UpdateBehavior()
    {
        // No continuous behavior
    }

    private void Explode(Vector3 playerPosition)
    {
        // Spawn explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Play explosion sound
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Apply explosion force to nearby objects
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var obj in nearbyObjects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Destroy self
        GetComponent<Obstacle>().TryDestroy();
    }
}
