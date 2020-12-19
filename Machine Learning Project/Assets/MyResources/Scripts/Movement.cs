using System;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

namespace MyResources.Scripts
{
    public class Movement : MonoBehaviour
    {

        public int speed = 10;

        public int turnSpeed = 2;

        public bool gravity = false;
        // Start is called before the first frame update
        private void Start()
        {
            Debug.Log("Hey Idiot");
        }


        private void FixedUpdate()
        {
            if(gravity) transform.Translate(Vector3.down * 9 * Time.deltaTime,Space.World);
        }

        // Update is called once per frame
        private void Update()
        {
            /*
             * Gravity
             */
            
            
            
            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                
                transform.Translate(Vector3.forward * (Time.deltaTime * speed));
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(new Vector3(0,-1,0)*turnSpeed);
            }
            
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(new Vector3(0,1,0f)*turnSpeed,Space.World);
            }
        }
    }
}
