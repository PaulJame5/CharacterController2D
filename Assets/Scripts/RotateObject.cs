using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TwoDTools
{
    public class RotateObject : MonoBehaviour
    {
        public float speed;
        Transform myTransform;

        // Start is called before the first frame update
        void Start()
        {
            myTransform = transform;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 current = myTransform.localEulerAngles;
            current.z += speed * Time.deltaTime;
            if(current.z >= 360)
            {
                current.z = 0;
            }

            myTransform.localEulerAngles = current;
        }
    }
}
