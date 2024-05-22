using DG.Tweening;
using UnityEngine;

public class Move : MonoBehaviour
{
    private void Start()
    {
        transform.DOMove(Vector3.zero, 2).SetEase(Ease.InOutCubic);
    }

    public void MoveTo(Vector3 pos)
    {
        transform.DOMove(pos, 2).SetEase(Ease.InOutCubic);
    }
}
