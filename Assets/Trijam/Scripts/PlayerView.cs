using System;

using DG.Tweening;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerView : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody rb;
    [SerializeField] public GameObject myCamera;

    [Header("UI Elements")]
    public TextMeshProUGUI currentForce;
    public Slider healthSlider;

    [Header("Movement Constraints")]
    [SerializeField] private float maxHorizontalPosition = 5f;
    [SerializeField] private float minHorizontalPosition = -5f;

    private PlayerController controller;
    private PlayerData data;
    private bool canMoveForward = false;
    // Track shift key state to handle press/release properly
    private bool wasShiftPressed = false;


    // Cache the transform to avoid repeated GetComponent calls
    private Transform cachedTransform;

    public static UnityAction<Camera> OnPlayerCameraActivated;

    // Property to check if forward movement is enabled
    public bool CanMoveForward => canMoveForward;

    private Vector3 initialPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cachedTransform = transform;
        initialPosition = transform.localPosition;
    }

    public void InitializePlayer(PlayerController controller, PlayerData data)
    {
        this.controller = controller;
        this.data = data;

        // Initialize UI
        ResetPlayerView(data);
        // Reset forward movement permission
        canMoveForward = false;
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        // Handle force reset input
        if (controller?.GetPlayerState() != PlayerState.Idle && Input.GetKeyUp(KeyCode.LeftShift))
        {
            controller?.ResetAddedForce();
        }
        HandleShiftInput();
    }

    private void HandleShiftInput()
    {
        bool isShiftCurrentlyPressed = Input.GetKey(KeyCode.LeftShift);

        // Detect shift press (transition from not pressed to pressed)
        if (isShiftCurrentlyPressed && !wasShiftPressed)
        {
            controller?.OnShiftPressed();
            wasShiftPressed = true;
        }
        // Detect shift release (transition from pressed to not pressed)
        else if (!isShiftCurrentlyPressed && wasShiftPressed)
        {
            controller?.OnShiftReleased();
            wasShiftPressed = false;
        }
    }

    private void HandleMovement()
    {
        PlayerState currentState = controller?.GetPlayerState() ?? PlayerState.Idle;

        switch (currentState)
        {
            case PlayerState.Moving:
                HandleMovingState();
                break;
            case PlayerState.Pushback:
                HandlePushbackState();
                break;
            case PlayerState.Collision:
                // No movement during collision
                break;
        }
    }



    private void HandleMovingState()
    {
        float horizontalInput = GetHorizontalMovement();
        float verticalInput = canMoveForward ? 1f : 0f;
        controller.HandleMovement(horizontalInput, verticalInput);

        // Handle force addition input
        if (Input.GetKey(KeyCode.LeftShift))
        {
            controller.AddForceAndConsumeHealth();
        }
    }

    private void HandlePushbackState()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            controller.AddForceAndConsumeHealth();
        }
        float horizontalInput = GetHorizontalMovement();
        DisableForwardMovement();
        controller.HandleMovement(horizontalInput, 0);
    }

    internal float GetHorizontalMovement()
    {
        return Input.GetAxis("Horizontal");
    }

    private void OnCollisionEnter(Collision collision)
    {
        IObstacle obstacle = collision.gameObject.GetComponent<IObstacle>();
        if (obstacle != null)
        {
            controller.OnCollisionWithObstacle(obstacle);
        }
    }

    public void Move(Vector3 movement)
    {
        // Apply horizontal position constraints
        Vector3 newPosition = cachedTransform.position + movement * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minHorizontalPosition, maxHorizontalPosition);

        // Use velocity for smooth movement with clamped horizontal position
        Vector3 clampedMovement = new Vector3(
            (newPosition.x - cachedTransform.position.x) / Time.deltaTime,
            0f,
            movement.z
        );

        rb.linearVelocity = new Vector3(clampedMovement.x, rb.linearVelocity.y, clampedMovement.z);
    }

    internal void UpdateHealthSlider(float value, bool instant = true)
    {
        if (instant)
        {
            healthSlider.value = Mathf.Clamp(healthSlider.value + value, 0, healthSlider.maxValue);
        }
        else
        {
            float targetValue = Mathf.Clamp(healthSlider.value + value, 0, healthSlider.maxValue);
            healthSlider.DOValue(targetValue, 0.5f).SetEase(Ease.Linear);
        }

        // Debug to track UI updates
        //Debug.Log($"Health UI updated: {healthSlider.value}/{healthSlider.maxValue}");
    }

    internal void PushBack(Action onComplete)
    {
        cachedTransform.DOMoveZ(cachedTransform.position.z - data.pushbackDistance, 1f, false)
            .OnComplete(() => onComplete?.Invoke());
    }

    internal void UpdateForceText(float force)
    {
        currentForce.text = force.ToString("00.00");
    }

    internal void Deactivate()
    {
        gameObject.SetActive(false);
        wasShiftPressed = false;
    }

    internal void Activate()
    {
        gameObject.SetActive(true);
        transform.localPosition = initialPosition;
    }

    internal void UpdateCamera(bool activate)
    {
        myCamera.SetActive(activate);
    }

    // Method to enable forward movement (call this when your timer completes)
    public void EnableForwardMovement()
    {
        canMoveForward = true;
    }

    // Method to disable forward movement
    public void DisableForwardMovement()
    {
        canMoveForward = false;
    }

    internal void ResetPlayerView(PlayerData data)
    {
        UpdateForceText(data.baseForce);
        healthSlider.maxValue = data.maxHealth;
        UpdateHealthSlider(data.maxHealth);
    }

    //internal void UpdateCamera()
    //{
    //    myCamera.SetActive(true);
    //    OnPlayerCameraActivated?.Invoke(myCamera.GetComponent<Camera>());
    //}


}

public enum PlayerState
{
    Idle, Moving, Collision, Pushback
}