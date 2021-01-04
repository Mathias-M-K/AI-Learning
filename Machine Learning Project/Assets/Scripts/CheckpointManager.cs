using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class CheckpointManager : MonoBehaviour
{
    public GameObject checkpoints;  //The empty gameobject containing all the checkpoints
    public TextMeshProUGUI labScreenText;

    public event Action<bool,string> OnCheckpointHit;  //Return true if the correct checkpoint is hit, false if not
    
    
    //Checkpoint variables
    private int _nrOfCheckpoints;
    public GameObject currentTarget;
    public GameObject prevTarget;

    public int errorCount;

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

        CurrentTargetCount = 0;

    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (other.gameObject == currentTarget)
            {
                CurrentTargetCount++;
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
                errorCount++;
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
        errorCount = 0;
        prevTarget = null;
        currentTarget = null;
        CurrentTargetCount = 0;
    }



    protected virtual void CheckpointHit(bool obj, string s)
    {
        OnCheckpointHit?.Invoke(obj,s);
    }
}