using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Vector3 rotationAxis = new(0, 1, 0);

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        transform.LookAt(mainCam.transform.position + rotationAxis * -2f, transform.parent.up);
    }
}
