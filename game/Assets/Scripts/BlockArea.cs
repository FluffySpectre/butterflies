using UnityEngine;

#if UNITY_EDITOR
public class BlockArea : MonoBehaviour
{
    void OnDrawGizmos()
    {
        var originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = originalMatrix;
    }
}
#endif
