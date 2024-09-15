using DG.Tweening;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField] private Transform model;

    void Start()
    {
        var s = DOTween.Sequence();
        s.Append(model.DOScale(1.05f, 0.5f))
            .Join(model.DOLocalRotate(new(0f, 10f, 0f), 0.5f, RotateMode.LocalAxisAdd))
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
