using System.Collections;
using System.Collections.Generic;
using Experiments.Scripts;
using UnityEngine;

public class GameController : MonoBehaviour
{

    private List<VehicleController> _vehicleControllers = new List<VehicleController>();

    private float _startTime;

    public float countdown = 4;
    
    
    void Start()
    {
        GameObject[] vechicles = GameObject.FindGameObjectsWithTag("agent");

        foreach (GameObject vechicle in vechicles)
        {
            _vehicleControllers.Add(vechicle.GetComponent<VehicleController>());
        }
        
        ForceStop(true);
        _startTime = Time.time;
    }

    void Update()
    {
        countdown = Mathf.Floor(4 - (Time.time - _startTime));

        if (countdown <= 0)
        {
            ForceStop(false);
        }

        if (Input.GetKey(KeyCode.R))
        {
            StartAgain();
        }

    }

    private void ForceStop(bool b)
    {
        foreach (VehicleController controller in _vehicleControllers)
        {
            controller.EnableForceStop(b);
        }
    }

    private void StartAgain()
    {
        ForceStop(true);
        _startTime = Time.time;
        countdown = 4;

        foreach (VehicleController controller in _vehicleControllers)
        {
            controller.AgentReset();
        }
    }
}
