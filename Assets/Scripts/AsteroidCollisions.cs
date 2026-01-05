using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidCollisions : MonoBehaviour
{
    [HideInInspector]
    public bool isBreakable = false; // set at start in parent
    private Asteroid parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.root.gameObject.GetComponent<Asteroid>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Laser" && isBreakable)
        {
            parent.health -= 1;
        }
    }
}
