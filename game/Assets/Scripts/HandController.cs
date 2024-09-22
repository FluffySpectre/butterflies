using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class HandController : MonoBehaviour
{
    public enum HandState
    {
        MovingOver,
        Descending,
        Idle,
        Lifting,
        Idle2,
        Throwing,
        SettingDown,
        Ascending,
        ActivationIdle,
    }

    public HandState currentState = HandState.MovingOver;

    public Transform domeTransform;
    public float speed = 1f;
    public int throwCount = 3;
    public Vector3 handOutsidePosition;
    public Vector3 handOnDomePosition;
    public Vector3 handInTheSkyPosition;
    public CinemachineVirtualCamera mainCamera;
    public AnimationCurve movementCurve;
    public ParticleSystem groundParticles;
    public UnityEvent onHandSequenceComplete;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private int currentThrow = 0; 

    private float idleTime = 2f;
    private float idleTimer = 0f;

    private bool hasLifted = false;
    private float liftTimer = 0f;
    public float liftDuration = 5f;
    private Vector3 liftStartPosition;
    private Vector3 liftEndPosition;
    public AnimationCurve liftCurve;

    private float initialCamDampeningX, initialCamDampeningY, initialCamDampeningZ;

    private float stateTimer;
    private bool stateChanged = true;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        initialCamDampeningX = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping;
        initialCamDampeningY = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping;
        initialCamDampeningZ = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping;
    }

    void Update()
    {
        switch (currentState)
        {
            case HandState.MovingOver:
                MovingOver();
                break;
            case HandState.Descending:
                Descend();
                break;
            case HandState.Idle:
                Idle();
                break;
            case HandState.Lifting:
                Lift();
                break;
            case HandState.Idle2:
                Idle2();
                break;
            case HandState.Throwing:
                ThrowAndCatch();
                break;
            case HandState.SettingDown:
                SetDown();
                break;
            case HandState.Ascending:
                Ascend();
                break;
        }
    }

    void MovingOver()
    {
        if (stateChanged)
        {
            stateChanged = false;
            stateTimer = 0f;
        }

        stateTimer += Time.deltaTime;

        transform.position = Vector3.Lerp(handOutsidePosition, handInTheSkyPosition, movementCurve.Evaluate(stateTimer * 0.075f));

        if (Vector3.Distance(transform.position, handInTheSkyPosition) < 0.01f)
        {
            transform.position = handInTheSkyPosition;

            currentState = HandState.Descending;
            stateChanged = true;

            Debug.Log("Hand: Next state=Descending");
        }
    }

    void Descend()
    {
        if (stateChanged)
        {
            stateChanged = false;
            stateTimer = 0f;
        }

        stateTimer += Time.deltaTime;

        var targetPosition = new Vector3(transform.position.x, handOnDomePosition.y, transform.position.z);

        transform.position = Vector3.Lerp(handInTheSkyPosition, handOnDomePosition, movementCurve.Evaluate(stateTimer * 0.15f));

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            currentState = HandState.Idle;

            Debug.Log("Hand: Next state=Idle");
        }
    }

    void Idle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTime)
        {
            idleTimer = 0f;
            currentState = HandState.Lifting;

            Debug.Log("Hand: Next state=Lifting");
        }
    }

    void Lift()
    {
        if (!hasLifted)
        {
            domeTransform.SetParent(transform);
            hasLifted = true;
            liftStartPosition = handOnDomePosition;
            liftEndPosition = handOnDomePosition + new Vector3(0, 20f, 0);
            liftTimer = 0f;

            initialRotation = transform.rotation;
            targetRotation = initialRotation * Quaternion.Euler(0, 0, 180f);

            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0.1f;
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0.1f;
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = 0.1f;
        }

        liftTimer += Time.deltaTime / liftDuration;
        float curveValue = liftCurve.Evaluate(liftTimer);

        transform.position = Vector3.Lerp(liftStartPosition, liftEndPosition, curveValue);

        transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, curveValue);

        if (liftTimer >= 1f)
        {
            transform.position = liftEndPosition;
            transform.rotation = targetRotation;

            var lerpedDampingX = Mathf.Lerp(mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping, initialCamDampeningX, Time.deltaTime * 2f);
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = lerpedDampingX;

            var lerpedDampingY = Mathf.Lerp(mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping, initialCamDampeningY, Time.deltaTime * 2f);
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = lerpedDampingY;

            var lerpedDampingZ = Mathf.Lerp(mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping, initialCamDampeningZ, Time.deltaTime * 2f);
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = lerpedDampingZ;

            if (Math.Abs(lerpedDampingX - initialCamDampeningX) < 0.1f)
            {
                // currentState = HandState.Throwing;

                // TEMP
                currentState = HandState.Idle2;

                mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = initialCamDampeningX;
                mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = initialCamDampeningY;
                mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = initialCamDampeningZ;

                groundParticles.Play();

                Debug.Log("Hand: Next state=Idle2");
            }
        }
    }

    void Idle2()
    {
        if (stateChanged)
        {
            stateChanged = false;
            stateTimer = 0f;
        }

        stateTimer += Time.deltaTime;

        if (stateTimer > 60f)
        {
            stateChanged = true;
            currentState = HandState.SettingDown;

            Debug.Log("Hand: Next state=SettingsDown");
        }
    }

    void ThrowAndCatch() { /* ... */ }
    void SetDown()
    {
        if (hasLifted)
        {
            domeTransform.SetParent(transform);
            hasLifted = false;
            liftStartPosition = handOnDomePosition + new Vector3(0, 20f, 0);
            liftEndPosition = handOnDomePosition;
            liftTimer = 0f;

            initialRotation = transform.rotation;
            targetRotation = initialRotation * Quaternion.Euler(0, 0, 180f);

            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0.1f;
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0.1f;
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = 0.1f;
        }

        liftTimer += Time.deltaTime / liftDuration;
        float curveValue = liftCurve.Evaluate(liftTimer);

        transform.position = Vector3.Lerp(liftStartPosition, liftEndPosition, curveValue);

        transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, curveValue);

        if (liftTimer >= 1f)
        {
            transform.position = liftEndPosition;
            transform.rotation = targetRotation;

            var lerpedDampingX = Mathf.Lerp(mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping, initialCamDampeningX, Time.deltaTime * 2f);
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = lerpedDampingX;

            var lerpedDampingY = Mathf.Lerp(mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping, initialCamDampeningY, Time.deltaTime * 2f);
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = lerpedDampingY;

            var lerpedDampingZ = Mathf.Lerp(mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping, initialCamDampeningZ, Time.deltaTime * 2f);
            mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = lerpedDampingZ;

            if (Math.Abs(lerpedDampingX - initialCamDampeningX) < 0.1f)
            {
                currentState = HandState.Ascending;
                stateChanged = true;

                mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = initialCamDampeningX;
                mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = initialCamDampeningY;
                mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = initialCamDampeningZ;

                groundParticles.Stop();

                domeTransform.SetParent(null);
                domeTransform.position = Vector3.zero;
                domeTransform.localRotation = Quaternion.identity;

                Debug.Log("Hand: Next state=Ascending");
            }
        }
    }
    void Ascend()
    {
        if (stateChanged)
        {
            stateChanged = false;
            stateTimer = 0f;
        }

        stateTimer += Time.deltaTime;

        transform.position = Vector3.Lerp(handOnDomePosition, handOutsidePosition, movementCurve.Evaluate(stateTimer * 0.15f));

        if (Vector3.Distance(transform.position, handOutsidePosition) < 0.01f)
        {
            transform.position = handOutsidePosition;

            onHandSequenceComplete.Invoke();

            currentState = HandState.ActivationIdle;

            // Debug.Log("Hand: Next state=Idle");
        }
    }
}