using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2d : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadMaxI = 5f;
    [SerializeField] private float velocidadMax = 5f;
    [SerializeField] private float fireVel = 12f;
    [SerializeField] private float waterVel = 5f;
    [SerializeField] private float accel = 20f;
    [SerializeField] private float decel = 20f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSaltoI = 9f;
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private float fuezaCaida = 2.5f;
    [SerializeField] private float saltoBajo = 2f;
    [SerializeField] private float saltoAgua = 2f;
    [SerializeField] private LayerMask suelo;
    [SerializeField] private Transform gcheck;
    [SerializeField] private bool enSuelo;

    //Controladores
    private float velocidad;
    private Animator animator;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    public bool enllamas;
    private bool enrio;

    [Header("Vida")]
    [SerializeField] private float vida;
    [SerializeField] private float maxVida;
    [SerializeField] private float cura = 0.7f;
    [SerializeField] private BarraVida barraVida;

    private int lifes = 2;
    private bool revive;
    private bool perdiste = false;

    [Header("Escenas")]
    [SerializeField] private MenuControl menu;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        barraVida = GameObject.FindWithTag("Vida").GetComponent<BarraVida>();
        gcheck = transform.GetChild(0).transform;
        menu = GameObject.FindWithTag("menu").GetComponent<MenuControl>();

        velocidad = 0f;

        vida = maxVida;
        if (barraVida != null)
        {
            barraVida.InizializarBV(vida);
        }
        animator.SetInteger("Lives", lifes);
    }

    void Update()
    {
        enSuelo = Physics2D.Linecast(transform.position, gcheck.position, suelo);

        float moveX = Input.GetAxis("Horizontal");
        if (moveX != 0f)
        {
            velocidad = Mathf.MoveTowards(velocidad, velocidadMax * moveX, accel * Time.deltaTime);
        }
        else
        {
            velocidad = Mathf.MoveTowards(velocidad, 0f, decel * Time.deltaTime);
        }

        //movimiento
        rb.velocity = new Vector2(velocidad, rb.velocity.y);

        if (moveX < 0)
        {
            animator.SetBool("Move", true);
            sprite.flipX = false;
        }
        else if (moveX > 0)
        {
            animator.SetBool("Move", true);
            sprite.flipX = true;
        }
        else
        {
            animator.SetBool("Move", false);
        }

        //salto
        if (enSuelo && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
        }

        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fuezaCaida - 1) * Time.deltaTime;
        }
        else if(rb.velocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (saltoBajo - 1) * Time.deltaTime;
        }

        //vida

        if (!enllamas)
        {
            if (!perdiste)
            {
                vida += (vida < maxVida) ? cura * Time.deltaTime : 0;
            }
        }
        else
        {
            if (vida > 0)
            {
                vida -= Time.deltaTime;
            }
        }

        if (barraVida != null)
        {
            barraVida.CambiarVidaActual(vida);
        }

        if (lifes > 0)
        {
            if (vida < 0.5 && !revive)
            {
                DisminuirVida();
            }
        }
        else
        {
            if (vida < 0.5 && !revive)
            {
                Perdiste();
            }
        }

    }

    private void DisminuirVida()
    {
        velocidad = 0;
        velocidadMax = 0;
        enllamas = false;
        revive = true;
        animator.SetTrigger("Lose");
        lifes -= 1;
        animator.SetInteger("Lives", lifes);
        Invoke("Despertar", 1f);
    }
    private void Despertar()
    {
        animator.SetTrigger("Revive");
        StartCoroutine(Parpadeo());
        velocidadMax = velocidadMaxI;
        vida = 7;
        revive = false;
    }
    public void Perdiste()
    {
        velocidad = 0;
        velocidadMax = 0;
        enllamas = false;
        animator.SetTrigger("Lose");
        perdiste = true;
        menu.MovePerdiste();
    }

    IEnumerator Parpadeo()
    {
        Color colorOriginal = sprite.color;
        Color colorParpadeo = new Color(0.5f, 0.5f, 0.5f, 1f);

        for (int i = 0; i < 3; i++)
        {
            sprite.color = colorParpadeo;
            yield return new WaitForSeconds(0.1f);
            sprite.color = colorOriginal;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fire") && !enllamas)
        {
            animator.SetTrigger("Fire");
            velocidadMax = fireVel;
            enllamas = true;
        }

        if (collision.gameObject.CompareTag("Water"))
        {
            animator.SetTrigger("Roll-in");
            velocidadMax = waterVel;
            enllamas = false;
            enrio = true;
            fuerzaSalto = saltoAgua;
        }

        if (collision.gameObject.CompareTag("Corazon"))
        {
            if (lifes < 2)
            {
                lifes++;
                animator.SetInteger("Lives", lifes);
                Destroy(collision.gameObject);
                if (!enrio && !enllamas)
                {
                    animator.SetTrigger("Revive");
                }
            }
        }

        if (collision.gameObject.CompareTag("Proyectil"))
        {
            vida -= 1;
            animator.SetTrigger("Fire");
            velocidadMax = fireVel;
            enllamas = true;
            Destroy(collision.gameObject);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            vida -= Time.deltaTime * 1.3f;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            if (enrio)
            {
                animator.SetTrigger("Roll-out");
                velocidad = 0f;
                Invoke("puedeMover", 1f);
            }
            enrio = false;
            fuerzaSalto = fuerzaSaltoI;
        }
    }

    void puedeMover()
    {
        velocidadMax = velocidadMaxI;
    }

}
