using UnityEngine;

public class RandomSize : MonoBehaviour
{
    public Vector2 scaleOffsetRange = new(-0.5f, 0.5f);

    // Start is called before the first frame update
    void Start()
    {
        var r = Random.Range(scaleOffsetRange.x, scaleOffsetRange.y);
        transform.localScale += new Vector3(r, r, r);
    }
}
