using UnityEngine;

public interface IObstacle
{
    void Destroy();
    float GetDestructionForce();
    float GetHealthReductionValue();

    GameObject gameObject { get; set; }
}
