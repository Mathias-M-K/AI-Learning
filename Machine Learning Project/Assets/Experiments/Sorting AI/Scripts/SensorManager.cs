using System;
using UnityEngine;

namespace Experiments.Sorting_AI.Scripts
{
    public class SensorManager : MonoBehaviour
    {
        [Header("Sensor Settings")] 
        public BallColor approvedBalls;
        
        public event Action<BallColor,bool> SensorTrigger;
    
        protected virtual void OnSensorTrigger(BallColor color, bool approved)
        {
            SensorTrigger?.Invoke(color,approved);
        }

        private void OnCollisionEnter(Collision other)
        {
            BallColor color = other.gameObject.GetComponent<BallController>().GetColor();
            
            
            OnSensorTrigger(color,approvedBalls==color);
        }


        
    }
    
    
}
