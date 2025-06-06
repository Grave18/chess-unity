using UnityEngine;

namespace BoardEditor
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private float angle = 30f;

        private void Update()
        {
            transform.Rotate(new Vector3(0f, angle, 0f) * Time.deltaTime);
        }
    }
}
