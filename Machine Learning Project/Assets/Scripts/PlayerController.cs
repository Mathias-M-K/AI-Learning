using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public static PlayerController MyPlayerController;

    private void Awake()
    {
        if (MyPlayerController == null) MyPlayerController = this;
    }
    
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
    
    //Collision values
    private bool _headOnCollisionStop;

    private void Start()
    {
        GetComponent<CheckpointManager>().OnCheckpointHit += OnCheckpointHit;
    }

    private void OnCheckpointHit(bool i)
    {
        Debug.Log($"Whatddup {i}");
    }

    // Update is called once per frame
    void Update()
    {
        //Determining turn-rate based on the current speed of the vehicle
        _maxTurnRateActual = map(Math.Abs(speed), 0, maxSpeed, maxTurnRate, 0);
        if (_maxTurnRateActual < 2) _maxTurnRateActual = 2;
        
        
        //Turning
        if (Input.GetKey(KeyCode.RightArrow) && Math.Abs(speed) > 0.1)
        {
            if (turnValue < 0)
            {
                turnValue += turnRate * turnRateResetSpeed * Time.deltaTime;
            }
            else
            {
                turnValue += turnRate * Time.deltaTime;
            }
            
        } else if (Input.GetKey(KeyCode.LeftArrow) && Math.Abs(speed)  > 0.1)
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
        if (Input.GetKey(KeyCode.Space))
        {
            if (speed > 0) speed -= brakeSpeed * Time.deltaTime;
            if (speed < 0) speed += brakeSpeed * Time.deltaTime;
            headOnCollision = true;
        }else if (Input.GetKey(KeyCode.UpArrow))
        {
            speed += accelerationSpeed * Time.deltaTime;
            
        } else if (Input.GetKey(KeyCode.DownArrow))
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

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Vector3 raycastOrigin = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        
        if (Physics.Raycast(raycastOrigin, transform.TransformDirection(Vector3.forward), out hit,raycastLength))
        {
            Debug.DrawRay(raycastOrigin, transform.TransformDirection(Vector3.forward) * raycastLength, Color.yellow);

            if (hit.collider.gameObject.tag == "Wall")
            {
                Debug.Log("Crash!");
                
                headOnCollision = true;
                if (!_headOnCollisionStop)
                {
                    speed = -speed*0.5f;
                    _headOnCollisionStop = true;
                }
            }
            else
            {
                Debug.DrawRay(raycastOrigin, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
                headOnCollision = false;
                _headOnCollisionStop = false;
            }
            
        }
        
        
        
    }
    
    float map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value-fromMin)*(toMax-toMin)/(fromMax-fromMin);
    }
    

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            if (Math.Abs(speed) > 0)
            {
                Debug.Log("Hit wall");
                speed = Mathf.Lerp(speed, 0, Time.deltaTime * 2);
            }
            
        }
    }
}