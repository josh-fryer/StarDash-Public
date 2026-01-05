using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region variables
    [SerializeField] bool godMode = false;
    [SerializeField] GameObject engines;

    [Header("Death Explosion")]
    [SerializeField] GameObject explosion;
    [SerializeField] AudioClip explosionSfx;
    [Range(0f, 1f)]
    [SerializeField] float explosionVol = 1f;

    [Header("Movement Settings")]
    [SerializeField] private AnimationCurve _curve; // set speed curve for side movement;
    [SerializeField] private float lerpDuration = 1f;
    [SerializeField] private float speed = 2f;

    private KeyboardInput keyboardInputActions;
    private InputAction moveWithKeys;

    public AudioClip shipMoveSfx;
    [Range(0f, 1f)]
    [SerializeField] float shipMoveVol = 1f;

    [Header("Gun Setttings")]
    public GameObject laser;
    public AudioClip laserSfx;
    //private float timeWhenAllowedNextShoot = 0f;
    [SerializeField] private float timeBetweenShooting = 2f;
    [SerializeField] Transform firePoint;

    private Animator animator;
    private GameMaster gameMaster;
    private Vector3 leftLane = new Vector3(-2, 0, 0);
    private Vector3 rightLane = new Vector3(2, 0, 0);
    private Vector3 centerLane = Vector3.zero;

    private Vector3 targetPos;
    private Vector3 movePos;
    private bool moving = false;
    private float lerpPct = 0f;
    private Vector3 camPos;
    #endregion


    private void OnEnable()
    {
        if (keyboardInputActions == null)
        { 
            keyboardInputActions = new KeyboardInput(); 
        }
        moveWithKeys = keyboardInputActions.Player.Move;
        moveWithKeys.Enable();
    }

    private void OnDisable()
    {
        if (keyboardInputActions == null)
        {
            keyboardInputActions = new KeyboardInput();
        }
        moveWithKeys.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        keyboardInputActions = new KeyboardInput();
        // store centerlane and add any y or z axis changes
        Vector3 startPos = transform.position;
        centerLane += startPos;
        leftLane.y += startPos.y;
        rightLane.y += startPos.y;
        leftLane.z += startPos.z;
        rightLane.z += startPos.z;

        gameMaster = GameMaster.Instance;
        animator = GetComponent<Animator>();
        camPos = gameMaster.camPos;

    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            lerpPct += Time.deltaTime * speed;
            float t = lerpPct / lerpDuration;

            transform.position = Vector3.Lerp(movePos, targetPos, _curve.Evaluate(t)); // divide by the seconds i want it to last for;
            if (lerpPct >= 1f)
            {
                transform.position = targetPos;
                lerpPct = 0f;
                moving = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Ship Collided with: " + other.gameObject.name);

    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Obstacle") && !godMode)
        {
            Death();
        }
    }

    private void Death()
    {
        gameMaster.OnDeath();
        //engines.SetActive(false);
        gameObject.SetActive(false);
        GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionSfx, camPos, explosionVol);
        Destroy(newExplosion, 3f);   
    }

    public void SetAliveAgain()
    {
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
    }

    // Input for development
    public void Move(InputAction.CallbackContext context)
    {
        //Debug.Log("Move!");
        if (moving) return;

        bool isLeftKeyDown = false;
        bool isRightKeyDown = false;

        var move = context.ReadValue<Vector2>().x;
        //Debug.Log("move vector2.x = "+ context.ReadValue<Vector2>());
        // is left
        if (move < 0)
       {
            isLeftKeyDown = true;
       }
       else if (move > 0)
        {
            isRightKeyDown = true;
        }
       

        if ((isRightKeyDown || isLeftKeyDown) && transform.position.x == 0)
        {
            // move to right or left lane
            movePos = centerLane;
            if (isLeftKeyDown)
            {
                targetPos = leftLane;
                animator.SetTrigger("turn left");
            }
            else
            {
                targetPos = rightLane;
                animator.SetTrigger("turn right");
            }

            moving = true;
            AudioSource.PlayClipAtPoint(shipMoveSfx, camPos, shipMoveVol);
        }
        else if (isRightKeyDown && transform.position.x == -2)
        {
            // move right
            movePos = leftLane;
            targetPos = centerLane;
            animator.SetTrigger("turn right");
            moving = true;
            AudioSource.PlayClipAtPoint(shipMoveSfx, camPos, shipMoveVol);
        }
        else if (isLeftKeyDown && transform.position.x == 2)
        {
            // move left
            movePos = rightLane;
            targetPos = centerLane;
            animator.SetTrigger("turn left");
            moving = true;
            AudioSource.PlayClipAtPoint(shipMoveSfx, camPos, shipMoveVol);

        }

    }


    public void MoveWithTouch(string direction)
    {
        if (moving) return;

        if (transform.position.x == 0)
        {
            moving = true;
            // move to right or left lane
            movePos = centerLane;
            if (direction == "left")
            {
                targetPos = leftLane;
                animator.SetTrigger("turn left");
            }
            else if (direction == "right")
            {
                targetPos = rightLane;
                animator.SetTrigger("turn right");
            }

            AudioSource.PlayClipAtPoint(shipMoveSfx, camPos, shipMoveVol);
        }
        else if (direction == "right" && transform.position.x == -2)
        {
            // move right
            moving = true;
            movePos = leftLane;
            targetPos = centerLane;
            animator.SetTrigger("turn right");

            AudioSource.PlayClipAtPoint(shipMoveSfx, camPos, shipMoveVol);
        }
        else if (direction == "left" && transform.position.x == 2)
        {
            // move left
            moving = true;
            movePos = rightLane;
            targetPos = centerLane;
            animator.SetTrigger("turn left");
            AudioSource.PlayClipAtPoint(shipMoveSfx, camPos, shipMoveVol);

        }

    }

    public void Fire(InputAction.CallbackContext context)
    {
        if(!gameMaster.isAlive)
        {
            Debug.Log("player is dead, do not Fire()");
            return;
        }

        if (context.started)
        {
            //GameObject laserInst = Instantiate(laser, firePoint.position, laser.transform.rotation);
            GameObject laserInst = ObjectPool.SharedInstance.GetPooledLaserObject();
            if (laserInst != null)
            {
                laserInst.transform.position = firePoint.position;
                laserInst.transform.rotation = laser.transform.rotation;
                laserInst.SetActive(true);
            }

            if (laserSfx != null)
            {
                AudioSource.PlayClipAtPoint(laserSfx, camPos, 1f);
            }
        }
    }
}
