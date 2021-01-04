using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardController : MonoBehaviour
{
    public static RewardController MyRewardController;

    private void Awake()
    {
        if (MyRewardController == null) MyRewardController = this;

        rewardOutput =
            $"correctCheckpoint:{correctCheckpoint}," +
            $"incorrectCheckpoint:{incorrectCheckpoint}," +
            $"wallHit:{wallHit}," +
            $"continuousPenaltyEnabled:{continuousPenaltyEnabled}," +
            $"continuousPenalty:{continuousPenalty}" +
            $"leftTrack:{leftTrack}";
    }

    [Header("Rewards")] 
    public float correctCheckpoint;
    
    [Header("Penalties")]
    public float incorrectCheckpoint;
    public float wallHit;
    public float leftTrack;

    [Header("Incentive")] public bool continuousPenaltyEnabled;
    public float continuousPenalty;

    [Header("Log")] public string rewardOutput;
}