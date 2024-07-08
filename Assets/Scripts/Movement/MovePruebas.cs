using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class MovePruebas : MonoBehaviour
{

    [Header("Components")]

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private gCheck enSuelo;

    [Header("Floor Settings")]
    [SerializeField][Tooltip("Distance between the ground-checking colliders")] private Vector3 colliderOffset;
    [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask groundLayer;

    [Header("Movement Stats")]
    [SerializeField, Range(0f, 20f)][Tooltip("Maximum movement speed")] public float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed")] public float maxAcceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop after letting go")] public float maxDeceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction")] public float maxTurnSpeed = 80f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed when in mid-air")] public float maxAirAcceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop in mid-air when no direction is used")] public float maxAirDeceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction when in mid-air")] public float maxAirTurnSpeed = 80f;
    [SerializeField][Tooltip("Friction to apply against movement on stick")] private float friction;

    [Header("Options")]
    [Tooltip("When false, the character will skip acceleration and deceleration and instantly move and stop")] public bool useAcceleration;

    [Header("Calculations")]
    public float directionX;
    private Vector2 desiredVelocity;
    public Vector2 velocity;
    private float maxSpeedChange;
    private float acceleration;
    private float deceleration;
    private float turnSpeed;

    [Header("Current State")]
    public bool onGround;
    public bool pressingKey;
    public bool canMove;


    public float velocidad = 5f;
    //private bool enElAgua = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        enSuelo = GetComponentInChildren<gCheck>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        directionX = Mathf.Clamp(moveX, -1, 1);

        if (!canMove)
        {
            directionX = 0;
        }

        pressingKey = directionX != 0 ? true : false;

        if(directionX < 0) sprite.flipX = false;
        else if(directionX > 0) sprite.flipX=true;

        desiredVelocity = new Vector2(directionX, 0f) * Mathf.Max(maxSpeed - friction, 0f);

        onGround = enSuelo.Suelo;
    }

    private void FixedUpdate()
    {
        velocity = body.velocity;

        if (useAcceleration)
        {
            runWithAccelleration();
        }
        else
        {
            if (onGround)
            {
                runWithoutAccelleration();
            }
            else
            {
                runWithAccelleration();
            }
        }
    }

    private void runWithAccelleration()
    {
        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        deceleration = onGround ? maxDeceleration : maxAirDeceleration;
        turnSpeed = onGround ? maxTurnSpeed : maxAirTurnSpeed;

        if (pressingKey)
        {
            if (Mathf.Sign(directionX) != Mathf.Sign(velocity.x))
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }
        else
        {
            maxSpeedChange = deceleration * Time.deltaTime;
        }

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        body.velocity = velocity;

    }

    private void runWithoutAccelleration()
    {
        velocity.x = desiredVelocity.x;

        body.velocity = velocity;
    }

}
