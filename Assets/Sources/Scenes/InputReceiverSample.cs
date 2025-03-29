﻿using oojjrs.omov;
using UnityEngine;

namespace Assets.Sources.Scenes
{
    public class InputReceiverSample : MonoBehaviour
    {
        [SerializeField]
        private MyMover _mover;

        private void Update()
        {
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            if (h != 0 || v != 0)
                _mover.MoveByDeltaTime(new Vector3(h, 0f, v).normalized, 8f);
        }
    }
}
