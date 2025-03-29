using UnityEngine;

namespace oojjrs.omov
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class MyMover : MonoBehaviour
    {
        private CapsuleCollider CapsuleCollider { get; set; }

        private void Awake()
        {
            CapsuleCollider = GetComponent<CapsuleCollider>();
        }

        public void MoveByDeltaTime(Vector3 dir, float speed)
        {
            if (dir == Vector3.zero)
                return;

            var delta = speed * Time.deltaTime;
            var moveDelta = dir * delta;
            transform.position += moveDelta;
        }
    }
}
