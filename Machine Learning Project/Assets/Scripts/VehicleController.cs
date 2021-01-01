using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class VehicleController : Agent
{
    [Header("Car Status")]
    public float turnValue = 0;     //Current rotation value to turn
    public float speed;             //Current speed
    
    [Header("Speed Values")]
    public int maxSpeed;            //Max speed that vehicle can achieve
    public float brakeSpeed;        //Deaccelerate value when breaking
    public float accelerationSpeed; //Acceleration value
    public float deaccelerationMultiplier;

    [Header("Turn Values")]
    public float maxTurnRate;       //Max turnValue when turning
    public float minTurnRate;       //Min turnValue when turning
    public float turnRateResetSpeed;//Determines how long it will take from turning to driving straight forward
    private float _maxTurnRateActual;    //Current maxTurnRate

    [Header("Raycast Values")] 
    public float raycastLength;
    
    [Header("Other")]
    public float turnRate;
    public bool headOnCollision;
    private CheckpointManager _checkpointManager;
    

    [Header("AI Controls")]
    public int turn;    //1 = right, 0 = no turn, -1 = left
    public int drive;   //1 = forward, 0 = no drive, -1 = reverse
    public int brake;   //1 = brake, 0 = no brake
    public Vector3 originPos;
    

    //Collision values
    private bool _headOnCollisionStop;
    
    public override void Initialize()
    {
        _checkpointManager = GetComponent<CheckpointManager>();
        GetComponent<CheckpointManager>().OnCheckpointHit += OnCheckpointHit;
        originPos = transform.position;

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //AddReward(-0.005f);
        //turn
        switch (actions.DiscreteActions[0])
        {
            case 0:
                turn = -1;
                break;
            case 1:
                turn = 0;
                break;
            case 2:
                turn = 1;
                break;
        }
        
        //drive
        switch (actions.DiscreteActions[1])
        {
            case 0:
                drive = -1;
                break;
            case 1:
                drive = 0;
                break;
            case 2:
                drive = 1;
                break;
        }
        
        //brake
        switch (actions.DiscreteActions[2])
        {
            case 0:
                brake = 0;
                break;
            case 1:
                brake = 1;
                break;
        }
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        

        if (Input.GetKey(KeyCode.RightArrow)) actions[0] = 2;
        if (Input.GetKey(KeyCode.LeftArrow)) actions[0] = 0;
        if (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) actions[0] = 1;
            
        if (Input.GetKey(KeyCode.UpArrow)) actions[1] = 2;
        if (Input.GetKey(KeyCode.DownArrow)) actions[1] = 0;
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) actions[1] = 1;

        if (Input.GetKey(KeyCode.Space))
        {
            actions[2] = 1;
        }
        else
        {
            actions[2]  = 0;
        }

    }

    public override void OnEpisodeBegin()
    {
        transform.position = originPos;
    }

    private void OnCheckpointHit(bool i, string s)
    {
        
        if (i)
        {
            AddReward(2);
        }
        else
        {
            AddReward(-5);
            Debug.Log("Reset : Wrong Way");
            Reset();
        }
    }

    // Update is called once per frame
    void Update()
    {


        //Determining turn-rate based on the current speed of the vehicle
        _maxTurnRateActual = map(Math.Abs(speed), 0, maxSpeed, maxTurnRate, 0);
        if (_maxTurnRateActual < 2) _maxTurnRateActual = 2;
        
        
        /*if (manualControlEnabled)
        {
            if (Input.GetKey(KeyCode.RightArrow)) turn = 1;
            if (Input.GetKey(KeyCode.LeftArrow)) turn = -1;
            if (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) turn = 0;
            
            if (Input.GetKey(KeyCode.UpArrow)) drive = 1;
            if (Input.GetKey(KeyCode.DownArrow)) drive = -1;
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) drive = 0;

            if (Input.GetKey(KeyCode.Space))
            {
                brake = 1;
            }
            else
            {
                brake = 0;
            }
        }*/

        if (transform.position.y < -10)
        {
            Debug.Log("Reset : Left Track");
            Reset();
        }
        
        //Turning
        if (turn == 1 && Math.Abs(speed) > 0.1)
        {
            if (turnValue < 0)
            {
                turnValue += turnRate * turnRateResetSpeed * Time.deltaTime;
            }
            else
            {
                turnValue += turnRate * Time.deltaTime;
            }
            
        } else if (turn == -1 && Math.Abs(speed)  > 0.1)
        {
            if (turnValue > 0)
            {
                turnValue -= turnRate * turnRateResetSpeed * Time.deltaTime;
            }
            else
            {
                turnValue -= turnRate * Time.deltaTime;
            }
        }
        else
        {
            if (turnValue < 0) turnValue += turnRate * turnRateResetSpeed * Time.deltaTime;
            if (turnValue > 0) turnValue -= turnRate * turnRateResetSpeed * Time.deltaTime;
            if (Math.Abs(turnValue) < 0.5) turnValue = 0;
        }

        //Breaking and acceleration
        if (brake == 1)
        {
            if (speed > 0) speed -= brakeSpeed * Time.deltaTime;
            if (speed < 0) speed += brakeSpeed * Time.deltaTime;
        }else if (drive == 1)
        {
            speed += accelerationSpeed * Time.deltaTime;
            
        } else if (drive == -1)
        {
            speed -= accelerationSpeed * Time.deltaTime;
        }
        else
        {
            if(speed > 0) speed -= accelerationSpeed * deaccelerationMultiplier * Time.deltaTime;
            if(speed < 0) speed += accelerationSpeed * deaccelerationMultiplier * Time.deltaTime;
            if (Math.Abs(speed) < 0.5) speed = 0;
            headOnCollision = false;
        }
        
        //Enforcing restraints
        if (speed > maxSpeed) speed = maxSpeed;
        if (speed < -maxSpeed) speed = -maxSpeed;
        if (turnValue > _maxTurnRateActual) turnValue = _maxTurnRateActual;
        if (turnValue < -_maxTurnRateActual) turnValue = -_maxTurnRateActual;


        //Moving vehicle
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        transform.Rotate(Vector3.up,turnRate*turnValue*Time.deltaTime);
        
        //TODO - Have vehicle come to full stop if hitting a wall head on
    }

    private void Reset()
    {
        _checkpointManager.CurrentTargetCount = 0;
        drive = 0;
        turn = 0;
        brake = 0;
        
        speed = 0;
        turnValue = 0;

        transform.rotation = Quaternion.Euler(0,0,0);
        EndEpisode();
    }

    float map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value-fromMin)*(toMax-toMin)/(fromMax-fromMin);
    }
    

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Reset : Wall hit");
            AddReward(-10);
            Reset();
            
            
            /*
            if (Math.Abs(speed) > 0)
            {
                Debug.Log("Hit wall");
                speed = Mathf.Lerp(speed, 0, Time.deltaTime * 2);
            }
            */
        }
    }
}