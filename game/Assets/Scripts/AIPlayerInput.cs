using UnityEngine;

public class AIPlayerInput : MonoBehaviour
{
    [SerializeField] private float minTimeUntilPickANewTarget = 5f;
    [SerializeField] private float maxTimeUntilPickANewTarget = 10f;

    private ButterflyController butterflyController;
    private float pickNextTargetTime = -1f;
    private Vector3 targetPosition;

    private void Awake()
    {
        butterflyController = GetComponent<ButterflyController>();
    }

    private void Update()
    {
        if (ShouldPickNewTarget())
        {
            PickNewTargetPosition();
        }

        MoveTowardsTarget();
    }

    private bool ShouldPickNewTarget()
    {
        return Time.time > pickNextTargetTime || Vector3.Distance(transform.position, targetPosition) < 1f;
    }

    private void PickNewTargetPosition()
    {
        pickNextTargetTime = Time.time + Random.Range(minTimeUntilPickANewTarget, maxTimeUntilPickANewTarget);
        targetPosition = new Vector3(
            Random.Range(-375f, 375f),
            Random.Range(15f, 80f),
            Random.Range(-375f, 375f)
        );
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
}
