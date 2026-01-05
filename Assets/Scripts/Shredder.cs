using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Destroy(other.transform.root.gameObject);
        other.transform.root.gameObject.SetActive(false);
    }
}
