﻿using System;
using UnityEngine;

namespace Experiments.Sorting_AI.Scripts
{
    public class SensorManager : MonoBehaviour
    {
        [Header("Sensor Settings")] 
        public string sensorName;
        public BallColor[] approvedBalls;
        
        
        public event Action<BallColor,bool,string> SensorTrigger;
    
        protected virtual void OnSensorTrigger(BallColor color, bool approved)
        {
            SensorTrigger?.Invoke(color,approved,sensorName);
        }

        private void OnTriggerEnter(Collider other)
        {
            BallColor color = other.gameObject.GetComponent<BallController>().GetColor();

            OnSensorTrigger(color,CheckIfApproved(color));
        }

        private bool CheckIfApproved(BallColor color)
        {
            if (color == BallColor.All) return true;
            
            foreach (BallColor approvedBall in approvedBalls)
            {
                if (color == approvedBall) return true;
            }

            return false;
        }
    }
    
    
}
