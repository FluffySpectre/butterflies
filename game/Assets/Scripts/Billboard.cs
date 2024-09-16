using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Vector3 rotationAxis = new(0, 1, 0);

    void Update()
    {
        transform.LookAt(Camera.main.transform.position + rotationAxis * -2f, transform.parent.up);
    }
}
