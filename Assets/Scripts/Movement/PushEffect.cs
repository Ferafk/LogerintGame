using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PushEffect : MonoBehaviour
{
    public Vector2 flowDirection = new(1f, 0);
    public float riverForce = 10f;

    private Vector2 offset;
    private Material flujo;

    void Start()
    {
        flujo = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        float magnitudOffset = flujo.mainTextureOffset.magnitude;

        if (magnitudOffset < 5.0f)
        {
            offset = -flowDirection * Time.deltaTime;
        }
        else
        {
            flujo.mainTextureOffset = Vector2.zero;
        }
        flujo.mainTextureOffset += offset;
    }

    public void cambiararFlujo()
    {
        flowDirection *= -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

}
