using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VehicleUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Update is called once per frame
    void Update()
    {
        float speed = PlayerController.MyPlayerController.speed;

        if (speed == 0)
        {
            text.text = "";
        }
        else
        {
            text.text = $"{Math.Round(speed)} km/t"; 
        }
        
    }
}
