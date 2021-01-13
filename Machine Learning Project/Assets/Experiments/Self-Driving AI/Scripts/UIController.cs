using System;
using System.Collections;
using System.Collections.Generic;
using Experiments.Scripts;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public VehicleController vehicle; 
    
    [Header("Car GUI")]
    public TextMeshProUGUI speedGUI;
    public TextMeshProUGUI rewardGUI;

    [Header("Screen GUI")] 
    public TextMeshProUGUI screenReward;
    public TextMeshProUGUI lastLapTime;
    public TextMeshProUGUI currentLapTime;

    [Header("Things")] 
    public float cumulativeReward;

    private CheckpointManager _cpm;


    private void Start()
    {
        _cpm = GetComponent<CheckpointManager>();
    }

    void Update()
    {
        float speed = vehicle.speed;
        
        speedGUI.text = $"{Math.Round(speed)} km/t"; 
        
        rewardGUI.text = vehicle.GetCumulativeReward().ToString("00.00");

        cumulativeReward = vehicle.GetCumulativeReward();

        
        
        
        //Updateing
        screenReward.text = vehicle.GetCumulativeReward().ToString("00.00");
        currentLapTime.text = (Time.time - _cpm.lapStartTime).ToString("00.00");
    }

    public void SetLastLapTime(float f)
    {
        lastLapTime.text = f.ToString("00.00");
    }
}
