using UnityEngine;

public class AIPlayerInput : MonoBehaviour
{
    public float minTimeUntilPickANewTarget = 5f;
    public float maxTimeUntilPickANewTarget = 10f;
    public float idleTimeUntilNewTarget = 1f;

    private ButterflyController butterflyController;
    private float pickNextTargetTime = -1f;
    private Vector3 targetPosition;
    private float collisionTime;
    private bool isColliding = false;

    private float minTimeUntilPickANewTargetStartup;
    private float maxTimeUntilPickANewTargetStartup;
    private float idleTimeUntilNewTargetStartup;

    public void ResetValues()
    {
        minTimeUntilPickANewTarget = minTimeUntilPickANewTargetStartup;
        maxTimeUntilPickANewTarget = maxTimeUntilPickANewTargetStartup;
        idleTimeUntilNewTarget = idleTimeUntilNewTargetStartup;
    }

    private void Awake()
    {
        butterflyController = GetComponent<ButterflyController>();

        minTimeUntilPickANewTargetStartup = minTimeUntilPickANewTarget;
        maxTimeUntilPickANewTargetStartup = maxTimeUntilPickANewTarget;
        idleTimeUntilNewTargetStartup = idleTimeUntilNewTarget;
    }

    private void Update()
    {
        if (ShouldPickNewTarget() || CollidedForTooLong())
        {
            PickNewTargetPosition();
        }

        MoveTowardsTarget();
    }

    private bool ShouldPickNewTarget()
    {
        return Time.time > pickNextTargetTime || Vector3.Distance(transform.position, targetPosition) < 1f;
    }

    private bool CollidedForTooLong()
    {
        return isColliding && collisionTime > idleTimeUntilNewTarget;
    }

    private void PickNewTargetPosition()
    {
        if (CollidedForTooLong())
        {
            targetPosition = transform.position - transform.forward * Random.Range(10f, 20f);
            transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            pickNextTargetTime = Time.time + Random.Range(minTimeUntilPickANewTarget, maxTimeUntilPickANewTarget);
            targetPosition = transform.parent.position + new Vector3(
                Random.Range(-375f, 375f),
                transform.parent.rotation.z < 0f ? Random.Range(-15f, -80f) : Random.Range(15f, 80f),
                Random.Range(-375f, 375f)
            );
        }

        collisionTime = 0f;
        isColliding = false;
    }

    private void MoveTowardsTarget()
    {
        var directionToTarget = targetPosition - transform.position;
        var targetRotation = Quaternion.LookRotation(directionToTarget);
        var rotationDifference = targetRotation * Quaternion.Inverse(transform.rotation);
        var rotationDifferenceEuler = rotationDifference.eulerAngles;

        if (rotationDifferenceEuler.x > 180f) rotationDifferenceEuler.x -= 360f;
        if (rotationDifferenceEuler.y > 180f) rotationDifferenceEuler.y -= 360f;

        var horizontal = Mathf.Clamp(rotationDifferenceEuler.y / 45f, -1f, 1f);
        var vertical = Mathf.Clamp(rotationDifferenceEuler.x / 45f, -1f, 1f);

        butterflyController.ApplyInput(horizontal, vertical);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, targetPosition);
        Gizmos.DrawWireSphere(targetPosition, 1f);
    }

    private void OnCollisionEnter()
    {
        isColliding = true;
        collisionTime = 0f;
    }

    private void OnCollisionStay()
    {
        collisionTime += Time.fixedDeltaTime;
    }

    private void OnCollisionExit()
    {
        isColliding = false;
        collisionTime = 0f;
    }
}
