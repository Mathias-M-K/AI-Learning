using UnityEngine;

namespace Experiments.Sorting_AI.Scripts
{
    public class BallController : MonoBehaviour
    {
        private BallColor _color;

        //Ball behavior
        public float gravity;
        private Rigidbody _rb;
        
        void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }
        
        void FixedUpdate()
        {
            _rb.AddForce(Vector3.down*gravity);
        }

        public void SetColor(BallColor color)
        {
            _color = color;
            
            //TODO Actually change the color of the ball, later.
        }

        public BallColor GetColor()
        {
            return _color;
        }
    }
    
    public enum BallColor{Red,Blue}
}
