using System;
using UnityEngine;

namespace Experiments.Sorting_AI.Scripts
{
    public class SensorManager : MonoBehaviour
    {
        [Header("Sensor Settings")] 
        public string sensorName;
        public string[] approvedBalls;
        
        
        public event Action<GameObject,bool,string> SensorTrigger;
    
        protected virtual void OnSensorTrigger(GameObject ball, bool approved)
        {
            SensorTrigger?.Invoke(ball,approved,sensorName);
        }

        private void OnTriggerEnter(Collider other)
        {
            string color = other.gameObject.GetComponent<BallController>().GetColor();

            OnSensorTrigger(other.gameObject,CheckIfApproved(color));
        }

        private bool CheckIfApproved(string color)
        {
            switch (approvedBalls[0])
            {
                case "All":
                    return true;
                case "None":
                    return false;
            }

            foreach (string approvedBall in approvedBalls)
            {
                if (color == approvedBall) return true;
            }

            return false;
        }
    }
    
    
}
