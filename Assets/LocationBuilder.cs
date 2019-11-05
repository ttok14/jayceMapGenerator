using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationBuilder : MonoBehaviour
{
    public float acceleration = 2;
    public float maximumSpeed = 8;

    float curSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        curSpeed += Time.deltaTime * acceleration; 
        if(curSpeed >= maximumSpeed)
        {
            curSpeed = maximumSpeed;
        }

        transform.Rotate(0, 0, curSpeed);
    }
}
