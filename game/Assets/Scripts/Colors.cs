using System.Linq;
using UnityEngine;

public class Colors : MonoBehaviour
{
    [System.Serializable]
    public struct ColorMapping
    {
        public string name;
        public Color color;
    }

    public ColorMapping[] colors;

    public static Colors Instance
    {
        get { return instance; }
    }
    private static Colors instance;

    void Awake()
    {
        instance = this;
    }

    public Color GetColor(string name)
    {
        return colors.FirstOrDefault(c => c.name == name).color;
    }

    public Color GetRandom()
    {
        return colors[Random.Range(0, colors.Length)].color;
    }
}
