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

        [Header("이동 옵션")]
        [SerializeField]
        private float _stepHeight = 0.3f;
        [SerializeField]
        private float _slopeLimit = 35f;

        private CapsuleCollider CapsuleCollider { get; set; }

        private void Awake()
        {
            CapsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void AttemptMove(Vector3 moveDelta)
        {
            var origin = transform.position + CapsuleCollider.center + Vector3.up * _collisionCheckMargin;
            var radius = CapsuleCollider.radius;
            var halfHeight = Mathf.Max(0f, CapsuleCollider.height * 0.5f - radius);
            var point1 = origin + Vector3.up * halfHeight;
            var point2 = origin - Vector3.up * halfHeight;

            if (!Physics.CapsuleCast(point1, point2, radius, moveDelta.normalized, moveDelta.magnitude, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
            {
                transform.position += moveDelta;
            }
        }

        public void MoveByDeltaTime(Vector3 dir, float speed)
        {
            if (dir == Vector3.zero)
                return;

            var moveDelta = speed * Time.deltaTime * dir.normalized;
            var origin = transform.position + CapsuleCollider.center + Vector3.up * _collisionCheckMargin;
            var radius = CapsuleCollider.radius;
            var halfHeight = Mathf.Max(0f, CapsuleCollider.height * 0.5f - radius);
            var point1 = origin + Vector3.up * halfHeight;
            var point2 = origin - Vector3.up * halfHeight;

            if (Physics.CapsuleCast(point1, point2, radius, moveDelta.normalized, out var hit, moveDelta.magnitude, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
            {
                var angle = Vector3.Angle(hit.normal, Vector3.up);

                if (angle <= _slopeLimit)
                {
                    transform.position += moveDelta;
                }
                else if (TryStepUp(moveDelta, hit))
                {
                }
                else
                {
                    var slideDir = Vector3.ProjectOnPlane(moveDelta, hit.normal);
                    AttemptMove(slideDir);
                }
            }
            else
            {
                transform.position += moveDelta;
            }

            SnapToGround();
        }

        private void SnapToGround()
        {
            var origin = transform.position + CapsuleCollider.center + Vector3.up * 0.1f;

            if (Physics.Raycast(origin, Vector3.down, out var hit, _groundSnapDistance, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
            {
                var bottomOffset = CapsuleCollider.height * 0.5f;
                var snapPosition = hit.point - CapsuleCollider.center + Vector3.up * bottomOffset;
                transform.position = snapPosition;
            }
        }

        private bool TryStepUp(Vector3 moveDelta, RaycastHit hit)
        {
            // 위로 올라간 위치에서 다시 시도
            var stepUpPos = transform.position + Vector3.up * _stepHeight;
            var newOrigin = stepUpPos + CapsuleCollider.center + Vector3.up * _collisionCheckMargin;
            var radius = CapsuleCollider.radius;
            var halfHeight = Mathf.Max(0f, CapsuleCollider.height * 0.5f - radius);
            var point1 = newOrigin + Vector3.up * halfHeight;
            var point2 = newOrigin - Vector3.up * halfHeight;

            if (!Physics.CapsuleCast(point1, point2, radius, moveDelta.normalized, moveDelta.magnitude, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
            {
                transform.position = stepUpPos + moveDelta;
                return true;
            }

            return false;
        }
    }
}
