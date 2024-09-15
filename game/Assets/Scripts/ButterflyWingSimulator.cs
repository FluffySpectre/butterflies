using UnityEngine;

public class ButterflyWingSimulator : MonoBehaviour
{
    public float curveValue = 0f;

    [SerializeField] private float deformationStrength = 0.2f;
    [SerializeField] private Vector3 deformationStartPoint = new(0, 0, 0);
    [SerializeField] private Vector3 exclusionCorner = new(1, 0, 1);
    [SerializeField] private float excludeRadius = 0.5f;
    [SerializeField] private float influenceRadius = 2f;

    private MeshFilter meshFilter;

    private Vector3[] originalVertices;
    private Vector3[] deformedVertices;

    private float previousCurveValue;
    private float previousDeformationStrength;
    private Vector3 previousDeformationStartPoint;
    private Vector3 previousExclusionCorner;
    private float previousExcludeRadius;
    private float previousInfluenceRadius;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalVertices = meshFilter.mesh.vertices;
        deformedVertices = new Vector3[originalVertices.Length];

        previousCurveValue = curveValue;
        previousDeformationStrength = deformationStrength;
        previousDeformationStartPoint = deformationStartPoint;
        previousExclusionCorner = exclusionCorner;
        previousExcludeRadius = excludeRadius;
        previousInfluenceRadius = influenceRadius;

        CalculateMesh();
    }

    void Update()
    {
        if (ParamsChanged())
        {
            CalculateMesh();
        }
    }

    private bool ParamsChanged()
    {
        bool changed = false;

        if (previousCurveValue != curveValue)
        {
            previousCurveValue = curveValue;
            changed = true;
        }

        if (previousDeformationStrength != deformationStrength)
        {
            previousDeformationStrength = deformationStrength;
            changed = true;
        }

        if (previousDeformationStartPoint != deformationStartPoint)
        {
            previousDeformationStartPoint = deformationStartPoint;
            changed = true;
        }

        if (previousExclusionCorner != exclusionCorner)
        {
            previousExclusionCorner = exclusionCorner;
            changed = true;
        }

        if (previousExcludeRadius != excludeRadius)
        {
            previousExcludeRadius = excludeRadius;
            changed = true;
        }

        if (previousInfluenceRadius != influenceRadius)
        {
            previousInfluenceRadius = influenceRadius;
            changed = true;
        }

        return changed;
    }

    private void CalculateMesh()
    {
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 originalVertex = originalVertices[i];

            float distanceToExclusion = Vector3.Distance(originalVertex, exclusionCorner);

            if (distanceToExclusion < excludeRadius)
            {
                deformedVertices[i] = originalVertex;
            }
            else
            {
                float distanceToStart = Vector3.Distance(originalVertex, deformationStartPoint);

                if (distanceToStart < influenceRadius)
                {
                    float influenceFactor = 1f - (distanceToStart / influenceRadius);
                    float deformation = curveValue * deformationStrength * influenceFactor;
                    deformedVertices[i] = new Vector3(originalVertex.x, originalVertex.y + deformation, originalVertex.z);
                }
                else
                {
                    deformedVertices[i] = originalVertex;
                }
            }
        }

        meshFilter.mesh.vertices = deformedVertices;
        meshFilter.mesh.RecalculateNormals();
    }
}
