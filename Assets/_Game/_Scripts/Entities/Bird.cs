using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    #region SerializeFields

    [SerializeField]
    private Vector2 minPower;

    [SerializeField]
    private Vector2 maxPower;

    [SerializeField]
    private float force;

    #endregion

    #region Varriables

    private Vector2 maxDistance;
    private Rigidbody2D rb;
    private SpringJoint2D springJoint;
    private LineRenderer lineRenderer;
    private Vector2 startTouchPosition;

    #endregion

    #region Unity Methods

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        springJoint = GetComponent<SpringJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        rb.velocity = Vector2.zero;

        GetComponent<SpringJoint2D>().enabled = false;
        GameManager.Instance.GameStateChanged += OnGameStateChanged;
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        var destroyable = other.gameObject.GetComponent<DestroyableBase>();
        if (destroyable != null)
        {
            destroyable.Hit(rb.velocity.magnitude);
        }
    }

    #endregion

    #region Methods
    public void Register(Rigidbody2D throwPoint)
    {
        GetComponent<SpringJoint2D>().enabled = true;
        GetComponent<SpringJoint2D>().connectedBody = throwPoint;

        InputController.Instance.Clicked += OnClicked;
    }

    private Vector2[] Plot(Vector2 position, Vector2 velocity, int steps)
    {
        Vector2[] result = new Vector2[steps];

        float timeStep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * rb.gravityScale * timeStep * timeStep;

        float drag = 1f - timeStep * rb.drag;
        Vector2 moveStep = velocity * timeStep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            position += moveStep;
            result[i] = position;
        }

        return result;
    }

    private void SetTrajectoryPositions(Vector2 touchPosition, Vector2 startTouchPosition)
    {
        Vector2 velocity = (startTouchPosition - touchPosition).normalized * force * Vector2.Distance(touchPosition, startTouchPosition);
        Vector2[] trajector = Plot((Vector2)transform.position, velocity, 500);
        lineRenderer.positionCount = trajector.Length;
        Vector3[] positions = new Vector3[trajector.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = trajector[i];
        }
        lineRenderer.SetPositions(positions);
    }

    #endregion

    #region Callbacks
    private void OnClicked(TouchPhase phase, Vector2 touchPosition)
    {
        switch (phase)
        {
            case TouchPhase.Began:
                startTouchPosition = touchPosition;
                lineRenderer.enabled = true;
                break;
            case TouchPhase.Moved:
                maxDistance = new Vector2
                (
                    Mathf.Clamp(touchPosition.x, minPower.x, maxPower.x),
                    Mathf.Clamp(touchPosition.y, minPower.y, maxPower.y)
                );
                transform.position = maxDistance;
                SetTrajectoryPositions(touchPosition, startTouchPosition);
                break;
            case TouchPhase.Stationary:
                transform.position = maxDistance;
                break;
            case TouchPhase.Ended:
                rb.velocity = ((startTouchPosition - touchPosition).normalized * force * Vector2.Distance(touchPosition, startTouchPosition));
                BirdController.Instance.BirdCount = -1;
                lineRenderer.enabled = false;
                springJoint.enabled = false;
                InputController.Instance.Clicked -= OnClicked;
                GameManager.Instance.UpdateGameState(GameStates.Unclickable);
                break;

        }

    }
    private void OnGameStateChanged(GameStates newState)
    {
        if (newState == GameStates.Success)
        {
            BirdController.Instance.BirdCount = 1;

        }
    }

    #endregion
}
