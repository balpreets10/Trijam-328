using System;
using System.Collections;

using UnityEngine;

public class PlayerController
{

    PlayerView view;
    PlayerData data;

    Coroutine healthCoroutine = null;

    public PlayerController(PlayerView view, PlayerData data)
    {
        this.view = view;
        this.data = data;
        data.Health = data.maxHealth;
        view.InitializePlayer(data);
    }

    public void HandleMovement(float horizontalInput, float verticalInput)
    {
        Vector3 movement = new Vector3(
            horizontalInput * data.MovementSpeed,
            0f,
            verticalInput * data.MovementSpeed
        );

        view.Move(movement);
    }

    public void OnCollisionWithObstacle(IObstacle obstacle)
    {
        if (obstacle.GetDestructionForce() < data.baseForce + data.addedForce)
        {
            obstacle.Destroy();
        }
        else
        {
            view.PushBack();
            ReduceHealth(obstacle);
            Debug.Log("Object is too strong");
        }
    }

    internal void AddForceAndConsumeHealth()
    {
        if (healthCoroutine != null)
        {
            view.StopCoroutine(healthCoroutine);
            healthCoroutine = null;
        }
        data.addedForce += data.addForceFactor;
        data.Health -= data.addForceFactor;
        view.UpdateForceText(data.baseForce + data.addedForce);
        view.UpdateHealthSlider(-data.addForceFactor);
    }

    internal void ResetAddedForce()
    {
        data.addedForce = 0;
        view.UpdateForceText(data.baseForce);
        healthCoroutine = view.StartCoroutine(RegenerateHealth());
    }

    private IEnumerator RegenerateHealth()
    {
        while (data.Health < data.maxHealth)
        {
            data.Health += data.addForceFactor;
            view.UpdateHealthSlider(data.addForceFactor);
            yield return null;
        }
    }

    private void ReduceHealth(IObstacle obstacle)
    {
        data.ReduceHealth(obstacle.GetHealthReductionValue());
        view.UpdateHealthSlider(-obstacle.GetHealthReductionValue(), false);
    }
}
