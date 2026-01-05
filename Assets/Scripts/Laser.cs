using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float speed = 100f;
    [SerializeField] float maxDistance = 180.0f;
    [SerializeField] float sparksZAxisOffset = 1.2f;
    [SerializeField] GameObject sparks;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // pos.z += speed * Time.deltaTime;
        // transform.position = pos;
        if (transform.position.z >= maxDistance)
        {
            //Destroy(gameObject);
            ReturnToObjectPool();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 direction = new Vector3(0f, 0f, 1);
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            //Debug.Log("Laser Collision with: " + other.name);
            // spawn laser sparks
            Vector3 sparksPos = new Vector3(transform.position.x, transform.position.y, (transform.position.z - sparksZAxisOffset));
            GameObject newSparks = Instantiate(sparks, sparksPos, sparks.transform.rotation);
            newSparks.GetComponent<LaserSparks>().SetSpeed(
                other.transform.root.gameObject.GetComponent<Asteroid>().speed);

            //Destroy(gameObject);
            ReturnToObjectPool();
        }
    }

    private void ReturnToObjectPool()
    {
        gameObject.SetActive(false);
    }
}
