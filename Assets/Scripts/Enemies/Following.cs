using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following : MonoBehaviour, IState
{
    public Transform searchObject;
    public Transform ownerGameObject;
    public float speed;

    public Following(Transform searchObject, Transform ownerGameObject, float speed)
    {
        this.searchObject = searchObject;
        this.ownerGameObject = ownerGameObject;
        this.speed = speed;
    }

    public void Enter()
    {

    }

    public void Execute()
    {
        ownerGameObject.position = Vector2.MoveTowards(ownerGameObject.position, searchObject.position, Time.deltaTime*speed);

        Vector3 _localscale = ownerGameObject.transform.localScale;

        if (ownerGameObject.transform.position.x < searchObject.position.x)
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
