using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Obstacle : MonoBehaviour, IObstacle
{
    public float destructionForce = 2;
    public float healthReductionValue;

    GameObject IObstacle.gameObject { get => gameObject; set => Debug.Log("Can't set gameobject value"); }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }

    public float GetDestructionForce()
    {
        return destructionForce;
    }

    public float GetHealthReductionValue()
    {
        return healthReductionValue;
    }
}
