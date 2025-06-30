using System;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Obstacle : MonoBehaviour, IObstacle
{
    [Header("Basic Properties")]
    public float destructionValue = 10f;
    public bool isDestructible = true;
    public float healthReductionValue;

    [Header("UI Components")]
    public TextMeshProUGUI destructionText;
    public Canvas canvas;
    public Collider obstacleCollider;

    [Header("Obstacle Configuration")]
    public ObstacleType obstacleType;

    // Composition system - behavior component
    private IObstacleBehavior behaviorComponent;

    // Events
    public static event Action OnLevelCompleted;
    public static event Action<Obstacle> OnObstacleCleared;

    // Interface implementation
    GameObject IObstacle.gameObject { get => gameObject; set => Debug.Log("Can't set gameobject value"); }


    private void Awake()
    {
        // Get behavior component based on obstacle type
        behaviorComponent = GetBehaviorComponent();
    }
    public void Initialize()
    {
        if (canvas == null) canvas = GetComponentInChildren<Canvas>();
        if (destructionText == null) destructionText = GetComponentInChildren<TextMeshProUGUI>();
        if (obstacleCollider == null) obstacleCollider = GetComponentInChildren<Collider>();
        obstacleCollider.enabled = true;

        behaviorComponent?.Initialize(this);
    }

    private void Update()
    {
        // Let behavior component handle continuous updates
        behaviorComponent?.UpdateBehavior();
    }

    private IObstacleBehavior GetBehaviorComponent()
    {
        // Try to get existing behavior component first
        IObstacleBehavior behavior = GetComponent<IObstacleBehavior>();
        if (behavior != null) return behavior;

        // If no behavior component exists, add one based on obstacle type
        switch (obstacleType)
        {
            case ObstacleType.Wall:
                return gameObject.AddComponent<WallBehavior>();
            case ObstacleType.Barrier:
                return gameObject.AddComponent<BarrierBehavior>();
            case ObstacleType.Explosive:
                return gameObject.AddComponent<ExplosiveBehavior>();
            case ObstacleType.MovingBlock:
                return gameObject.AddComponent<MovingBlockBehavior>();
            case ObstacleType.Spike:
                return gameObject.AddComponent<SpikeBehavior>();
            case ObstacleType.PowerUp:
                return gameObject.AddComponent<PowerupBehavior>();
            case ObstacleType.HealthOrb:
                return gameObject.AddComponent<HealthOrbBehavior>();
            case ObstacleType.FinishLine:
                return gameObject.AddComponent<FinishLineBehavior>();
            default:
                return gameObject.AddComponent<WallBehavior>();
        }
    }

    public bool TryDestroy()
    {
        if (!isDestructible) return false;
        Disable();
        return true;
    }

    private void Disable()
    {
        gameObject.SetActive(false);
        obstacleCollider.enabled = false;

    }

    public float GetDestructionForce()
    {
        return destructionValue;
    }

    public float GetHealthReductionValue()
    {
        return healthReductionValue;
    }

    internal void UpdateUI()
    {
        if ((obstacleType == ObstacleType.FinishLine))
        {
            destructionText.text = "Finish";
            return;
        }
        destructionText.text = destructionValue.ToString("00");
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (obstacleType == ObstacleType.FinishLine && collision.collider.CompareTag("Player"))
        {
            Debug.Log("Level Completed!");
            OnLevelCompleted?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            behaviorComponent?.OnPlayerTrigger(other);
        }
    }

    public ObstacleType GetObstacleType()
    {
        return obstacleType;
    }

    public void SetObstacleType(ObstacleType obstacleType)
    {
        this.obstacleType = obstacleType;

        if (Application.isPlaying)
        {
            behaviorComponent = GetBehaviorComponent();
            behaviorComponent?.Initialize(this);
        }
    }
}

// Base interface for obstacle behaviors
public interface IObstacleBehavior
{
    void Initialize(Obstacle obstacle);
    void OnPlayerCollision(Collider playerCollider);
    void OnPlayerTrigger(Collider playerCollider);
    void UpdateBehavior();
}

