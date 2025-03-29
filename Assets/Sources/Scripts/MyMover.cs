using UnityEngine;

namespace oojjrs.omov
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class MyMover : MonoBehaviour
    {
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
                Debug.Log($"충돌함 : {hit.collider.name}");
                transform.position += (hit.distance - _skinWidth) * dir;
            }
            else
            {
                transform.position += dir * distance;
            }
        }
    }
}
