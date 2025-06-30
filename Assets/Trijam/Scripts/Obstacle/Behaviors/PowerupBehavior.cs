using UnityEngine;

[System.Serializable]
public class PowerupBehavior : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private float powerUpValue = 10f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectSound;

    public enum PowerUpType
    {
        SpeedBoost,
        StrengthBoost,
        Shield,
        DoubleScore
    }

    public void Initialize(Obstacle obstacle)
    {
        obstacle.isDestructible = true;
        obstacle.destructionValue = 0f; // PowerUps don't require force to collect
    }

    public void OnPlayerCollision(Collider playerCollider)
    {
        CollectPowerUp(playerCollider);
    }

    public void OnPlayerTrigger(Collider playerCollider)
    {
        CollectPowerUp(playerCollider);
    }

    public void UpdateBehavior()
    {
        // Rotate for visual appeal
        transform.Rotate(0, 90f * Time.deltaTime, 0);
    }

    private void CollectPowerUp(Collider playerCollider)
    {
        // Apply power-up effect to player
        var playerPowerUps = playerCollider.GetComponent<IPlayerPowerUps>();
        if (playerPowerUps != null)
        {
            switch (powerUpType)
            {
                case PowerUpType.SpeedBoost:
                    playerPowerUps.ApplySpeedBoost(powerUpValue, duration);
                    break;
                case PowerUpType.StrengthBoost:
                    playerPowerUps.ApplyStrengthBoost(powerUpValue, duration);
                    break;
                case PowerUpType.Shield:
                    playerPowerUps.ApplyShield(duration);
                    break;
                case PowerUpType.DoubleScore:
                    playerPowerUps.ApplyScoreMultiplier(2f, duration);
                    break;
            }
        }

        // Spawn collection effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Play collection sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Destroy self
        GetComponent<Obstacle>().TryDestroy();
    }
}