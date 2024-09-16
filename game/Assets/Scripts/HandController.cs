using System;
using Cinemachine;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public enum HandState
    {
        MovingOver,
        Descending,
        Idle,
        Lifting,
        Throwing,
        SettingDown,
        Ascending
    }

    public HandState currentState = HandState.MovingOver;

    public Transform domeTransform;
    public float speed = 1f;
    public int throwCount = 3;
    public Vector3 handOutsidePosition;
    public Vector3 handOnDomePosition;
    public Vector3 handInTheSkyPosition;
    public CinemachineVirtualCamera mainCamera;

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

    public ParticleSystem groundParticles;

    private float initialCamDampeningX, initialCamDampeningY, initialCamDampeningZ;

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
        transform.position = Vector3.MoveTowards(transform.position, handInTheSkyPosition, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, handInTheSkyPosition) < 0.01f)
        {
            currentState = HandState.Descending;

            Debug.Log("Hand: Next state=Descending");
        }
    }

    void Descend()
    {
        var targetPosition = new Vector3(transform.position.x, handOnDomePosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

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
                currentState = HandState.SettingDown;

                mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = initialCamDampeningX;
                mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = initialCamDampeningY;
                mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = initialCamDampeningZ;

                groundParticles.Play();
            }
        }
    }

    void ThrowAndCatch() { /* ... */ }
    void SetDown()
    {
        // if (hasLifted)
        // {
        //     domeTransform.SetParent(transform);
        //     hasLifted = false;
        //     liftStartPosition = handOnDomePosition;
        //     liftEndPosition = handOnDomePosition + new Vector3(0, 20f, 0);
        //     liftTimer = 0f;

        //     // Anfangs- und Zielrotation festlegen
        //     initialRotation = transform.rotation;
        //     targetRotation = initialRotation * Quaternion.Euler(0, 0, 180f);
        // }

        // liftTimer += Time.deltaTime / liftDuration;
        // float curveValue = liftCurve.Evaluate(liftTimer);

        // // Position interpolieren
        // transform.position = Vector3.Lerp(liftStartPosition, liftEndPosition, curveValue);

        // // Rotation interpolieren
        // transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, curveValue);

        // if (liftTimer >= 1f)
        // {
        //     currentState = HandState.Throwing;
        // }
    }
    void Ascend() { /* ... */ }
}