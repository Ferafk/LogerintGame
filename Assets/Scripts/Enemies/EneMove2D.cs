using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EneMove2D : MonoBehaviour
{
    [SerializeField] private IEnemies stateMachine;
    [SerializeField] private Patrol patrolState;
    [SerializeField] private Following followState;

    [SerializeField] private float enemySpeed = 5f;
    [SerializeField] private Transform[] patrolPos;
    [SerializeField] private float minChaseDistance = 5f;

    private Animator[] animators;
    private bool Imad = false;
    private Transform goingObject;
    private float distance;

    void Start()
    {
        goingObject = GameObject.FindGameObjectWithTag("Player").transform;
        animators = GetComponentsInChildren<Animator>();

        if (goingObject != null)
        {
            Patrol();
        }
        else
        {
            Debug.Log("No se encontró al Player");
        }
    }

    void Update()
    {

        if (goingObject != null)
        {
            distance = (transform.position - goingObject.position).magnitude;
        }
        else
        {
            distance = minChaseDistance + 1;
        }

        if (distance < minChaseDistance && goingObject != null)
        {
            Follow();
        }
        else if (stateMachine.currentlyRunningState != null)
        {
            if (stateMachine.currentlyRunningState.ToString() != "Patrol")
            {
                Patrol();
            }
        }
        stateMachine.ExecuteStateUpdate();


        foreach (Animator animator in animators)
        {
            animator.SetBool("Mad", Imad);
        }
    }

    private void Patrol()
    {
        if (patrolState != null)
        {
            patrolState.patrolPositions = patrolPos;
            patrolState.ownerGameObject = transform;
            patrolState.speed = enemySpeed;

            stateMachine.ChangeState(patrolState);
        }
        Imad = false;
    }

    private void Follow()
    {
        if (followState != null)
        {
            followState.searchObject = goingObject;
            followState.ownerGameObject = transform;
            followState.speed = enemySpeed * 2;

            stateMachine.ChangeState(followState);
        }
        Imad = true;
    }
}
