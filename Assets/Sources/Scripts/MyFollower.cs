using System;
using System.Collections;
using UnityEngine;

namespace Assets.Sources.Scripts
{
    // 초간단 이동기
    public class MyFollower : MonoBehaviour
    {
        private Coroutine Coroutine { get; set; }

        public void MoveTo(Vector3 pos, float speed, Action onFinish = default)
        {
            if (speed == 0)
                return;

            if (Coroutine != default)
                StopCoroutine(Coroutine);

            Coroutine = StartCoroutine(Func());

            IEnumerator Func()
            {
                var distance = (pos - transform.position).magnitude;
                var estimated = distance / speed;
                var time = Time.time;
                var start = transform.position;
                while (true)
                {
                    var t = Mathf.Clamp01((Time.time - time) / estimated);
                    transform.position = Vector3.Lerp(start, pos, t);

                    if (t >= 1)
                        break;

                    yield return default;
                }

                transform.position = pos;

                Coroutine = default;

                onFinish?.Invoke();
            }
        }
    }
}
