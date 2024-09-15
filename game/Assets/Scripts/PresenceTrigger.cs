using UnityEngine;
using UnityEngine.Events;

public class PresenceTrigger : MonoBehaviour
{
    public UnityEvent enterEvent;
    public UnityEvent exitEvent;

    void OnTriggerEnter(Collider collider)
    {
        enterEvent.Invoke();
    }

    void OnTriggerExit(Collider collider)
    {
        exitEvent.Invoke();
    }
}
