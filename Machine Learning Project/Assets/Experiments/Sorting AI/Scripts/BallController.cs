using System;
using UnityEngine;

namespace Experiments.Sorting_AI.Scripts
{
    public class BallController : MonoBehaviour
    {
        private BallColor _color = BallColor.Blue;

        //Ball behavior
        public float gravity;
        
        private float _userChoosenGravityValue;
        private Rigidbody _rb;


        private void Awake()
        {
            _userChoosenGravityValue = gravity;
            _rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            _rb.AddForce(Vector3.down*gravity);
        }

        public void SetColor(BallColor color)
        {
            _color = color;
        }

        public BallColor GetColor()
        {
            return _color;
        }

        public void EnableGravity(bool gravityEnabled)
        {
            gravity = gravityEnabled ? _userChoosenGravityValue : 0;
        }
    }
    
    public enum BallColor{All,Red,Blue}
}
