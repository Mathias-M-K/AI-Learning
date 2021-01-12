using System.Collections;
using System.Collections.Generic;
using Experiments.Scripts;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public VehicleController vehicle;
    public GameObject driveCam;
    public GameObject reverseCam;
    public GameObject overheadCam;

    public bool overheadCamActivated;

    // Update is called once per frame
    void Update()
    {
        overheadCamActivated = Input.GetKeyDown(KeyCode.Q) ? !overheadCamActivated : overheadCamActivated;
        
        
        if (overheadCamActivated)
        {
            driveCam.SetActive(false);
            reverseCam.SetActive(false);
            overheadCam.SetActive(true);
        }
        else
        {
            overheadCam.SetActive(false);
            float speed = vehicle.speed;

            if (speed < 0)
            {
                driveCam.SetActive(false);
                reverseCam.SetActive(true);
            }
            else
            {
                driveCam.SetActive(true);
                reverseCam.SetActive(false);
            }
        }
    }
}