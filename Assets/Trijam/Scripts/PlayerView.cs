using System;

using DG.Tweening;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerView : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerController controller;
    [SerializeField] private PlayerData data;
    public TextMeshProUGUI currentForce;
    public Slider healthSlider;
    float verticalInput = 0;
    PlayerState playerState = PlayerState.Moving;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //data = new PlayerData(); // You might want to inject this from elsewhere
        controller = new PlayerController(this, data);
    }

    public void InitializePlayer(PlayerData data)
    {
        healthSlider.SetValueWithoutNotify(data.Health);
        healthSlider.maxValue = data.Health;
        currentForce.text = data.baseForce.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            controller.ResetAddedForce();
        }
        switch (playerState)
        {
            case PlayerState.Moving:
                {
                    while (verticalInput < 0.999f)
                        verticalInput += 0.01f;
                    controller.HandleMovement(GetHorizontalMovement(), verticalInput);
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        controller.AddForceAndConsumeHealth();
                    }
                    break;
                }
            case PlayerState.Collision:
                {
                    break;
                }
            case PlayerState.Pushback:
                {
                    controller.HandleMovement(GetHorizontalMovement(), 0);
                    break;
                }
        }

    }

    internal float GetHorizontalMovement()
    {
        return Input.GetAxis("Horizontal");
    }

    private void OnCollisionEnter(Collision collision)
    {
        IObstacle obstacle = collision.gameObject.GetComponent<IObstacle>();
        if (obstacle != null)
            controller.OnCollisionWithObstacle(collision.gameObject.GetComponent<IObstacle>());
    }

    public void Move(Vector3 movement)
    {
        // Using velocity for smooth movement
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }

    internal void UpdateHealthSlider(float v, bool instant = true)
    {
        if (instant)
        {
            healthSlider.value += v;
        }
        else
        {
            float initialValue = healthSlider.value;
            healthSlider.DOValue(healthSlider.value + v, 0.5f).SetEase(Ease.Linear);
        }
    }

    internal void PushBack()
    {
        SetState(PlayerState.Pushback);
        transform.DOMoveZ(transform.position.z - data.pushbackDistance, 1f, false).OnComplete(() => SetState(PlayerState.Moving));
    }

    internal void SetState(PlayerState state)
    {
        playerState = state;
    }

    internal void UpdateForceText(float force)
    {
        currentForce.text = force.ToString("00.00");
    }
}

public enum PlayerState
{
    Moving, Collision, Pushback
}