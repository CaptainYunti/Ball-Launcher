using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Rigidbody2D pivot;
    [SerializeField] float delayDuration = .1f;
    [SerializeField] float respawnDelay = 1f;

    Rigidbody2D currentBallRigidbody;
    SpringJoint2D currentBallSpringJoint;

    Camera mainCamera;
    bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidbody == null)
        {
            return;
        }

        if(Touch.activeTouches.Count == 0)
        {
            if(isDragging)
            {
                LauchBall();

            }

            isDragging = false;
            return;
        }

        isDragging = true;
        currentBallRigidbody.isKinematic = true;

        Vector2 touchPosition = new Vector2();

        foreach(Touch touch in Touch.activeTouches)
        {
            touchPosition += touch.screenPosition;
        }


        touchPosition /= Touch.activeTouches.Count;


        Vector3 worldPosition =  mainCamera.ScreenToWorldPoint(touchPosition);
        currentBallRigidbody.position = worldPosition;

    }

    private void LauchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;

        Invoke(nameof(DetachBall), delayDuration);

    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }

    void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }
}
