using UnityEngine;

public class ToggleColliders : MonoBehaviour
{
    public Collider[] colliders;

    public void Toggle(bool enableColliders)
    {
        foreach (var c in colliders)
        {
            c.enabled = enableColliders;
        }
    }
}
