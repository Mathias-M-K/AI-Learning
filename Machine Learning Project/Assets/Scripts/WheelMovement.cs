using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMovement : MonoBehaviour
{
    [Header("Wheels")] 
    public GameObject leftFrontWheel;
    public GameObject rightFrontWheel;

    public float turnMultiplier;

    [Header("Vehicle")] 
    public VehicleController vehicle;
    
    
    // Update is called once per frame
    void Update()
    {
        float turnValue = vehicle.turnValue;
        
        leftFrontWheel.transform.localRotation = Quaternion.Euler(0,turnValue*turnMultiplier,0);
        rightFrontWheel.transform.localRotation = Quaternion.Euler(0,turnValue*turnMultiplier,0);
    }
}
