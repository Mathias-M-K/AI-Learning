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

    public event Action<bool,string> OnCheckpointHit;  //Return true if the correct checkpoint is hit, false if not
    
    //Lab variables
    private bool _timeStarted = false;
    private float _startTime;
    
    //Checkpoint variables
    private int _nrOfCheckpoints;
    public GameObject currentTarget;

    private int _currentTargetCount = 0;
    public int CurrentTargetCount
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

        currentTarget = checkpoints.transform.GetChild(CurrentTargetCount).gameObject;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (other.gameObject == currentTarget)
            {
                CheckpointHit(true, other.gameObject.name);
                CurrentTargetCount++;

                if (other.gameObject.name.Equals("Start"))
                {
                    if(_timeStarted) UpdateLabScreen();
                    _startTime = Time.realtimeSinceStartup;
                }
                else
                {
                    _timeStarted = true;
                }
            }
            else
            {
                CheckpointHit(false, other.gameObject.name);
                CurrentTargetCount--;
            }
        }
    }

    private void UpdateTargetCheckpoint()
    {

        if (_currentTargetCount >= _nrOfCheckpoints) _currentTargetCount = 0;
        if (_currentTargetCount == -1) _currentTargetCount = _nrOfCheckpoints-1;
        currentTarget = checkpoints.transform.GetChild(CurrentTargetCount).gameObject;
    }

    private void UpdateLabScreen()
    {
        labScreenText.text = (Time.realtimeSinceStartup - _startTime).ToString();
    }



    protected virtual void CheckpointHit(bool obj, string s)
    {
        OnCheckpointHit?.Invoke(obj,s);
    }
}