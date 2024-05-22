using DG.Tweening;
using UnityEngine;

public class Move : MonoBehaviour
{
    private void Start()
    {
    }

    public void MoveTo(Vector3 pos)
    {
        transform.DOMove(pos, 2).SetEase(Ease.InOutCubic);
    }
}
