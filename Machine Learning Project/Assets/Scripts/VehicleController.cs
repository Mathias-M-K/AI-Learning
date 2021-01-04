using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class VehicleController : Agent
{
    [Header("Controller")] 
    public float controllerX, controllerY;
    
    [Header("Car Status")] 
    public float turnValue; //Current rotation value to turn
    public float speed; //Current speed

    [Header("Speed Values")] 
    public int maxSpeed; //Max speed that vehicle can achieve
    public float brakeSpeed; //Deaccelerate value when breaking
    public float accelerationSpeed; //Acceleration value

    [Header("Turn Values")] 
    public float maxTurnRate; //Max turnValue when turning
    public float minTurnRate; //Min turnValue when turning
    public float turnRateResetSpeed; //Determines how long it will take from turning to driving straight forward
    private float _maxTurnRateActual; //Current maxTurnRate

    [Header("Other")] public float turnRate;
    private CheckpointManager _checkpointManager;
    private StatsRecorder _statsRecorder;


    [Header("AI Controls")] 
    public float turn; //1 = right, 0 = no turn, -1 = left
    public float drive; //1 = forward, 0 = no drive, -1 = reverse
    public Vector3 originPos;
    
    public override void Initialize()
    {
        _statsRecorder = Academy.Instance.StatsRecorder;
        
        _checkpointManager = GetComponent<CheckpointManager>();
        _checkpointManager.OnCheckpointHit += OnCheckpointHit;
        _checkpointManager.OnLapCompleted += OnLapCompleted;
        
        originPos = transform.position;
    }

    

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Continous Penalty to make it go faster!
        AddReward(RewardController.MyRewardController.continuousPenalty);

        //turn
        turn = actions.ContinuousActions[0];

        //drive
        drive = actions.ContinuousActions[1];
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;

        //ContinuousActions[0] = Turn
        actions[0] = Input.GetAxis("Horizontal");

        //ContinuousActions[1] = Drive
        actions[1] = map(Input.GetAxis("Fire1"), 0, 1, -1, 1);
    }

    public override void OnEpisodeBegin()
    {
        transform.position = originPos;
    }

    private void OnCheckpointHit(bool i, string s)
    {
        if (i)
        {
            AddReward(RewardController.MyRewardController.correctCheckpoint);
        }
        else
        {
            AddReward(RewardController.MyRewardController.incorrectCheckpoint);
            Debug.Log("Reset : Wrong Way");
            Reset();
        }
    }
    
    private void OnLapCompleted(int completedLaps, float lapTime)
    {
        Debug.Log($"OnLapCompleted:{completedLaps},{lapTime}");
        
        if (completedLaps >= 3)
        {
            AddReward(RewardController.MyRewardController.episodeCompleted);
            Reset();
        }
        
        _statsRecorder.Add("MyStats/Lap time",lapTime);
    }

    // Update is called once per frame
    void Update()
    {
        controllerX = Input.GetAxis("Horizontal");
        controllerY = Input.GetAxis("Fire1");

        //Determining turn-rate based on the current speed of the vehicle
        _maxTurnRateActual = map(Math.Abs(speed), 0, maxSpeed, maxTurnRate, 0);
        if (_maxTurnRateActual < 2) _maxTurnRateActual = 2;


        if (transform.position.y < -10)
        {
            Debug.Log("Reset : Left Track");
            AddReward(RewardController.MyRewardController.leftTrack);
            Reset();
        }
        /*
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
        }*/

        //Breaking and acceleration
        /*
        if (drive == 1)
        {
            speed += accelerationSpeed * Time.deltaTime;
            
        } else {
            if (speed > 0) speed -= brakeSpeed * Time.deltaTime;
            if (speed < 0) speed += brakeSpeed * Time.deltaTime;
            if (Math.Abs(speed) < 0.5f) speed = 0;
        }*/

        //Prototype implementation of controller support

        float targetTurnValue = map(turn, -1, 1, -maxTurnRate, maxTurnRate);


        if (targetTurnValue == 0)
        {
            if (Math.Abs(turnValue) < 1)
            {
                turnValue = Mathf.SmoothStep(turnValue, 0, Time.deltaTime * 3);
            }
            else
            {
                if (turnValue < 0) turnValue += turnRate * turnRateResetSpeed * Time.deltaTime;
                if (turnValue > 0) turnValue -= turnRate * turnRateResetSpeed * Time.deltaTime;
            }
        }
        else if (targetTurnValue > turnValue && Math.Abs(speed) > 0.1)
        {
            if (turnValue < 0)
            {
                turnValue += turnRate * turnRateResetSpeed * Time.deltaTime;
            }
            else
            {
                turnValue += turnRate * Time.deltaTime;
            }
        }
        else if (targetTurnValue < turnValue && Math.Abs(speed) > 0.1)
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

        if (targetTurnValue == 0)
        {
            if (Math.Abs(turnValue) < 1)
            {
                turnValue = 0;
            }
            else
            {
                if (turnValue < 0) turnValue += turnRate * turnRateResetSpeed * Time.deltaTime;
                if (turnValue > 0) turnValue -= turnRate * turnRateResetSpeed * Time.deltaTime;
            }
        }


        float targetSpeed = map(drive, -1, 1, 0, maxSpeed);

        if (speed < targetSpeed)
        {
            speed += accelerationSpeed * Time.deltaTime;
        }
        else
        {
            if (speed > 0) speed -= brakeSpeed * Time.deltaTime;
            if (speed < 0) speed += brakeSpeed * Time.deltaTime;
            if (Math.Abs(speed) < 0.5f) speed = 0;
        }


        //Enforcing restraints
        if (speed > maxSpeed) speed = maxSpeed;
        if (speed < -maxSpeed) speed = -maxSpeed;
        if (turnValue > _maxTurnRateActual) turnValue = _maxTurnRateActual;
        if (turnValue < -_maxTurnRateActual) turnValue = -_maxTurnRateActual;


        //Moving vehicle
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        transform.Rotate(Vector3.up, turnRate * turnValue * Time.deltaTime);

        //TODO - Have vehicle come to full stop if hitting a wall head on
    }

    public void Reset()
    {
        transform.position = originPos;
        
        _statsRecorder.Add("MyStats/Checkpoints Reached", _checkpointManager.checkpointsReached);
        
        _checkpointManager.Reset();
        drive = 0;
        turn = 0;

        speed = 0;
        turnValue = 0;

        transform.rotation = Quaternion.Euler(0, 0, 0);
        EndEpisode();
    }

    float map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }


    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Reset : Wall hit");
            AddReward(RewardController.MyRewardController.wallHit);
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