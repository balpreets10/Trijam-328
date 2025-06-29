using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Obstacle : MonoBehaviour, IObstacle
{
    public float destructionValue = 10f;
    public bool isDestructible = true;
    public float healthReductionValue;
    public TextMeshProUGUI destructionText;
    public Canvas canvas;
    public Collider obstacleCollider;

    public ObstacleType obstacleType;
    public static event System.Action OnLevelCompleted;
    GameObject IObstacle.gameObject { get => gameObject; set => Debug.Log("Can't set gameobject value"); }

    public void Initialize()
    {
        if (canvas == null) canvas = GetComponentInChildren<Canvas>();
        if (destructionText == null) destructionText = GetComponentInChildren<TextMeshProUGUI>();
        if (obstacleCollider == null) obstacleCollider = GetComponentInChildren<Collider>();
        obstacleCollider.enabled = true;
        AddListeners();
    }
    public bool TryDestroy()
    {
        if (!isDestructible) return false;
        obstacleCollider.enabled = false;
        gameObject.SetActive(false);

        return true;
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
        destructionText.text = destructionValue.ToString("00.0");
    }

    internal void SetupCamera(Camera camera)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Reset the canvas transform relative to parent
        canvasRect.localPosition = Vector3.zero;
        canvasRect.localRotation = Quaternion.identity;

        // Set appropriate scale (World Space canvases are huge by default)
        canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Position it slightly in front of the object
        canvasRect.localPosition = new Vector3(0, 0, -1f);

        // Make it face the camera
        canvas.worldCamera = camera;
    }

    private void AddListeners()
    {
        PlayerView.OnPlayerCameraActivated += OnCameraActivated;
    }

    private void OnCameraActivated(Camera camera)
    {
        SetupCamera(camera);
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (obstacleType == ObstacleType.FinishLine && collision.collider.CompareTag("Player"))
        {
            Debug.Log("Level Completed!");
            OnLevelCompleted?.Invoke();
        }
    }

    public ObstacleType GetObstacleType()
    {
        return obstacleType;
    }

    public void SetObstacleType(ObstacleType obstacleType)
    {
        this.obstacleType = obstacleType;
    }
}

