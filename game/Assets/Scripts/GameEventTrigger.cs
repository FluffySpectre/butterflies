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

    void OnTriggerEnter(Collider collider)
    {
        if ((allowedLayer & (1 << collider.gameObject.layer)) == 0)
        {
            return;
        }

        if (gameEventType == GameEventType.OnApproachFlower)
        {
            GameEvents.Instance.TriggerApproachFlower(gameObject, collider.gameObject);
        }
    }
}
