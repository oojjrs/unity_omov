using Assets.Sources.Scripts;
using UnityEngine;

namespace Assets.Sources.Test.Scripts
{
    [RequireComponent(typeof(MyFollower))]
    public class FollowerTest : MonoBehaviour
    {
        private bool Toggle { get; set; }

        private void Start()
        {
            GetComponent<MyFollower>().MoveTo(transform.position + Vector3.right * 10, 3, Func);

            void Func()
            {
                if (Toggle)
                    GetComponent<MyFollower>().MoveTo(transform.position + Vector3.right * 10, 3, Func);
                else
                    GetComponent<MyFollower>().MoveTo(transform.position + Vector3.left * 10, 3, Func);

                Toggle = !Toggle;
            }
        }
    }
}
