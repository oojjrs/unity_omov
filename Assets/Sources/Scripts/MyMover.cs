using UnityEngine;

namespace oojjrs.omov
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class MyMover : MonoBehaviour
    {
        [SerializeField]
        private float _collisionCheckMargin = 0.001f;

        [SerializeField]
        private float _groundSnapDistance = 1.0f;

        private CapsuleCollider CapsuleCollider { get; set; }

        private void Awake()
        {
            CapsuleCollider = GetComponent<CapsuleCollider>();
        }

        public void MoveByDeltaTime(Vector3 dir, float speed)
        {
            if (dir == Vector3.zero)
                return;

            var moveDelta = speed * Time.deltaTime * dir.normalized;
            var centerWorld = transform.position + CapsuleCollider.center + Vector3.up * _collisionCheckMargin;
            var halfHeight = Mathf.Max(0f, CapsuleCollider.height * 0.5f - CapsuleCollider.radius);
            var start = centerWorld - Vector3.up * halfHeight;
            var end = centerWorld + Vector3.up * halfHeight;

            if (Physics.CheckCapsule(start, end, CapsuleCollider.radius, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore) == false)
                transform.position += moveDelta;

            SnapToGround();
        }

        private void SnapToGround()
        {
            var origin = transform.position + CapsuleCollider.center + Vector3.up * 0.1f;

            if (Physics.Raycast(origin, Vector3.down, out var hit, _groundSnapDistance, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
                transform.position = hit.point - CapsuleCollider.center + CapsuleCollider.height * 0.5f * Vector3.up;
        }
    }
}
