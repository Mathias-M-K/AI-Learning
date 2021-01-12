using UnityEngine;


namespace Experiments.Sorting_AI.Scripts
{
    public class Controller : MonoBehaviour
    {
        private Vector3 _spawnPosition;
        public GameObject ballObj;

        [Header("Components")] 
        public SensorManager sensorManager;
        
        
        
        // Start is called before the first frame update
        void Start()
        {
            _spawnPosition = ballObj.transform.position;
            
            //Registering sensors and subscribing to them
            RegisterSensors();
        }

        private void SpawnNewBall()
        {
            int randomNr = Random.Range(0, 1);
            BallColor bColor;

            switch (randomNr)
            {
                case 0:
                    bColor = BallColor.Blue;
                    break;
                case 1:
                    bColor = BallColor.Red;
                    break;
                default:
                    bColor = BallColor.Red;
                    break;
            }       
            
            GameObject go = Instantiate(ballObj,_spawnPosition,Quaternion.Euler(0,0,0));
            go.GetComponent<BallController>().SetColor(bColor);
        }

        private void OnSensorTrigger(BallColor color, bool approved)
        {
            Debug.Log($"Sensor Reviced: {color},{approved}");
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
