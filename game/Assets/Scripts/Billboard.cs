using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Vector3 rotationAxis = new(0, 1, 0);

    private Transform mainCamTransform;
    private Transform thisTransform;
    private Renderer thisRenderer;

    void Start()
    {
        mainCamTransform = Camera.main.transform;
        thisTransform = transform;
        thisRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        if (thisRenderer.isVisible)
        {
            thisTransform.LookAt(mainCamTransform.position + rotationAxis * -2f, thisTransform.parent.up);
        }
    }
}
