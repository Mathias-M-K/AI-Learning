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
        public bool automaticBallSpawns;

        [Header("Sorting Arm")] 
        public ArmController middleArmController;
        public ArmController leftArmController;
        public ArmController rightArmController;
        

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
        public string entrySensorMidLastObs = "";
        public string entrySensorRightLastObs = "";
        public string entrySensorLeftLastObs = "";


        /*
         * AI Methods
         */

        public override void Initialize()
        {
            _statsRecorder = Academy.Instance.StatsRecorder;

            //Registering sensors and subscribing to them
            RegisterSensors();
            
            //Set ball-spawner mode
            if(automaticBallSpawns) ballSpawner.SetMode(SpawnModes.Automatic);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            //Adding the last ball through the entry-sensor as an observation
            if(entrySensorMidLastObs != "") sensor.AddObservation(ballSpawner.GetColorIndex(entrySensorMidLastObs));
            if(entrySensorLeftLastObs != "") sensor.AddObservation(ballSpawner.GetColorIndex(entrySensorMidLastObs));
            if(entrySensorRightLastObs != "") sensor.AddObservation(ballSpawner.GetColorIndex(entrySensorMidLastObs));
            
            //Adding the position of the arm as an observation
            sensor.AddObservation(TranslateArmPosition(middleArmController.GetCurrentArmPosition()));
            sensor.AddObservation(TranslateArmPosition(leftArmController.GetCurrentArmPosition()));
            sensor.AddObservation(TranslateArmPosition(rightArmController.GetCurrentArmPosition()));
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            /*
             * 0 = middle arm
             * 1 = Left arm
             * 2 = Right arm
             */
            
            middleArmController.MoveArm(actions.DiscreteActions[0]);
            leftArmController.MoveArm(actions.DiscreteActions[1]);
            rightArmController.MoveArm(actions.DiscreteActions[2]);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var actions = actionsOut.DiscreteActions;
            
            //Middle arm
            if (Input.GetKey(KeyCode.E)) actions[0] = 0;
            if (Input.GetKey(KeyCode.I)) actions[0] = 1;
            
            //Left arm
            if (Input.GetKey(KeyCode.Q)) actions[1] = 0;
            if (Input.GetKey(KeyCode.W)) actions[1] = 1;
            
            //Right arm
            if (Input.GetKey(KeyCode.O)) actions[2] = 0;
            if (Input.GetKey(KeyCode.P)) actions[2] = 1;
        }
        
        

        private void OnSensorTrigger(GameObject ball, bool approved, string sensorName, SensorType sensorType)
        {
            //Request decision from academy
            RequestDecision();
            
            //Updating the cumulative reward spectator field
            cumulativeReward = GetCumulativeReward();

            //Checking if the entry sensor. If yes, then new balls need to be spawned
            if (sensorType == SensorType.Observation)
            {
                if (sensorName.Equals("Mid"))
                {
                    Debug.Log("Mid fire");
                    entrySensorMidLastObs = ball.GetComponent<BallController>().GetColor();
                }
                
                if (sensorName.Equals("Left"))
                {
                    Debug.Log("Left fire");
                    entrySensorLeftLastObs = ball.GetComponent<BallController>().GetColor();
                }
                
                if (sensorName.Equals("Right"))
                {
                    Debug.Log("Right fire");
                    entrySensorRightLastObs = ball.GetComponent<BallController>().GetColor();
                }

                if (approved)
                {
                    AddReward(correctSortReward);
                }
                else
                {
                    AddReward(wrongSortPenalty);
                }
                
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