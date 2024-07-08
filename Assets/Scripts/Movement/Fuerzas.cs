using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Fuerzas : MonoBehaviour
{
    [SerializeField] private float forceDamping = 1.2f;
    private Vector2 forceToApply;
    private Rigidbody2D _rb;
    public List<PushEffect> activePushEffects = new List<PushEffect>();
    public bool OnPush;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 moveForce = forceToApply;

        if (Mathf.Abs(forceToApply.x) <= 0.01f && Mathf.Abs(forceToApply.y) <= 0.01f)
        {
            forceToApply = Vector2.zero;
        }

        foreach (var pushEffect in activePushEffects)
        {
            forceToApply += pushEffect.flowDirection * pushEffect.riverForce * Time.fixedDeltaTime;
        }

        moveForce += forceToApply;
        forceToApply /= forceDamping;

        _rb.AddForce(moveForce);

        OnPush = activePushEffects.Count > 0;
    }

    public void ApplyForce(Vector2 force)
    {
        forceToApply += force;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & LayerMask.GetMask("River")) > 0)
        {
            OnPush = true;
            PushEffect flowDir = collision.gameObject.GetComponent<PushEffect>();
            if (flowDir != null && !activePushEffects.Contains(flowDir))
            {
                activePushEffects.Add(flowDir);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & LayerMask.GetMask("River")) > 0)
        {
            PushEffect flowDir = collision.gameObject.GetComponent<PushEffect>();
            if (flowDir != null && activePushEffects.Contains(flowDir))
            {
                activePushEffects.Remove(flowDir);
            }
        }
    }

    public void ApplyExplosionForce(Vector2 explosionPosition, float explosionForce)
    {
        Vector2 direction = (_rb.position - explosionPosition).normalized;
        ApplyForce(direction * explosionForce);
    }

    public void ApplyMeleeHitForce(Vector2 hitDirection, float hitForce)
    {
        ApplyForce(hitDirection * hitForce);
    }

    public void ApplyAttractionForce(Vector2 targetPosition, float attractionForce)
    {
        Vector2 direction = (targetPosition - _rb.position).normalized;
        ApplyForce(direction * attractionForce);
    }

}
