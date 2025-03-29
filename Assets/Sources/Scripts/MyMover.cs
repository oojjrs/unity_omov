using UnityEngine;

namespace oojjrs.omov
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class MyMover : MonoBehaviour
    {
        // 적당히 긴 거리
        private const float AmpleDistance = 1000;

        [SerializeField]
        private bool _debugMode = false;
        [SerializeField]
        private float _skinWidth = 0.001f;

        private CapsuleCollider CapsuleCollider { get; set; }

        private void Awake()
        {
            CapsuleCollider = GetComponent<CapsuleCollider>();
        }

        public void MoveDelta(Vector3 dir, float speed)
        {
            if (dir == Vector3.zero)
                return;

            var origin = transform.TransformPoint(CapsuleCollider.center);
            var radius = CapsuleCollider.radius;
            var halfHeight = Mathf.Max(0, CapsuleCollider.height * 0.5f - radius);
            var point1 = origin + Vector3.up * halfHeight;
            var point2 = origin - Vector3.up * halfHeight;
            var distance = speed * Time.deltaTime;
            if (Physics.CapsuleCast(point1, point2, radius, dir, out RaycastHit hit, distance, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
            {
                if (_debugMode)
                    Debug.Log($"충돌함 : {hit.collider.name}");

                transform.position += (hit.distance - _skinWidth) * dir;
            }
            else
            {
                transform.position += dir * distance;
            }
        }

        public void SnapToGround()
        {
            var origin = transform.TransformPoint(CapsuleCollider.center);
            if (Physics.Raycast(origin, Vector3.down, out var hit, AmpleDistance, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
                transform.position += (hit.distance - CapsuleCollider.height * 0.5f - _skinWidth) * Vector3.down;
        }
    }
}
