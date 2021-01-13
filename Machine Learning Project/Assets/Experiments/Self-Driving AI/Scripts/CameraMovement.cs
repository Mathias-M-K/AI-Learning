using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject objToBeFollowed;
    public GameObject car;


    public float smoothingFactor = 10;
    public float cameraHeight;
    public float cameraLookHeight;
    public float cameraDistance;


    // Update is called once per frame
    void Update()
    {
        Vector3 objPos = objToBeFollowed.transform.position;
        Vector3 camPos = transform.position;
        
        
        float xPos = Mathf.Lerp(camPos.x, objPos.x, Time.deltaTime * smoothingFactor);
        float yPos = Mathf.Lerp(camPos.y, objPos.y + cameraHeight, Time.deltaTime * smoothingFactor);
        float zPos = Mathf.Lerp(camPos.z, objPos.z + cameraDistance, Time.deltaTime * smoothingFactor);
        
        transform.position = new Vector3(xPos, yPos, zPos);
    }
}