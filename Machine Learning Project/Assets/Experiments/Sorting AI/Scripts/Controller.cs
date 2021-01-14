using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Experiments.Sorting_AI.Scripts
{
    public class Controller : Agent
    {

        [Header("Spectator Values")] 
        public float cumulativeReward;

        [Header("Settings")] 
        public int maxBallsBeforeResetting;

        [Header("Sorting Arm")] 
        public ArmController armController;

        [Header("Rewards")] 
        public float wrongSortPenalty;
        public float correctSortReward;
        public float endGameReward;

        [Header("Components")] 
        [Tooltip("Sensor Parent Obj")] 
        public GameObject sensors;
        public BallSpawner ballSpawner;
        
        //Counting balls
        private int _ballsSortedCorrectCount;
        private int _ballsSortedIncorrectCount;

        //Stats Recorder
        private StatsRecorder _statsRecorder;
        
        //Observations
        public string lastBall = "";


        /*
         * AI Methods
         */

        public override void Initialize()
        {
            _statsRecorder = Academy.Instance.StatsRecorder;

            //Registering sensors and subscribing to them
            RegisterSensors();
            
            //Set Ballspawner to contunius shoot balls
            ballSpawner.SetMode(SpawnModes.Automatic);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            //Adding the last ball through the entry-sensor as an observation
            if(lastBall != "") sensor.AddObservation(ballSpawner.GetColorIndex(lastBall));
            
            //Adding the position of the arm as an observation
            sensor.AddObservation(TranslateArmPosition(armController.GetCurrentArmPosition()));
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            // 0 = left, 1 = right
            switch (actions.DiscreteActions[0])
            {
                case 0:
                    armController.MoveArm(Tilt.Left);
                    break;
                case 1:
                    armController.MoveArm(Tilt.Right);
                    break;
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var actions = actionsOut.DiscreteActions;

            if (Input.GetKey(KeyCode.LeftArrow)) actions[0] = 0;
            if (Input.GetKey(KeyCode.RightArrow)) actions[0] = 1;
        }
        
        

        private void OnSensorTrigger(GameObject ball, bool approved, string sensorName)
        {
            //Request decision from academy
            RequestDecision();
            
            //Updating the cumulative reward spectator field
            cumulativeReward = GetCumulativeReward();

            //Checking if the entry sensor. If yes, then new balls need to be spawned
            if (sensorName == "EntrySensor")
            {
                lastBall = ball.GetComponent<BallController>().GetColor();
                
                return;
            }

            //Print sensor obs
            Debug.Log($"SensorObs:{ball},{approved},{sensorName}");

            

            //Check whether the ball was sorted correct or incorrect
            if (approved)
            {
                _ballsSortedCorrectCount++;
                AddReward(correctSortReward);
            }
            else
            {
                _ballsSortedIncorrectCount++;
                AddReward(wrongSortPenalty);
            }

            //Checking whether the episode should be reset
            if (_ballsSortedCorrectCount + _ballsSortedIncorrectCount >= maxBallsBeforeResetting)
            {
                AddReward(endGameReward);
                AgentReset();
            }
        }

        private void RegisterSensors()
        {
            foreach (Transform child in sensors.transform)
            {
                child.gameObject.GetComponent<SensorManager>().SensorTrigger += OnSensorTrigger;
            }
        }

        private void AgentReset()
        {
            ballSpawner.ClearBalls();
            
            _statsRecorder.Add("MyStats/Balls sorted correct",_ballsSortedCorrectCount);
            _statsRecorder.Add("MyStats/Balls sorted incorrect",_ballsSortedIncorrectCount);
            
            _ballsSortedCorrectCount = 0;
            _ballsSortedIncorrectCount = 0;

            EndEpisode();
        }

        /*
         * Translation Methods
         */
        


        private int TranslateArmPosition(Tilt armPosition)
        {
            switch (armPosition)
            {
                case Tilt.Right:
                    return 2;
                case Tilt.Left:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(armPosition), armPosition, null);
            }
        }
    }
}