using oojjrs.omov;
using UnityEngine;

namespace Assets.Sources.Scenes
{
    public class Gravity : MonoBehaviour
    {
        [SerializeField]
        private MyMover[] _movers;

        private void Start()
        {
            foreach (var mover in _movers)
                mover.SnapToGround(true);
        }
    }
}
