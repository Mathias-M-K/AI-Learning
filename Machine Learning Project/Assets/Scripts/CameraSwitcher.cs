﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject driveCam;
    public GameObject reverseCam;

    // Update is called once per frame
    void Update()
    {
        float speed = PlayerController.MyPlayerController.speed;

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