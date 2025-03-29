﻿using UnityEngine;

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
        private float _slopeLimit = 35f;
        [SerializeField]
        private float _stepHeight = 0.3f;

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

            if (Physics.CapsuleCast(point1, point2, radius, moveDelta.normalized, moveDelta.magnitude, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore) == false)
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
                    var slopeMove = Vector3.ProjectOnPlane(moveDelta, hit.normal);
                    transform.position += slopeMove;
                }
                else if (TryStepUp(moveDelta))
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

            SnapToGround(dir);
        }

        private void SnapToGround(Vector3 dir)
        {
            var origin = transform.position + CapsuleCollider.center + dir.normalized * 0.1f + Vector3.up * 0.1f;

            if (Physics.Raycast(origin, Vector3.down, out var hit, _groundSnapDistance,
                CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
            {
                var bottomOffset = CapsuleCollider.height * 0.5f;
                var y = hit.point.y - CapsuleCollider.center.y + bottomOffset;
                var pos = transform.position;
                pos.y = y;
                transform.position = pos;
            }
        }

        private bool TryStepUp(Vector3 moveDelta)
        {
            var stepUpPos = transform.position + Vector3.up * _stepHeight;
            var stepDownPos = stepUpPos + moveDelta;

            var radius = CapsuleCollider.radius;
            var halfHeight = Mathf.Max(0f, CapsuleCollider.height * 0.5f - radius);
            var origin = stepUpPos + CapsuleCollider.center + Vector3.up * _collisionCheckMargin;
            var point1 = origin + Vector3.up * halfHeight;
            var point2 = origin - Vector3.up * halfHeight;

            if (Physics.CapsuleCast(point1, point2, radius, moveDelta.normalized, moveDelta.magnitude, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore) == false)
                return false;

            transform.position = stepDownPos;
            return true;
        }
    }
}
