using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharState currentState;

    public float normalSpeed;
    public float onfireSpeed;
    public float swimingSpeed;

    public float currentSpeed;

    private Rigidbody2D body;
    public Animator animator;
    private SpriteRenderer sprite;

    private Vector2 movimiento;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        currentState = new NormalState(this);
        currentState.EnterState();
        currentSpeed = normalSpeed;
    }

    private void Update()
    {
        currentState.UpdateState();
        
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        movimiento = new Vector2(moveX, moveY).normalized * currentSpeed;
        body.velocity = movimiento;

        animator.SetBool("Move", moveX != 0 || moveY != 0);

        if (moveX != 0)
        {
            sprite.flipX = moveX > 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentState.OnTriggerEnter2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentState.OnTriggerExit2D(collision);
    }

    public void TransitionToState(CharState newState)
    {
        currentState = newState;
        currentState?.EnterState();
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

}
