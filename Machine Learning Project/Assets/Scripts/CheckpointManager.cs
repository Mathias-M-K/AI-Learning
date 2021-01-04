using System;
using Unity.MLAgents;
using UnityEngine;


public class CheckpointManager : MonoBehaviour
{
    public GameObject checkpoints;  //The empty game-object containing all the checkpoints

    public event Action<bool,string> OnCheckpointHit;  //Return true if the correct checkpoint is hit, false if not
    public event Action<int,float> OnLapCompleted; 
    
    //Lap Variables
    public float lapStartTime;
    public int lapsCompleted = -1;

    //Checkpoint variables
    private int _nrOfCheckpoints;
    public GameObject currentTarget;
    public GameObject prevTarget;

    public int checkpointsReached;

    private int _currentTargetCount;
    
    
    //Other
    private VehicleUI _vui;

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

        CurrentTargetCount = 0;

        _vui = GetComponent<VehicleUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (other.gameObject == currentTarget)
            {
                //Log when the car crosses the finish line
                if (currentTarget.gameObject.name == "Start")
                {
                    lapsCompleted++;
                    
                    //Using the VehicleUI script to update the lap time
                    _vui.SetLastLapTime(Time.time - lapStartTime);

                    //add laptime to stats
                    if(lapStartTime != 0) LapCompleted(lapsCompleted,Time.time-lapStartTime);
                    
                    
                    lapStartTime = Time.time;
                    
                }
                
                CurrentTargetCount++;
                checkpointsReached++;
                CheckpointHit(true, other.gameObject.name);
            }
            else if(other.gameObject == prevTarget)
            {
                CurrentTargetCount--;
                CheckpointHit(false, other.gameObject.name);
            }
            else
            {
                Debug.Log("How the actual fuck");
            }
        }
    }
    

    private void UpdateTargetCheckpoint()
    {

        if (_currentTargetCount >= _nrOfCheckpoints) _currentTargetCount = 0;
        if (_currentTargetCount == -1) _currentTargetCount = _nrOfCheckpoints-1;
        
        currentTarget = checkpoints.transform.GetChild(_currentTargetCount).gameObject;

        int prevTargetIndex = GetChildPosition(currentTarget)-2;

        if (prevTargetIndex < 0)
        {
            prevTargetIndex = prevTargetIndex + _nrOfCheckpoints;
        }

        prevTarget = checkpoints.transform.GetChild(prevTargetIndex).gameObject;
    }
    

    private int GetChildPosition(GameObject child)
    {
        Transform parrent = child.transform.parent;

        int i = 0;
        foreach (Transform c in parrent.transform)
        {
            if (c.gameObject == child)
            {
                return i;
            }

            i++;
        }

        throw new Exception("There was en error finding the child index");
    }

    public void Reset()
    {
        //Checkpoint stats
        checkpointsReached = 0;
        
        //Checkpoint targets
        prevTarget = null;
        currentTarget = null;
        CurrentTargetCount = 0;
        
        //Lap
        lapStartTime = 0;
        lapsCompleted = -1;
    }



    protected virtual void CheckpointHit(bool obj, string s)
    {
        OnCheckpointHit?.Invoke(obj,s);
    }

    protected virtual void LapCompleted(int numberOfCompletedLaps, float lapTime)
    {
        OnLapCompleted?.Invoke(numberOfCompletedLaps,lapTime);
    }
}