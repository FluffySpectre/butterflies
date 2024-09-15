using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("GameEvents");
                instance = go.AddComponent<GameEvents>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    private static GameEvents instance;

    public event Action OnPlayerDied;
    public event Action<int> OnScoreChanged;
    public event Action<GameObject, GameObject> OnApproachFlower;

    public void TriggerPlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

    public void TriggerScoreChanged(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }

    public void TriggerApproachFlower(GameObject flower, GameObject interactor)
    {
        OnApproachFlower?.Invoke(flower, interactor);
    }
}
