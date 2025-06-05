using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemMove : MonoBehaviour
{
    [SerializeField] private float velocidadNormal = 2f;
    [SerializeField] private float velocidadEnojado = 4f;
    [SerializeField] private float velocidad;
    [SerializeField] private float distanciaMin;

    [Header("Puntos de Patrulla")]
    [SerializeField] private Transform[] puntosGuia;
    [SerializeField] private Transform puntoSeguro;

    [Header("Tiempo de espera")]
    [SerializeField] private float esperaMin = 5f;
    [SerializeField] private float esperaMax = 10f;
    [SerializeField] private float espera = 5f;

    private float tiempoespera;
    private float restante = 5f;
    private int orden = 0;
    private bool enespera = false;
    private bool enojado = false;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D box;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        box = GetComponent<BoxCollider2D>();
        if (puntosGuia.Length != 0)
        {
            Girar();
        }
        tiempoespera = Random.Range(esperaMin, esperaMax);
    }


    void Update()
    {
        if (!enespera)
        {
            velocidad = enojado ? velocidadEnojado : velocidadNormal;
        }
        else
        {
            velocidad = 0;
        }

        if (velocidad > 0)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }

        if (!enojado)
        {
            if(puntosGuia.Length != 0)
            {
                MoverHaciaPuntoDePatrulla();
            }
            else
            {
                enespera = true;
            }
            
        }
        else
        {
            MoverHaciaPuntoSeguro();
            enespera = false;
        }

        ActualizarTiempoEspera();

    }

    void MoverHaciaPuntoDePatrulla()
    {
        if (Vector2.Distance(transform.position, puntosGuia[orden].position) < distanciaMin)
        {
            orden = (orden + 1) % puntosGuia.Length;
            Girar();
        }

        transform.position = Vector2.MoveTowards(transform.position, puntosGuia[orden].position, velocidad * Time.deltaTime);
    }
    void MoverHaciaPuntoSeguro()
    {
        Girar();
        if (puntoSeguro != null)
            transform.position = Vector2.MoveTowards(transform.position, puntoSeguro.position, velocidad * Time.deltaTime);
    }
    void ActualizarTiempoEspera()
    {
        if (!enespera && !enojado)
        {
            tiempoespera -= Time.deltaTime;
        }

        if (tiempoespera <= 0)
        {
            enespera = true;
            restante -= Time.deltaTime;
            if (restante <= 0)
            {
                enespera = false;
                restante = espera;
                tiempoespera = Random.Range(esperaMin, esperaMax);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !enojado)
        {
            Movement moveScript = collision.gameObject.GetComponent<Movement>();

            bool enllamas = false;

            if (moveScript != null)
            {
                enllamas = moveScript.currentState == PlayerState.OnFire;
            }

            if (enllamas)
            {
                animator.SetTrigger("Damage");
                enojado = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("descanso"))
        {
            enespera = true;
            animator.SetTrigger("Swim");
        }
    }

    private void Girar()
    {
        if (!enojado)
        {
            if (transform.position.x < puntosGuia[orden].position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            if (transform.position.x < puntoSeguro.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }
}
