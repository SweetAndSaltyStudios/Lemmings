using UnityEngine;

namespace Sweet_And_Salty_Studios
{
    public class CameraEngine : MonoBehaviour
    {
        public float MoveSpeed = 0.01f;
        private Camera mainCamera;

        public float Min_X;
        public float Max_X;
        public float Min_Y;
        public float Max_Y;        

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
            var mp = Vector2.zero;
            mp.x = horizontal * MoveSpeed;

            Vector2 tp = transform.position;
            tp += mp;

            tp.x = Mathf.Clamp(tp.x, Min_X, Max_X + 0.01f);

            transform.position = tp;         
        }

    }
}