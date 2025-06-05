using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#region Estados

public enum PlayerState
{
    Normal,
    OnFire,
    InWater,
    Reviving,
    Dead
}

#endregion

public class Movement : MonoBehaviour
{
    [Header("Movimiento del personaje")]
    [SerializeField] private float velocidadInicial = 10f;
    [SerializeField] public float aceleracion = 15f;
    [SerializeField] public float desacelerar = 5f;

    [Header("Vida")]
    public float vida;
    [SerializeField] private float maxVida = 10f;
    [SerializeField] private BarraVida barraVida;

    [Header("Escenas")]
    [SerializeField] private MenuControl menu;

    [Header("Debug")]
    [SerializeField] private TextMeshPro debugText;

    private int lifes;

    private float velocidad;
    private bool istakingDamage = false;
    private Animator animador;
    private SpriteRenderer sprite;
    private Rigidbody2D body;

    public PlayerState currentState = PlayerState.Normal;
    private int waterZoneCount = 0;

    [HideInInspector] public bool isDisabled = false;
    [HideInInspector] public bool isInAttackRange = false;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animador = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        menu = GameObject.FindWithTag("menu")?.GetComponent<MenuControl>();
        barraVida = GameObject.FindWithTag("Vida")?.GetComponent<BarraVida>();

        velocidad = velocidadInicial;
        vida = maxVida;
        lifes = Puntaje.Instance != null ? Puntaje.Instance.vidas : 2;

        barraVida?.InizializarBV(vida);
        animador.SetInteger("Lives", lifes);
    }

    
    void Update()
    {
        if (currentState == PlayerState.Dead) return;

        HandleInput();
        HandleHealth();

        if (debugText)
            debugText.text = currentState.ToString();
    }

    void HandleInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 input = new Vector2(moveX, moveY).normalized;

        switch (currentState)
        {
            case PlayerState.Normal:
                body.velocity = input * velocidad;
                isDisabled = false;
                if (moveX != 0)
                {
                    sprite.flipX = moveX > 0;
                }
                animador.SetBool("Move", moveX != 0 || moveY != 0);
                break;
            case PlayerState.OnFire:
                isInAttackRange = true;
                velocidad = aceleracion;
                animador.SetTrigger("Fire");
                //AudioManager.instance?.SetCombat(true);
                body.velocity = input * velocidad;
                if (moveX != 0) sprite.flipX = moveX > 0;
                break;
            case PlayerState.InWater:
                isInAttackRange = false;
                isDisabled = true;
                body.velocity = input * velocidad;
                if (moveX != 0) sprite.flipX = moveX > 0;
                break;
            case PlayerState.Reviving:
            case PlayerState.Dead:
                isInAttackRange = false;
                isDisabled = true;
                body.velocity = Vector2.zero;
                break;
        }
    }

    void HandleHealth()
    {
        if (istakingDamage)
            vida -= Time.deltaTime;
        else if (vida < maxVida && currentState != PlayerState.Dead)
            vida += Time.deltaTime;

        barraVida?.CambiarVidaActual(vida);

        if (vida <= 0.1f && currentState != PlayerState.Dead)
        {
            if (lifes > 0) TriggerRevive();
            else TriggerGameOver();
        }

        if (currentState == PlayerState.OnFire || isInAttackRange)
        {
            istakingDamage = true;
        }
        else
        {
            istakingDamage = false;
        }
    }

    void TriggerRevive()
    {
        currentState = PlayerState.Reviving;
        animador.SetTrigger("Lose");
        
        if (Puntaje.Instance != null)
        {
            Puntaje.Instance.QuitarVida();
            lifes = Puntaje.Instance.vidas;
        }
        else
        {
            lifes--;
        }
        
        animador.SetInteger("Lives", lifes);
        Invoke(nameof(Despertar), 1f);
    }

    void TriggerGameOver()
    {
        currentState = PlayerState.Dead;
        animador.SetTrigger("Lose");
        menu?.MovePerdiste();
    }

    private void Despertar()
    {
        velocidad = 0;
        animador.SetTrigger("Revive");
        StartCoroutine(Parpadeo());
        vida = maxVida;
        AudioManager.instance?.SetCombat(false);
        Invoke("puedeMover", 1f);
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
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            if (waterZoneCount == 0)
            {
                animador.SetTrigger("Roll-in");
                AudioManager.instance?.PlaySFX("WaterStep");
            }

            waterZoneCount++;
            velocidad = desacelerar;

            if (currentState == PlayerState.OnFire)
            {
                currentState = PlayerState.InWater;
                AudioManager.instance?.SetCombat(false);
            }
            else if (currentState == PlayerState.Normal)
            {
                currentState = PlayerState.InWater;
            }

        }


        if (collision.gameObject.CompareTag("Fire") && currentState != PlayerState.OnFire && currentState != PlayerState.Reviving && currentState != PlayerState.Dead)
        {
            currentState = PlayerState.OnFire;
        }

        if (collision.gameObject.CompareTag("Corazon"))
        {
            if (lifes < 2)
            {
                AudioManager.instance?.PlaySFX("Coin");
                Puntaje.Instance.AgregarVida();
                lifes = Puntaje.Instance.vidas;
                animador.SetInteger("Lives", lifes);
                Destroy(collision.gameObject);
                if (currentState == PlayerState.Normal)
                {
                    velocidad = 0;
                    animador.SetTrigger("Revive");
                    Invoke(nameof(puedeMover), 1f);
                }
            }
        }

        if (collision.CompareTag("Moneda"))
        {
            Puntaje.Instance.AgregarMoneda(1);
            AudioManager.instance?.PlaySFX("Coin");
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            velocidad = desacelerar;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            waterZoneCount = Mathf.Max(0, waterZoneCount - 1);
            if (waterZoneCount == 0)
            {
                animador.SetTrigger("Roll-out");
                currentState = PlayerState.Normal;
                velocidad = 0;
                Invoke(nameof(puedeMover), 1f);
            }
        }
    }

    void puedeMover()
    {
        velocidad = velocidadInicial;
        currentState = PlayerState.Normal;
    }

}
