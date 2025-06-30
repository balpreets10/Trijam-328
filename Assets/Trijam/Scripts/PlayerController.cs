using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
[Serializable]
public class PlayerController : IPlayerController
{

    private PlayerView view;
    private PlayerData data;
    private Coroutine healthCoroutine = null;
    private Coroutine forceDecayCoroutine = null;
    private float currentAddValue = 0;
    private readonly float maxAddValue = 1f;
    private PlayerState playerState = PlayerState.Idle;

    // Force boost mechanism
    private float currentBoostForce = 0f;
    private bool isBoostActive = false;
    private bool isShiftHeld = false; // Track shift key state


    public static UnityAction OnPlayerDied;

    public PlayerController(PlayerView view, PlayerData data)
    {
        this.view = view;
        this.data = data;
        // Initialize health properly
        data.health = data.maxHealth;
        ResetPlayer(data);
        view.InitializePlayer(this, data);
    }


    internal void ResetPlayer(PlayerData data)
    {
        this.data = data;
        healthCoroutine = null;
        forceDecayCoroutine = null;
        currentAddValue = 0;
        currentBoostForce = 0f;
        isBoostActive = false;
        isShiftHeld = false;
        view.ResetPlayerView(data);
        SetState(PlayerState.Idle);
    }


    public float GetCurrentForce()
    {
        //return data.baseForce + ((100f - data.health) * data.healthToPowerRatio);
        return data.baseForce + data.addedForce + currentBoostForce;
    }

    public void StartPlayer()
    {
        view.UpdateCamera(true);
        view.EnableForwardMovement();
        SetState(PlayerState.Moving);
        UpdateForceDisplay();
    }

    public void Activate()
    {
        view.Activate();
        view.UpdateCamera(false);
    }

    public void Deactivate()
    {
        view.UpdateCamera(false);
        view.Deactivate();
        SetState(PlayerState.Idle);
        ResetPlayer(data);
    }

    public void HandleMovement(float horizontalInput, float verticalInput)
    {
        Vector3 movement = new Vector3(
            horizontalInput * data.movementSpeed,
            0f,
            verticalInput * data.movementSpeed
        );

        view.Move(movement);
    }

    public void OnCollisionWithObstacle(IObstacle obstacle)
    {
        Debug.Log($"Colliding with:", obstacle.gameObject);
        if (obstacle.GetDestructionForce() < GetCurrentForce())
        {
            obstacle.TryDestroy();
        }
        else
        {
            HandleObstacleCollision(obstacle);
        }
    }

    private void HandleObstacleCollision(IObstacle obstacle)
    {
        SetState(PlayerState.Pushback);
        view.PushBack(OnPushBackComplete);
        ReduceHealth(obstacle);
        Debug.Log($"Object is too strong - Required: {obstacle.GetDestructionForce()}, Available: {GetCurrentForce()}", obstacle.gameObject);
    }

    private void OnPushBackComplete()
    {
        view.EnableForwardMovement();
        SetState(PlayerState.Moving);
    }

    public void OnShiftPressed()
    {
        if (isShiftHeld) return; // Prevent multiple calls

        isShiftHeld = true;
        // Stop any ongoing decay when shift is pressed
        StopForceDecay();
        Debug.Log("Shift pressed - starting force boost");
    }

    public void OnShiftReleased()
    {
        if (!isShiftHeld) return; // Prevent multiple calls

        isShiftHeld = false;
        currentAddValue = 0; // Reset the add value for next boost
        Debug.Log($"Shift released - starting force decay - {GetCurrentForce():00.00}");

        if (isBoostActive && currentBoostForce > 0)
        {
            StartForceDecay();
        }
    }

    public void AddForceAndConsumeHealth()
    {
        if (!isShiftHeld) return; // Only add force if shift is being held
        if (data.health <= 10) return;

        StopHealthRegeneration();
        StopForceDecay(); // Stop any ongoing force decay

        if (currentAddValue < maxAddValue)
        {
            currentAddValue += data.addForceFactor;
            currentAddValue = Mathf.Min(currentAddValue, maxAddValue);
        }

        currentBoostForce += currentAddValue;
        isBoostActive = true;

        // Consume health and update UI immediately
        ConsumeHealth(currentAddValue);
        UpdateForceDisplay();

        Debug.Log($"Added Force: {data.addedForce}, Current Add Value: {currentAddValue}, Current Health: {data.health}");
    }

    public void OnForceBoostReleased()
    {
        if (isBoostActive)
        {
            // Start the boost decay process
            StartForceDecay();
        }
        currentAddValue = 0; // Reset the add value for next boost
    }

    private void StartForceDecay()
    {
        StopForceDecay(); // Ensure no duplicate coroutines
        forceDecayCoroutine = view.StartCoroutine(ForceDecayRoutine());
    }

    private void StopForceDecay()
    {
        if (forceDecayCoroutine != null)
        {
            view.StopCoroutine(forceDecayCoroutine);
            forceDecayCoroutine = null;
        }
    }

    private IEnumerator ForceDecayRoutine()
    {
        // Wait for the boost duration (1 second)
        yield return new WaitForSeconds(data.boostDuration);

        // Only continue decay if shift is not being held again
        if (isShiftHeld)
        {
            Debug.Log("Shift pressed during decay - cancelling decay");
            forceDecayCoroutine = null;
            yield break;
        }

        // Rapidly decay the force back to base levels
        while (currentBoostForce > 0 && !isShiftHeld)
        {
            currentBoostForce -= data.forceDecaySpeed * Time.deltaTime;
            currentBoostForce = Mathf.Max(0, currentBoostForce);
            UpdateForceDisplay();
            yield return null;
        }

        // If shift was pressed during decay, don't reset everything
        if (isShiftHeld)
        {
            Debug.Log("Shift pressed during decay - maintaining boost state");
            forceDecayCoroutine = null;
            yield break;
        }

        // Clean up only if decay completed naturally
        currentBoostForce = 0;
        isBoostActive = false;
        forceDecayCoroutine = null;
        UpdateForceDisplay();

        Debug.Log("Force boost ended, back to base levels");

        // Start health regeneration after boost completely ends
        StartHealthRegeneration();
    }

    private void UpdateForceDisplay()
    {
        float totalForce = GetCurrentForce();
        view.UpdateForceText(totalForce);
    }

    public void ResetAddedForce()
    {
        // This method can be renamed to ResetPermanentForce if needed
        data.addedForce = 0;
        UpdateForceDisplay();
        StartHealthRegeneration();
    }

    private void StopHealthRegeneration()
    {
        if (healthCoroutine != null)
        {
            view.StopCoroutine(healthCoroutine);
            healthCoroutine = null;
        }
    }

    private void StartHealthRegeneration()
    {
        StopHealthRegeneration(); // Ensure no duplicate coroutines
        healthCoroutine = view.StartCoroutine(RegenerateHealth());
    }

    private IEnumerator RegenerateHealth()
    {
        while (data.health < data.maxHealth)
        {
            float healthToAdd = Mathf.Min(data.addForceFactor, data.maxHealth - data.health);
            data.health += healthToAdd;
            view.UpdateHealthSlider(healthToAdd);
            yield return new WaitForEndOfFrame();
        }
        healthCoroutine = null; // Clean up reference
    }

    private void ReduceHealth(IObstacle obstacle)
    {
        float healthReduction = obstacle.GetHealthReductionValue();
        ConsumeHealth(healthReduction);
        //view.UpdateHealthSlider(-healthReduction, false);
    }

    private void ConsumeHealth(float amount)
    {
        data.health = Mathf.Max(0, data.health - amount);
        if (data.health <= 1)
        {
            OnPlayerDied.Invoke();
            return;
        }
        view.UpdateHealthSlider(-amount, true);
        Debug.Log($"Health consumed: {amount}, Current health: {data.health}");
    }

    public PlayerState GetPlayerState()
    {
        return playerState;
    }

    public void SetState(PlayerState state)
    {
        if (playerState != state)
        {
            playerState = state;
            Debug.Log($"Player state changed to: {state}");
        }
    }

    public bool ConsumeHealthForPower(float healthAmount)
    {
        if (data.health > healthAmount)
        {
            ConsumeHealth(healthAmount);
            return true;
        }
        return false;
    }

    // Helper method to get current shift state (for debugging)
    public bool IsShiftHeld()
    {
        return isShiftHeld;
    }

    public GameObject GetMyCamera()
    {
        return view.myCamera;
    }
}

public interface IPlayerController
{
    void OnCollisionWithObstacle(IObstacle obstacle);
    void AddForceAndConsumeHealth();
    void HandleMovement(float horizontalInput, float verticalInput);

    void Activate();

    GameObject GetMyCamera();
}

public interface IPlayerHealth
{
    void TakeDamage(float amount);
    void Heal(float amount);
}

public interface IPlayerPowerUps
{
    void ApplySpeedBoost(float multiplier, float duration);
    void ApplyStrengthBoost(float multiplier, float duration);
    void ApplyShield(float duration);
    void ApplyScoreMultiplier(float multiplier, float duration);
}