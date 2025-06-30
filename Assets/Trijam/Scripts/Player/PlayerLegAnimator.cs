using UnityEngine;
using DG.Tweening;

public class PlayerLegAnimator : MonoBehaviour
{
    [Header("Leg References")]
    [SerializeField] private Transform leftLeg;
    [SerializeField] private Transform rightLeg;

    [Header("Animation Settings")]
    [SerializeField] private float walkStepHeight = 0.3f;
    [SerializeField] private float walkStepDuration = 0.25f;
    [SerializeField] private float boostStepHeight = 0.6f;
    [SerializeField] private float boostStepDuration = 0.15f;
    [SerializeField] private float boostShakeIntensity = 0.1f;

    [Header("Leg Positioning")]
    [SerializeField] private Vector3 leftLegOffset = new Vector3(-0.3f, -0.8f, 0f);
    [SerializeField] private Vector3 rightLegOffset = new Vector3(0.3f, -0.8f, 0f);
    [SerializeField] private Vector3 legScale = new Vector3(0.4f, 0.6f, 0.4f);

    // Private variables
    private Vector3 leftLegInitialPos;
    private Vector3 rightLegInitialPos;
    private bool isWalking = false;
    private bool isBoosting = false;
    private Sequence walkSequence;
    private Sequence boostSequence;
    private PlayerController playerController;

    private void Awake()
    {
        SetupLegs();
    }

    public void SetupController(PlayerController controller)
    {
        // Get reference to player controller
        playerController = controller;
    }
    private void Start()
    {

        // Store initial positions AFTER setup is complete
        leftLegInitialPos = leftLegOffset;
        rightLegInitialPos = rightLegOffset;

        // Ensure legs are at correct initial positions
        leftLeg.localPosition = leftLegInitialPos;
        rightLeg.localPosition = rightLegInitialPos;
    }

    private void SetupLegs()
    {
        // Create leg GameObjects if they don't exist
        if (leftLeg == null)
        {
            GameObject leftLegGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftLegGO.name = "LeftLeg";
            leftLegGO.transform.SetParent(transform);
            leftLegGO.transform.localPosition = leftLegOffset;
            leftLegGO.transform.localScale = legScale;
            leftLeg = leftLegGO.transform;

            // Remove collider to avoid interference
            Destroy(leftLegGO.GetComponent<Collider>());
        }

        if (rightLeg == null)
        {
            GameObject rightLegGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightLegGO.name = "RightLeg";
            rightLegGO.transform.SetParent(transform);
            rightLegGO.transform.localPosition = rightLegOffset;
            rightLegGO.transform.localScale = legScale;
            rightLeg = rightLegGO.transform;

            // Remove collider to avoid interference
            Destroy(rightLegGO.GetComponent<Collider>());
        }
    }

    public void StartWalking()
    {
        if (isWalking || isBoosting) return;

        isWalking = true;
        PlayWalkAnimation();
    }

    public void StopWalking()
    {
        isWalking = false;

        if (walkSequence != null)
        {
            walkSequence.Kill();
            walkSequence = null;
        }

        // Return legs to initial position using local coordinates
        leftLeg.DOLocalMove(leftLegInitialPos, 0.2f).SetEase(Ease.OutQuad);
        rightLeg.DOLocalMove(rightLegInitialPos, 0.2f).SetEase(Ease.OutQuad);
    }

    public void StartBoostAnimation()
    {
        if (isBoosting) return;

        isBoosting = true;

        // Stop walk animation if running
        if (walkSequence != null)
        {
            walkSequence.Kill();
            walkSequence = null;
        }

        PlayBoostAnimation();
    }

    public void StopBoostAnimation()
    {
        isBoosting = false;

        if (boostSequence != null)
        {
            boostSequence.Kill();
            boostSequence = null;
        }

        // Return legs to correct positions
        leftLeg.localPosition = leftLegInitialPos;
        rightLeg.localPosition = rightLegInitialPos;

        // Return to walking if still moving, otherwise stop
        if (isWalking)
        {
            PlayWalkAnimation();
        }
        else
        {
            StopWalking();
        }
    }

    private void PlayWalkAnimation()
    {
        if (walkSequence != null)
        {
            walkSequence.Kill();
        }

        walkSequence = DOTween.Sequence();

        // Left leg step
        walkSequence.Append(leftLeg.DOLocalMoveY(leftLegInitialPos.y + walkStepHeight, walkStepDuration / 2))
                   .Append(leftLeg.DOLocalMoveY(leftLegInitialPos.y, walkStepDuration / 2))

                   // Right leg step (simultaneous with left leg return)
                   .Insert(walkStepDuration, rightLeg.DOLocalMoveY(rightLegInitialPos.y + walkStepHeight, walkStepDuration / 2))
                   .Insert(walkStepDuration + walkStepDuration / 2, rightLeg.DOLocalMoveY(rightLegInitialPos.y, walkStepDuration / 2))

                   .SetLoops(-1, LoopType.Restart)
                   .SetEase(Ease.InOutQuad);
    }

    private void PlayBoostAnimation()
    {
        if (boostSequence != null)
        {
            boostSequence.Kill();
        }

        boostSequence = DOTween.Sequence();

        // Rapid alternating leg movement with shake effect
        boostSequence.Append(leftLeg.DOLocalMoveY(leftLegInitialPos.y + boostStepHeight, boostStepDuration / 2))
                    .Join(leftLeg.DOShakePosition(boostStepDuration / 2, boostShakeIntensity, 10, 90, false, true))
                    .Append(leftLeg.DOLocalMoveY(leftLegInitialPos.y, boostStepDuration / 2))

                    .Insert(boostStepDuration / 2, rightLeg.DOLocalMoveY(rightLegInitialPos.y + boostStepHeight, boostStepDuration / 2))
                    .Insert(boostStepDuration / 2, rightLeg.DOShakePosition(boostStepDuration / 2, boostShakeIntensity, 10, 90, false, true))
                    .Insert(boostStepDuration, rightLeg.DOLocalMoveY(rightLegInitialPos.y, boostStepDuration / 2))

                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.InOutBack);
    }

    private void Update()
    {
        // Check if player is moving and update walk state accordingly
        if (playerController != null)
        {
            PlayerState currentState = playerController.GetPlayerState();
            bool shouldWalk = (currentState == PlayerState.Moving || currentState == PlayerState.Pushback);

            if (shouldWalk && !isWalking && !isBoosting)
            {
                StartWalking();
            }
            else if (!shouldWalk && isWalking && !isBoosting)
            {
                StopWalking();
            }
        }
    }

    // Call this method when boost starts (from PlayerController or PlayerView)
    public void OnBoostStart()
    {
        StartBoostAnimation();
    }

    // Call this method when boost ends (from PlayerController or PlayerView)
    public void OnBoostEnd()
    {
        StopBoostAnimation();
    }

    // Method to reset legs to initial positions (useful for debugging)
    public void ResetLegPositions()
    {
        // Kill any ongoing animations
        if (walkSequence != null)
        {
            walkSequence.Kill();
            walkSequence = null;
        }
        if (boostSequence != null)
        {
            boostSequence.Kill();
            boostSequence = null;
        }

        // Reset positions
        leftLeg.localPosition = leftLegInitialPos;
        rightLeg.localPosition = rightLegInitialPos;

        isWalking = false;
        isBoosting = false;
    }

    private void OnDestroy()
    {
        // Clean up sequences
        if (walkSequence != null)
        {
            walkSequence.Kill();
        }
        if (boostSequence != null)
        {
            boostSequence.Kill();
        }
    }
}