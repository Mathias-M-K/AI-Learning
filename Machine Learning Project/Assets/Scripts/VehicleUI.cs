using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VehicleUI : MonoBehaviour
{
    public VehicleController vehicle; 
    
    [Header("Car GUI")]
    public TextMeshProUGUI speedGUI;
    public TextMeshProUGUI rewardGUI;

    [Header("Screen GUI")] 
    public TextMeshProUGUI screenReward;

    [Header("Things")] 
    public float cumulativeReward;
    // Update is called once per frame
    void Update()
    {
        float speed = vehicle.speed;
        
        speedGUI.text = $"{Math.Round(speed)} km/t"; 
        
        rewardGUI.text = vehicle.GetCumulativeReward().ToString("00.00");

        cumulativeReward = vehicle.GetCumulativeReward();

        if (screenReward != null)screenReward.text = vehicle.GetCumulativeReward().ToString("00.00");
    }
}
