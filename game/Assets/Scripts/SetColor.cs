using UnityEngine;

public class SetColor : MonoBehaviour
{
    public string colorName;

    void Start()
    {
        Color color;
        if (string.IsNullOrEmpty(colorName))
        {
            color = Colors.Instance.GetRandom();
        }
        else
        {
            color = Colors.Instance.GetColor(colorName);
        }

        if (TryGetComponent<Renderer>(out var r))
        {
            r.material.color = color;
        }

        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.material.color = color;
        }
    }
}
