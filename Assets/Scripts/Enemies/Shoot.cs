using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] public GameObject proyectilPref;
    [SerializeField] private float fireSpeed = 5f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float range = 5f;
    private Transform player;
    private float next;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        
        if (Vector2.Distance(transform.position, player.position) < range)
        {
            if (Time.time > next)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                ShootFire(direction);
                next = Time.time + 1f / fireRate; 
            }
        }

    }

    void ShootFire(Vector2 direccion)
    {
        GameObject Proyectil = Instantiate(proyectilPref, transform.position, Quaternion.identity);
        Rigidbody2D rb = Proyectil.GetComponent<Rigidbody2D>();
        rb.velocity = direccion * fireSpeed;
    }

}
