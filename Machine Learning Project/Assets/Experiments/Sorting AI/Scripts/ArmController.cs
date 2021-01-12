using System;
using UnityEngine;

namespace Experiments.Sorting_AI.Scripts
{
    public class ArmController : MonoBehaviour
    {
        [Header("Arm Settings")]
        public float rotationTime;
        
        //Current
        private Tilt _currentTilt = Tilt.Right;



        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MoveArm(Tilt.Left);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                MoveArm(Tilt.Right);
            }
            
        }

        private void MoveArm(Tilt tilt)
        {
            if (tilt == _currentTilt) return;
            
            
            
            switch (tilt)
            {
                case Tilt.Right:
                    LeanTween.rotateLocal(this.gameObject, new Vector3(30, 0, 45), rotationTime);
                    break;
                case Tilt.Left:
                    LeanTween.rotateLocal(this.gameObject, new Vector3(30, 0, -45), rotationTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tilt), tilt, "Should be either Tilt.Right or Tilt.Left");
            }

            _currentTilt = tilt;
        }
    }
    public enum Tilt {Right,Left}
}


