using System.Collections;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] float minimumDistance = .2f;
    [SerializeField] float maximumTime = 1f;
    //[SerializeField] float maximumTapTime = .5f;

    [SerializeField, Range(0f, 1f)] float directionThreshold = .9f;

    private InputManager inputManager;
    private Player player;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;

    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        startTime = time;
    }

    private void SwipeEnd(Vector2 position, float time)
    {
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Vector3.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maximumTime)
        {
            // is swipe
            Debug.Log("Swiping!");
            Debug.DrawLine(startPosition, endPosition, Color.red, 5f, false);
            Vector3 direction = endPosition - startPosition;
            Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
            SwipeDirection(direction2D);
        }
    }

    void SwipeDirection(Vector2 direction)
    {
        // have else if so only one action is fired in case of diaganol swipe activating two or more swipes

        if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            //Debug.Log("Swipe LEFT");
            player.MoveWithTouch("left");
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            //Debug.Log("Swipe RIGHT");
            player.MoveWithTouch("right");
        }
    }
}
