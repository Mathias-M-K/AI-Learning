using System;
using UnityEngine;

namespace Experiments.Sorting_AI.Scripts
{
    public class ArmController : MonoBehaviour
    {
        [Header("Arm Settings")]
        public float rotationTime;
        public bool lockArm;
        public Tilt armLockDirection;

        private void Start()
        {
            if (lockArm)
            {
                MoveArm(armLockDirection);
            }
        }

        //Current
        private Tilt _currentTilt = Tilt.Right;

        /// <summary>
        /// Moveing sorting arm, using the 
        /// </summary>
        /// <param name="tilt"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void MoveArm(Tilt tilt)
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

        /// <summary>
        /// 0 will move arm left, 1 will move arm right
        /// </summary>
        /// <param name="tilt"></param>
        public void MoveArm(int tilt)
        {
            switch (tilt)
            {
                case 0:
                    MoveArm(Tilt.Left);
                    break;
                case 1:
                    MoveArm(Tilt.Right);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tilt),"Argument should be either 0 or 1");
            }
        }

        public Tilt GetCurrentArmPosition()
        {
            return _currentTilt;
        }
    }
    public enum Tilt {Right,Left}
}


