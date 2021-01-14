using System;
using UnityEngine;

namespace Experiments.Sorting_AI.Scripts
{
    public class BallController : MonoBehaviour
    {
        private string _color;

        //Ball behavior
        public float gravity;
        
        private float _userChosenGravityValue;
        private Rigidbody _rb;


        private void Awake()
        {
            _userChosenGravityValue = gravity;
            _rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            _rb.AddForce(Vector3.down*gravity);
        }

        public void SetColor(string color)
        {
            _color = color;
        }

        public string GetColor()
        {
            return _color;
        }

        public void EnableGravity(bool gravityEnabled)
        {
            gravity = gravityEnabled ? _userChosenGravityValue : 0;
        }
    }
}
