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
        public bool automaticallyClearBalls;
        public int maxBallsBeforeResetting;
        

        [Header("Sorting Arm")] 
        public ArmController armController;

        [Header("Rewards")] 
        public float wrongSortPenalty;
        public float correctSortReward;
        public float endGameReward;

        [Header("Components")] 
        [Tooltip("Sensor Parent Obj")] 
        public SensorManager entrySensor;
        public SensorManager sortingSensor1;
        public SensorManager sortingSensor2;
        public BallSpawner ballSpawner;

        //Counting balls
        private int _ballsSortedCorrectCount;
        private int _ballsSortedIncorrectCount;
        private readonly List<GameObject> _collectedBalls = new List<GameObject>();

        //Stats Recorder
        private StatsRecorder _statsRecorder;
        
        //Observations
        public string entrySensorLastObs = "";


        /*
         * AI Methods
         */

        public override void Initialize()
        {
            _statsRecorder = Academy.Instance.StatsRecorder;

            //Registering sensors and subscribing to them
            entrySensor.SensorTrigger += OnEntrySensorTrigger;
            sortingSensor1.SensorTrigger += OnObservationSensorTrigger;
            sortingSensor2.SensorTrigger += OnObservationSensorTrigger;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            //Adding the last ball through the entry-sensor as an observation
            if(entrySensorLastObs != "") sensor.AddObservation(ballSpawner.GetColorIndex(entrySensorLastObs));

            //Adding the position of the arm as an observation
            sensor.AddObservation(TranslateArmPosition(armController.GetCurrentArmPosition()));

        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            /*
             * 0 = middle arm
             * 1 = Left arm
             * 2 = Right arm
             */
            
            armController.MoveArm(actions.DiscreteActions[0]);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var actions = actionsOut.DiscreteActions;
            
            //Middle arm
            if (Input.GetKey(KeyCode.LeftArrow)) actions[0] = 0;
            if (Input.GetKey(KeyCode.RightArrow)) actions[0] = 1;

        }


        private void OnObservationSensorTrigger(GameObject ball, bool approved, string sensorName)
        {
            //Print sensor obs
            Debug.Log($"SensorObs:{ball},{approved},{sensorName}");

            _collectedBalls.Add(ball);

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
            if (_ballsSortedCorrectCount + _ballsSortedIncorrectCount >= maxBallsBeforeResetting && automaticallyClearBalls)
            {
                AddReward(endGameReward);
                AgentReset();
            }
        }

        private void OnEntrySensorTrigger(GameObject ball, bool approved, string sensorName)
        {
            //Updating the cumulative reward spectator field
            cumulativeReward = GetCumulativeReward();
            
            //Request decision from academy
            RequestDecision();
                
            //Setting last obs to the current ball
            entrySensorLastObs = ball.GetComponent<BallController>().GetColor();
        }
        

        private void AgentReset()
        {
            //ballSpawner.ClearBalls();
            foreach (GameObject ball in _collectedBalls)
            {
                Destroy(ball);
            }
            
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