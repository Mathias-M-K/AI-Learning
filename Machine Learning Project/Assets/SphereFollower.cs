using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereFollower : MonoBehaviour
{
    public GameObject car;

    public float xOffset, yOffset, zOffset;

    // Update is called once per frame
    void Update()
    {

        Vector3 temp = car.transform.position;
        
        transform.position = new Vector3(temp.x+xOffset,temp.y+yOffset , temp.z+zOffset);
        
        Debug.Log(temp);
    }
}
