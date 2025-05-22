using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movimiento del personaje")]
    [SerializeField] private float velocidadInicial = 10f;
    [SerializeField] public float aceleracion = 15f;
    [SerializeField] public float desacelerar = 5f;

    [Header("Vida")]
    [SerializeField] private float vida;
    [SerializeField] private float maxVida = 10f;
    [SerializeField] private BarraVida barraVida;

    [Header("Escenas")]
    [SerializeField] private MenuControl menu;

    private int lifes;
    private bool perdiste = false;

    private float velocidad;
    private Vector2 movimiento;
    private Animator animador;
    private SpriteRenderer sprite;
    private Rigidbody2D body;
    private Fuerzas _fuerzas;
    public bool enllamas;
    private bool _revive;
    private int enrio;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animador = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        menu = GameObject.FindWithTag("menu").GetComponent<MenuControl>();
        barraVida = GameObject.FindWithTag("Vida").GetComponent<BarraVida>();
        _fuerzas = GetComponent<Fuerzas>();

        velocidad = velocidadInicial;
        vida = maxVida;
        lifes = Puntaje.Instance.vidas;

        barraVida?.InizializarBV(vida);
        animador.SetInteger("Lives", lifes);
    }

    
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        movimiento = new Vector2(moveX, moveY).normalized * velocidad;
        body.velocity = movimiento;

        if (enllamas)
        {
            if (vida > 0)
            {
                vida -= Time.deltaTime;
            }
        }
        else if (!perdiste)
        {
            vida += (vida < maxVida) ? Time.deltaTime : 0;
        }

        barraVida?.CambiarVidaActual(vida);

        if (moveX != 0)
        {
            sprite.flipX = moveX > 0;
        }

        animador.SetBool("Move", moveX != 0 || moveY != 0);

        if (lifes > 0)
        {
            if (vida < 1 && !_revive)
            {
                DisminuirVida();
            }
        }
        else
        {
            if (vida < 0.1)
            {
                Perdiste();
            }
        }

    }

    private void DisminuirVida()
    {
        velocidad = 0;
        enllamas = false;
        animador.SetTrigger("Lose");
        Puntaje.Instance.QuitarVida();
        lifes = Puntaje.Instance.vidas;
        animador.SetInteger("Lives", lifes);
        Invoke("Despertar", 1f);
    }
    private void Despertar()
    {
        velocidad = 0;
        animador.SetTrigger("Revive");
        _revive = true;
        StartCoroutine(Parpadeo());
        vida = maxVida;
        AudioManager.instance.SetCombat(false);
        Invoke("puedeMover", 1f);
    }
    public void Perdiste()
    {
        velocidad = 0;
        enllamas = false;
        animador.SetTrigger("Lose");
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
        sprite.color = colorOriginal;
        _revive = false;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fire") && !enllamas && enrio==0 && !_revive)
        {
            animador.SetTrigger("Fire");
            velocidad = aceleracion;
            enllamas = true;
            AudioManager.instance.SetCombat(true);
        }

        if (collision.gameObject.CompareTag("Water"))
        {
            if (enrio == 0)
            {
                animador.SetTrigger("Roll-in");
                AudioManager.instance.PlaySFX("WaterStep");
            }
            velocidad = desacelerar;
            if (enllamas)
            {
                enllamas = false;
                AudioManager.instance.SetCombat(false);
            }
            enrio++;
        }

        if (collision.gameObject.CompareTag("Corazon"))
        {
            if (lifes < 2)
            {
                AudioManager.instance.PlaySFX("Coin");
                Puntaje.Instance.AgregarVida();
                lifes = Puntaje.Instance.vidas;
                animador.SetInteger("Lives", lifes);
                Destroy(collision.gameObject);
                if (enrio == 0 && !enllamas)
                {
                    velocidad = 0;
                    animador.SetTrigger("Revive");
                    Invoke("puedeMover", 1f);
                }
            }
        }

        if (collision.CompareTag("Moneda"))
        {
            Puntaje.Instance.AgregarMoneda(1);
            AudioManager.instance.PlaySFX("Coin");
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            vida -= Time.deltaTime * 1.3f;
        }

        if (collision.gameObject.CompareTag("Water"))
        {
            velocidad = desacelerar;
            enllamas = false;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            if (enrio == 1)
            {
            animador.SetTrigger("Roll-out");
            velocidad = 0f;
            Invoke("puedeMover", 1f);
            }
            enrio--;
        }
    }

    void puedeMover()
    {
        velocidad = velocidadInicial;
    }

}
