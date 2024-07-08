using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Patrol : MonoBehaviour, IState
{
    public Transform[] patrolPositions;
    public Transform ownerGameObject;
    public float speed;
    private int positionCount = 0;

    public Patrol(Transform[] patrolPositions, Transform ownerGameObject, float speed)
    {
        this.patrolPositions = patrolPositions;
        this.ownerGameObject = ownerGameObject;
        this.speed = speed;
    }

    public void Enter()
    {

    }

    public void Execute()
    {
        Vector2 distance = patrolPositions[positionCount].position - ownerGameObject.position;

        if (distance.magnitude < 0.5f)
        {
            positionCount++;
            if (positionCount > patrolPositions.Length - 1) positionCount = 0;
        }
        ownerGameObject.position = Vector2.MoveTowards(ownerGameObject.position, patrolPositions[positionCount].position, Time.deltaTime*speed);

        Vector3 _localscale = ownerGameObject.transform.localScale;

        if (ownerGameObject.transform.position.x < patrolPositions[positionCount].position.x)
        {
            _localscale.x = -Mathf.Abs(_localscale.x);
        }
        else
        {
            _localscale.x = Mathf.Abs(_localscale.x);
        }

        ownerGameObject.transform.localScale = _localscale;
    }

    public void Exit()
    {

    }
}
