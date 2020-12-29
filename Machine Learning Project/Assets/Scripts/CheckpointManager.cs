using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject checkpoints;  //The empty gameobject containing all the checkpoints

    public TextMeshProUGUI labScreenText;
    
    
    //Lab variables
    private bool timeStarted = false;
    private float startTime;
    
    
    //Checkpoint variables
    private int _nrOfCheckpoints;
    private GameObject _currentTarget;

    private int _currentTargetCount = 0;
    private int CurrentTargetCount
    {
        get => _currentTargetCount;
        set
        {
            _currentTargetCount = value;
            UpdateTargetCheckpoint();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _nrOfCheckpoints = checkpoints.transform.childCount;
        Debug.Log($"Number of checkpoints: {_nrOfCheckpoints}");

        _currentTarget = checkpoints.transform.GetChild(CurrentTargetCount).gameObject;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            Debug.Log(Time.realtimeSinceStartup);
            
            if (other.gameObject == _currentTarget)
            {
                CurrentTargetCount++;

                if (other.gameObject.name.Equals("Start"))
                {
                    if(timeStarted) UpdateLabScreen();
                    startTime = Time.realtimeSinceStartup;
                }
                else
                {
                    timeStarted = true;
                }
            }
            else
            {
                CurrentTargetCount--;
            }
        }
    }

    private void UpdateTargetCheckpoint()
    {

        if (_currentTargetCount >= _nrOfCheckpoints) _currentTargetCount = 0;
        _currentTarget = checkpoints.transform.GetChild(CurrentTargetCount).gameObject;
    }

    private void UpdateLabScreen()
    {
        labScreenText.text = (Time.realtimeSinceStartup - startTime).ToString();
    }
    
}