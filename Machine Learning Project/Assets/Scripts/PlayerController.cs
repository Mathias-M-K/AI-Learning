using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    
    [Header("Car Status")]
    public float turnValue = 0;
    public float speed;

    public float horizontalInput;
    public float forwardInput;
    
    [Header("SpeedValues")]
    public int maxSpeed;
    public int turnSpeed;
    public float accelerationSpeed;
    public float brakeSpeed;
    public float driftingMultiplier;

    public bool breaking;

    // Update is called once per frame
    void Update()
    {
        
        
        /*horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * maxSpeed * forwardInput);
        transform.Rotate(Vector3.up,turnSpeed*horizontalInput*Time.deltaTime);*/


        if (Input.GetKey(KeyCode.RightArrow))
        {
            turnValue += turnSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            turnValue -= turnSpeed * Time.deltaTime;
        }
        else
        {
            turnValue = 0;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            speed += accelerationSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            speed -= accelerationSpeed * Time.deltaTime;
        }else if (Input.GetKey(KeyCode.Space))
        {
            if (speed > 0) speed -= brakeSpeed * Time.deltaTime;
            if (speed < 0) speed += brakeSpeed * Time.deltaTime;
            breaking = true;
        }
        else
        {
            if(speed > 0) speed -= accelerationSpeed * Time.deltaTime;
            if(speed < 0) speed += accelerationSpeed * Time.deltaTime;
            breaking = false;
        }
        
        //Restraints
        if (speed > maxSpeed) speed = maxSpeed;
        if (speed < -maxSpeed) speed = -maxSpeed;
        if (turnValue > turnSpeed) turnValue = turnSpeed;
        if (turnValue < -turnSpeed) turnValue = -turnSpeed;
        
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        transform.Rotate(Vector3.up,turnSpeed*turnValue*Time.deltaTime);

        //TODO - Implement the rest of the car controls
        
        
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
}