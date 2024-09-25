using UnityEngine;
using UnityEngine.Events;

public class PresenceTrigger : MonoBehaviour
{
    public UnityEvent enterEvent;
    public UnityEvent exitEvent;
    public UnityEvent allExitedEvent;

    private int numColliders = 0;

    void OnTriggerEnter(Collider collider)
    {
        numColliders++;
        enterEvent.Invoke();
    }

    void OnTriggerExit(Collider collider)
    {
        numColliders--;
        exitEvent.Invoke();
        
        if (numColliders == 0)
        {
            allExitedEvent.Invoke();
        }
    }
}
