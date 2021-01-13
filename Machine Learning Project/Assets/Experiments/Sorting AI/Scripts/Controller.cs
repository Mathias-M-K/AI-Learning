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
        private Vector3 _spawnPosition;
        [Header("Spectator Values")] 
        public float cumulativeReward;

        [Header("Sorting Arm")] 
        public ArmController armController;

        [Header("Ball")] 
        public GameObject ballStartPos;
        public GameObject ballPrefab;

        [Header("Materials")] 
        public Material materialRed;
        public Material materialBlue;

        [Header("Rewards")] 
        public float wrongSortPenalty;
        public float correctSortReward;
        public float endGameReward;

        [Header("Sensors")] [Tooltip("Sensor Parent Obj")]
        public GameObject sensors; 

        //In game balls
        private BallController _ballToBeReleased;
        private BallController _fallingBall;

        //Counting balls
        private int _ballsSortedCorrectCount;
        private int _ballsSortedIncorrectCount;
        private List<GameObject> _balls = new List<GameObject>();
        
        //Statsrecorder
        private StatsRecorder _statsRecorder;


        /*
         * AI Methods
         */

        public override void Initialize()
        {
            _spawnPosition = ballStartPos.transform.position;
            _fallingBall = ballStartPos.GetComponent<BallController>();
            _ballToBeReleased = _fallingBall;
            _statsRecorder = Academy.Instance.StatsRecorder;

            //Registering sensors and subscribing to them
            RegisterSensors();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            int fallingBallTranslationInt = TranslateColor(_fallingBall.GetColor());
            int spawnedBallTranslationInt = TranslateColor(_ballToBeReleased.GetColor());
            int armPositionTranslationInt = TranslateArmPosition(armController.GetCurrentArmPosition());
            
            sensor.AddObservation(fallingBallTranslationInt);
            sensor.AddObservation(spawnedBallTranslationInt);
            sensor.AddObservation(armPositionTranslationInt);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            // 0 = left, 1 = right
            Debug.Log(actions.DiscreteActions[0]);

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

        public override void OnEpisodeBegin()
        {
        }


        private void SpawnNewBall()
        {
            int randomNr = Random.Range(0, 2);
            BallColor bColor;
            Material bMaterial;

            switch (randomNr)
            {
                case 0:
                    bColor = BallColor.Blue;
                    bMaterial = materialBlue;
                    break;
                case 1:
                    bColor = BallColor.Red;
                    bMaterial = materialRed;
                    break;
                default:
                    bColor = BallColor.Red;
                    bMaterial = materialRed;
                    break;
            }

            GameObject go = Instantiate(ballPrefab, _spawnPosition, Quaternion.Euler(0, 0, 0));
            go.GetComponent<Renderer>().material.color = bMaterial.color;

            BallController newBallController = go.GetComponent<BallController>();
            newBallController.SetColor(bColor);
            newBallController.EnableGravity(false);

            _ballToBeReleased = newBallController;
        }

        private void ReleaseNewBall()
        {
            if (_ballToBeReleased == null) return;


            _fallingBall = _ballToBeReleased;
            _ballToBeReleased.EnableGravity(true);
        }

        private void OnSensorTrigger(GameObject ball, bool approved, string sensorName)
        {
            //Request decision from academy
            //RequestDecision();
            
            //Updating the cumulative reward spectator field
            cumulativeReward = GetCumulativeReward();

            //Checking if the entry sensor. If yes, then new balls need to be spawned
            if (sensorName == "EntrySensor")
            {
                SpawnNewBall();

                _balls.Add(ball);
                return;
            }

            //Print sensor obs
            Debug.Log($"SensorObs:{ball},{approved},{sensorName}");

            //Release the next ball
            ReleaseNewBall();

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
            if (_ballsSortedCorrectCount + _ballsSortedIncorrectCount >= 50)
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
            Debug.Log("Instance Reset");

            foreach (GameObject ball in _balls)
            {
                Destroy(ball);
            }

            _ballsSortedCorrectCount = 0;
            _ballsSortedIncorrectCount = 0;
            
            _statsRecorder.Add("MyStats/Balls sorted correct",_ballsSortedCorrectCount);
            _statsRecorder.Add("MyStats/Balls sorted incorrect",_ballsSortedIncorrectCount);

            EndEpisode();
        }

        /*
         * Translation Methods
         */

        /*
             * Translation
             * Color red = 1
             * Color blue = 2
             * Arm position left = 1;
             * Arm position right = 2;
             */

        private int TranslateColor(BallColor color)
        {
            //Translating ball color of the ball currently falling
            switch (_fallingBall.GetColor())
            {
                case BallColor.All:
                    throw new ArgumentException("Ball can't have 'all' set as it's color", nameof(color));
                case BallColor.Red:
                    return 1;
                case BallColor.Blue:
                    return 2;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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