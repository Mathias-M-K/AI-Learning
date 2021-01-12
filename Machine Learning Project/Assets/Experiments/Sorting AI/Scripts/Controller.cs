using Unity.MLAgents;
using UnityEngine;


namespace Experiments.Sorting_AI.Scripts
{
    public class Controller : Agent
    {
        private Vector3 _spawnPosition;
        
        [Header("Ball")]
        public GameObject ballStartPos;
        public GameObject ballPrefab;

        [Header("Materials")] 
        public Material materialRed;
        public Material materialBlue;
        
        //Spawn Ball Values
        private BallController _ballToBeReleased;
        
        
        /*
         * AI Methods
         */
        
        

        void Start()
        {
            _spawnPosition = ballStartPos.transform.position;
            
            //Registering sensors and subscribing to them
            RegisterSensors();
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
            
            GameObject go = Instantiate(ballPrefab,_spawnPosition,Quaternion.Euler(0,0,0));
            go.GetComponent<Renderer>().material.color = bMaterial.color;
            
            BallController newBallController = go.GetComponent<BallController>();
            newBallController.SetColor(bColor);
            newBallController.EnableGravity(false);

            _ballToBeReleased = newBallController;
        }

        private void ReleaseNewBall()
        {
            if (_ballToBeReleased == null) return;
            
            _ballToBeReleased.EnableGravity(true);
        }

        private void OnSensorTrigger(BallColor color, bool approved, string sensorName)
        {
            Debug.Log($"Sensor Report: {color},{approved},{sensorName}");
            
            if (sensorName == "EntrySensor")
            {
                SpawnNewBall();
            }
            else
            {
                ReleaseNewBall();
            }
        }

        private void RegisterSensors()
        {
            GameObject[] sensors = GameObject.FindGameObjectsWithTag("Sensor");

            foreach (GameObject sensor in sensors)
            {
                sensor.GetComponent<SensorManager>().SensorTrigger += OnSensorTrigger;
            }
        }
        
    }
}
