// Moving Block Behavior - Moves along a path
using UnityEngine;

[System.Serializable]
public class MovingBlockBehavior : MonoBehaviour, IObstacleBehavior
{
    [SerializeField] private Vector3[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool loopPath = true;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private int currentWaypointIndex = 0;
    private Vector3 startPosition;
    private bool movingForward = true;

    public void Initialize(Obstacle obstacle)
    {
        startPosition = transform.position;
        if (waypoints == null || waypoints.Length == 0)
        {
            waypoints = new Vector3[] { Vector3.zero, Vector3.right * 5f };
        }
    }

    public void OnPlayerCollision(Collider playerCollider)
    {
        // Standard obstacle collision behavior
    }

    public void OnPlayerTrigger(Collider playerCollider)
    {
        // Moving blocks don't use triggers typically
    }

    public void UpdateBehavior()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (waypoints.Length == 0) return;

        Vector3 targetPosition = startPosition + waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (loopPath)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            else
            {
                if (movingForward)
                {
                    currentWaypointIndex++;
                    if (currentWaypointIndex >= waypoints.Length)
                    {
                        currentWaypointIndex = waypoints.Length - 2;
                        movingForward = false;
                    }
                }
                else
                {
                    currentWaypointIndex--;
                    if (currentWaypointIndex < 0)
                    {
                        currentWaypointIndex = 1;
                        movingForward = true;
                    }
                }
            }
        }
    }
}