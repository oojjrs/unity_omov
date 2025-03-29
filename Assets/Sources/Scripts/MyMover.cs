﻿using UnityEngine;

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
        private float _maxSnapDistance = 1;
        [SerializeField]
        private float _skinWidth = 0.001f;
        [SerializeField]
        private float _slopeAngle = 35;

        private CapsuleCollider CapsuleCollider { get; set; }

        private void Awake()
        {
            CapsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void AdjustToGround(float maxSnapDistance)
        {
            var origin = transform.TransformPoint(CapsuleCollider.center);
            var distance = CapsuleCollider.height * 0.5f + maxSnapDistance;

            if (Physics.Raycast(origin, Vector3.down, out var hit, distance, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
            {
                var groundOffset = hit.distance - CapsuleCollider.height * 0.5f;
                if (groundOffset > _skinWidth)
                {
                    var pos = transform.position + (groundOffset - _skinWidth) * Vector3.down;
                    if (_debugMode)
                        Debug.Log($"{name}> 바닥({hit.collider.name})에 맞춰 조정됨 : {transform.position} -> {pos}");

                    transform.position = pos;
                }
            }
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
                var angle = Vector3.Angle(hit.normal, Vector3.up);

                if (_debugMode)
                    Debug.Log($"{name}> 충돌함({hit.collider.name}) : {angle}도");

                if (angle <= _slopeAngle)
                {
                    var slopeDir = Vector3.ProjectOnPlane(dir, hit.normal).normalized;
                    if (Physics.CapsuleCast(point1, point2, radius, slopeDir, out RaycastHit slopeHit, distance, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
                        transform.position += (slopeHit.distance - _skinWidth) * slopeDir;
                    else
                        transform.position += slopeDir * distance;
                }
                else
                {
                    transform.position += (hit.distance - _skinWidth) * dir;

                    var slideDir = Vector3.ProjectOnPlane(dir, hit.normal);
                    if (slideDir != Vector3.zero)
                    {
                        if (Physics.CapsuleCast(point1, point2, radius, slideDir, out RaycastHit slideHit, distance, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
                            transform.position += (slideHit.distance - _skinWidth) * slideDir;
                        else
                            transform.position += slideDir * distance;
                    }

                    AdjustToGround(_maxSnapDistance);
                }
            }
            else
            {
                transform.position += dir * distance;

                AdjustToGround(_maxSnapDistance);
            }
        }

        public void SnapToGround(bool tryFindUpOnFail = false)
        {
            var origin = transform.TransformPoint(CapsuleCollider.center);
            if (Physics.Raycast(origin, Vector3.down, out var hitDown, AmpleDistance, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
            {
                var pos = transform.position + (hitDown.distance - CapsuleCollider.height * 0.5f - _skinWidth) * Vector3.down;
                if (_debugMode)
                    Debug.Log($"{name}> 바닥 찾음({hitDown.collider.name}) : {transform.position} -> {pos}");

                transform.position = pos;
            }
            else if (tryFindUpOnFail)
            {
                var originUp = origin + Vector3.up * AmpleDistance;
                if (Physics.Raycast(originUp, Vector3.down, out var hit, AmpleDistance, CapsuleCollider.includeLayers, QueryTriggerInteraction.Ignore))
                {
                    var pos = originUp + (hit.distance - CapsuleCollider.height * 0.5f - _skinWidth) * Vector3.down;
                    if (_debugMode)
                        Debug.Log($"{name}> 아래 바닥이 없어서 올라가서 찾음({hit.collider.name}) : {transform.position} -> {pos}");

                    transform.position = pos;
                }
            }
        }
    }
}
