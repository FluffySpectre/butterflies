using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    public enum GameEventType
    {
        OnApproachFlower,
        OnPlayerDied,
        OnScoreChanged,
    }

    public GameEventType gameEventType;
    public LayerMask allowedLayer;
    public float cooldownAfterTrigger = 15f;

    private float nextTriggerTime;

    void OnTriggerEnter(Collider collider)
    {
        if (Time.time < nextTriggerTime)
        {
            return;
        }

        if ((allowedLayer & (1 << collider.gameObject.layer)) == 0)
        {
            return;
        }

        if (gameEventType == GameEventType.OnApproachFlower)
        {
            GameEvents.Instance.TriggerApproachFlower(gameObject, collider.gameObject);
        }

        nextTriggerTime = Time.time + cooldownAfterTrigger;
    }
}
