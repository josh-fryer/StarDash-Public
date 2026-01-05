using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSparks : MonoBehaviour
{
    float speed = 1;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, 1 * speed * Time.deltaTime);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        //Debug.Log("Speed = " + speed);
    }
}
