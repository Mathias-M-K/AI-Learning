﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Experiments.Sorting_AI.Scripts
{
    public class BallSpawner : MonoBehaviour
    {

        [Header("Ball")] 
        public GameObject ballStartPos;
        public GameObject ballPrefab;

        [Header("Ball Colors")] 
        public Color32[] colors;
        public string[] colorNames;
        
        [Header("Spawn Settings")]
        public float spawnRate;
        public SpawnModes currentMode = SpawnModes.Manual;
        
        private Vector3 _spawnPosition;
        private readonly List<GameObject> _activeBalls = new List<GameObject>();
        
        [Header("Next Spawn")]
        public string nextColor;
        public int nextColorInt;

        private float _lastBallSpawn;
        
        private void Awake()
        {

            _spawnPosition = ballStartPos.transform.position;
            Destroy(ballStartPos);
            
            //Generating the first ball spawn
            nextColorInt = Random.Range(0, colorNames.Length);
            nextColor = colorNames[nextColorInt];
        }

        private void Update()
        {
            if (currentMode == SpawnModes.Manual)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    SpawnNewBall();
                }
                return;
            }

            if (Time.time - _lastBallSpawn >= spawnRate)
            {
                SpawnNewBall();
                _lastBallSpawn = Time.time;
            }
        }

        private void SpawnNewBall()
        {
            GameObject go = Instantiate(ballPrefab, _spawnPosition, Quaternion.Euler(0, 0, 0));
            go.GetComponent<Renderer>().material.color = colors[nextColorInt];
            
            _activeBalls.Add(go);

            BallController newBallController = go.GetComponent<BallController>();
            newBallController.SetColor(nextColor);
            
            nextColorInt = Random.Range(0, colorNames.Length);
            nextColor = colorNames[nextColorInt];
        }
        

        /// <summary>
        /// Returns the index of the parsed color
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Index</returns>
        /// <exception cref="ArgumentException"></exception>
        public int GetColorIndex(string color)
        {
            int i = 0;
            foreach (string colorName in colorNames)
            {
                if (colorName.Equals(color)) return i;
                i++;
            }

            throw new ArgumentException($"Color not found : {color}");
        }

    }
    public enum SpawnModes{Automatic,Manual}
}
