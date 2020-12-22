using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [Header("Other")]
    public float turnRate;
    public bool breaking;




    // Update is called once per frame
    void Update()
    {
        /*
         * Test. Trying to determine turn-rate by how fast the vehicle is moving
         */

        _maxTurnRateActual = map(Math.Abs(speed), 0, maxSpeed, maxTurnRate, 0);
        if (_maxTurnRateActual < 2) _maxTurnRateActual = 2;
        
        
        
        
        //Turning
        if (Input.GetKey(KeyCode.RightArrow) && Math.Abs(speed) > 0.1)
        {
            turnValue += turnRate * Time.deltaTime;
            
        } else if (Input.GetKey(KeyCode.LeftArrow) && Math.Abs(speed)  > 0.1)
        {
            turnValue -= turnRate * Time.deltaTime;
        }
        else
        {
            if (turnValue < 0) turnValue += turnRate * turnRateResetSpeed * Time.deltaTime;
            if (turnValue > 0) turnValue -= turnRate * turnRateResetSpeed * Time.deltaTime;
        }

        //Breaking and acceleration
        if (Input.GetKey(KeyCode.Space))
        {
            if (speed > 0) speed -= brakeSpeed * Time.deltaTime;
            if (speed < 0) speed += brakeSpeed * Time.deltaTime;
            breaking = true;
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
            breaking = false;
        }
        
        //Enforcing restraints
        if (speed > maxSpeed) speed = maxSpeed;
        if (speed < -maxSpeed) speed = -maxSpeed;
        if (Math.Abs(speed) < 0.1) speed = 0;
        if (turnValue > _maxTurnRateActual) turnValue = _maxTurnRateActual;
        if (turnValue < -_maxTurnRateActual) turnValue = -_maxTurnRateActual;


        //Moving vehicle
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        transform.Rotate(Vector3.up,turnRate*turnValue*Time.deltaTime);
        
        

        //TODO - Implement the rest of the car controls

        return;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }

    }
    
    float map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value-fromMin)*(toMax-toMin)/(fromMax-fromMin);
    }
}