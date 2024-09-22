using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ButterflyController : MonoBehaviour
{
    // Movement parameters
    [Header("Movement Settings")]
    public float speed = 5f;
    public float turnSpeed = 100f;
    public float pitchSpeed = 50f;
    public float maxHoverAmplitude = 0.3f;
    public float maxPitch = 45f;
    public bool controllable = true;

    // Wing & Body Transforms
    [Header("Wing & Body Transforms")]
    public Transform leftWing;
    public Transform rightWing;
    public Transform body;
    public Transform model;

    // Wing animation
    [Header("Wing Animation")]
    public float maxWingFlapSpeed = 10f;
    public float wingFlapAngle = 30f;
    public AnimationCurve wingFlapCurve;
    public float wingRotationAmplitude = 20f;

    // Wind and Jitter
    [Header("Wind and Jitter Settings")]
    public float jitterStrength = 0.1f;
    public float jitterFrequency = 0.5f;
    public float windInfluence = 1.0f;
    public float movementInfluence = 0.5f;
    public Vector3 windDirection = Vector3.right;
    public float windStrength = 0.2f;

    // Input variables
    private float horizontalInput;
    private float verticalInput;

    // Wing flap state
    private float wingFlapSpeed;
    private float wingFlapTime;
    private bool lockFlapAtMidpoint;
    private bool cycleComplete;
    private float hoverAmplitude;

    // Rigidbody
    private Rigidbody rb;

    // Saved values
    private float speedOriginal;
    private float turnSpeedOriginal;
    private float pitchSpeedOriginal;
    private float maxWingFlapSpeedOriginal;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        speedOriginal = speed;
        turnSpeedOriginal = turnSpeed;
        pitchSpeedOriginal = pitchSpeed;
        maxWingFlapSpeedOriginal = maxWingFlapSpeed;
    }

    public void ResetValues()
    {
        speed = speedOriginal;
        turnSpeed = turnSpeedOriginal;
        pitchSpeed = pitchSpeedOriginal;
        maxWingFlapSpeed = maxWingFlapSpeedOriginal;
    }

    public void ApplyInput(float horizontal, float vertical)
    {
        horizontalInput = horizontal;
        verticalInput = vertical;
    }

    private void Update()
    {
        if (controllable)
        {
            rb.isKinematic = false;
            HandleMovement();
        }
        else
        {
            ResetInput();
            rb.isKinematic = true;
        }

        AnimateWings();
        ApplyHoverEffect();
        SimulateWingMovement();
        AdjustBodyForWind();
    }

    private void ResetInput()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
    }

    private void HandleMovement()
    {
        Rotate();
        MoveForward();
    }

    private void Rotate()
    {
        // Yaw rotation (around the Y axis)
        var yawRotation = Quaternion.Euler(0f, horizontalInput * turnSpeed * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * yawRotation);

        // Pitch rotation (up and down)
        if (Mathf.Abs(verticalInput) > 0)
        {
            var currentPitch = rb.rotation.eulerAngles.x;
            currentPitch = (currentPitch > 180f) ? currentPitch - 360f : currentPitch;

            var newPitch = Mathf.Clamp(currentPitch - verticalInput * pitchSpeed * Time.deltaTime, -maxPitch, maxPitch);
            var pitchRotation = Quaternion.Euler(newPitch, rb.rotation.eulerAngles.y, 0f);
            rb.MoveRotation(pitchRotation);
        }
        else
        {
            LevelOut();
        }
    }

    private void LevelOut()
    {
        var targetRotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y, 0f);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime));
    }

    private void MoveForward()
    {
        var movement = speed * transform.forward;
        movement += windStrength * windDirection.normalized;
        movement += GetRandomJitter();

        rb.velocity = movement;
    }

    private Vector3 GetRandomJitter()
    {
        var jitterOffset = new Vector3(
            (Mathf.PerlinNoise(Time.time * jitterFrequency, 0f) - 0.5f) * 2f,
            (Mathf.PerlinNoise(0f, Time.time * jitterFrequency) - 0.5f) * 2f,
            (Mathf.PerlinNoise(Time.time * jitterFrequency, Time.time * jitterFrequency) - 0.5f) * 2f
        ) * jitterStrength;

        return jitterOffset;
    }

    private void AnimateWings()
    {
        var flapSpeedOffset = 0f;

        if (verticalInput != 0f)
        {
            var mappedInput = (verticalInput + 1f) / 2f;
            flapSpeedOffset = Mathf.Lerp(-(maxWingFlapSpeed - 0.5f), 1f, mappedInput);
        }

        var targetFlapSpeed = maxWingFlapSpeed + flapSpeedOffset;
        wingFlapSpeed = Mathf.Lerp(wingFlapSpeed, targetFlapSpeed, Time.deltaTime * 5f);
        wingFlapTime += Time.deltaTime * wingFlapSpeed;

        var normalizedTime = Mathf.PingPong(wingFlapTime, 1f);

        if (verticalInput < 0f)
        {
            if (cycleComplete && normalizedTime >= 0.5f)
                lockFlapAtMidpoint = true;

            if (lockFlapAtMidpoint)
                normalizedTime = 0.5f;

            if (!cycleComplete && normalizedTime >= 0.9f)
                cycleComplete = true;
        }
        else
        {
            lockFlapAtMidpoint = false;
            cycleComplete = false;
        }

        var flapAngle = Mathf.Lerp(-wingFlapAngle, wingFlapAngle, wingFlapCurve.Evaluate(normalizedTime));
        SetWingRotation(flapAngle);
    }

    private void SetWingRotation(float flapAngle)
    {
        var rotationAngle = Mathf.Sin(wingFlapTime * Mathf.PI * 2) * wingRotationAmplitude;

        var verticalFlapLeft = Quaternion.Euler(0f, 0f, flapAngle);
        var verticalFlapRight = Quaternion.Euler(0f, 0f, -flapAngle);

        var baseRotationLeft = Quaternion.Euler(0f, rotationAngle, 0f);
        var baseRotationRight = Quaternion.Euler(0f, -rotationAngle, 0f);

        leftWing.localRotation = verticalFlapLeft * baseRotationLeft;
        rightWing.localRotation = verticalFlapRight * baseRotationRight;
    }

    private void ApplyHoverEffect()
    {
        hoverAmplitude = Mathf.Lerp(hoverAmplitude, verticalInput < 0f ? 0f : maxHoverAmplitude, Time.deltaTime * 2f);

        var hoverOffset = wingFlapCurve.Evaluate(Mathf.PingPong(wingFlapTime, 1f)) * hoverAmplitude;
        model.localPosition = new Vector3(model.localPosition.x, hoverOffset, model.localPosition.z);
    }

    private void SimulateWingMovement()
    {
        var movementEffect = transform.forward * movementInfluence;
        var windEffect = windInfluence * windStrength * windDirection.normalized;
        ApplyWingDeformation(movementEffect + windEffect);
    }

    private void ApplyWingDeformation(Vector3 effect)
    {
        var windAlignment = Vector3.Dot(transform.forward.normalized, windDirection.normalized);
        var wingDeformationAngle = Mathf.Lerp(-30f, 0f, (windAlignment + 1f) / 2f) * windStrength;

        var deformationRotation = Quaternion.Euler(effect.z + wingDeformationAngle, 0f, effect.x);
        leftWing.localRotation *= deformationRotation;
        rightWing.localRotation *= deformationRotation;
    }

    private void AdjustBodyForWind()
    {
        var windAlignment = Vector3.Dot(transform.forward.normalized, windDirection.normalized);
        var maxBodyTiltAngle = 20f;
        var bodyTiltAngle = (windAlignment <= 0f) ? Mathf.Lerp(0f, maxBodyTiltAngle, -windAlignment) * windStrength : 0f;

        body.localRotation = Quaternion.Euler(-bodyTiltAngle, body.localRotation.eulerAngles.y, body.localRotation.eulerAngles.z);
    }
}
