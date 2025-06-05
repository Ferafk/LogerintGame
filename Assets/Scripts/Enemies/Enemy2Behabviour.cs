using TMPro;
using UnityEngine;

public class Enemy2Behabviour : MonoBehaviour
{
    #region Estados

    private enum Estados
    {
        Patrullando,
        Esperando,
        Persiguiendo
    }

    private Estados estadoActual = Estados.Patrullando;

    #endregion

    #region Variables

    [Header("Velocidades")]
    [SerializeField] private float velocidadNormal = 3f;
    [SerializeField] private float velocidadEnojado = 5f;

    [Header("Distancias")]
    [SerializeField] private Transform[] puntosPatrulla;
    [SerializeField] private float minChaseDistance = 5f;
    [SerializeField] private float minAttackDistance = 0.2f;

    [Header("Debug")]
    [SerializeField] private TextMeshPro debugText;

    private float velocidad;
    private float distanceToPlayer;
    private int indiceActual = 0;

    private bool imad = false;
    private bool imOnFire = false;

    //debug bool
    bool limiteError = false;

    #endregion

    [SerializeField] private GameObject body;
    [SerializeField] private GameObject wings;
    [SerializeField] private LayerMask playerLayer;
    private Animator cuerpoAnimator;
    private SpriteRenderer cuerpoSprite;
    private Animator alasAnimator;
    private Rigidbody2D rb;

    private Transform playerTransform;
    private Movement playerMovement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cuerpoAnimator = body.GetComponent<Animator>();
        cuerpoSprite = body.GetComponent<SpriteRenderer>();
        alasAnimator = wings.GetComponent<Animator>();
    }

    private void Update()
    {
        ActualizarEstado();
        SearchPlayer();

        cuerpoAnimator.SetBool("Mad", imad);
        alasAnimator.SetBool("Mad", imad);
        alasAnimator.SetBool("OnFire", imOnFire);

        if (debugText)
            debugText.text = estadoActual.ToString();
    }

    void ActualizarEstado()
    {
        switch (estadoActual)
        {
            case Estados.Patrullando:

                velocidad = velocidadNormal;
                imad = false;
                Patrullar();

                break;

            case Estados.Esperando:

                velocidad = 0f;
                imad = false;

                break;

            case Estados.Persiguiendo:

                velocidad = velocidadEnojado;
                imad = true;
                Perseguir();

                break;
        }
    }

    private void Patrullar()
    {
        if (puntosPatrulla.Length == 0)
        {
            if (limiteError == false)
            {
                Debug.LogError($"{name} no tiene puntos de patrulla");
                limiteError = true;
            }
            return;
        }

        if (puntosPatrulla[indiceActual] == null)
        {
            if (limiteError == false)
            {
                Debug.LogError($"{name} tiene un punto nulo en la lista de puntos");
                limiteError = true;
            }
            return;
        }

        Transform objetivo = puntosPatrulla[indiceActual];
        Vector2 direccion = (objetivo.position - transform.position).normalized;
        rb.velocity = direccion * velocidad;

        if (Vector2.Distance(transform.position, objetivo.position) < 0.2f)
        {
            indiceActual = (indiceActual + 1) % puntosPatrulla.Length;
        }

        Girar(objetivo.position.x);
    }

    private void SearchPlayer()
    {
        // Detectar al jugador en rango
        Collider2D hit = Physics2D.OverlapCircle(transform.position, minChaseDistance, playerLayer);

        if (hit != null)
        {
            playerTransform = hit.transform;
            playerMovement = playerTransform.GetComponent<Movement>();

            if (playerMovement != null && !playerMovement.isDisabled)
            {
                distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                estadoActual = Estados.Persiguiendo;
            }
            else
            {
                estadoActual = Estados.Patrullando;
            }
        }
        else
        {
            playerTransform = null;
            estadoActual = Estados.Patrullando;
        }
    }

    private void Perseguir()
    {
        if (playerTransform == null || playerMovement == null || playerMovement.isDisabled)
        {
            estadoActual = Estados.Patrullando;
            return;
        }

        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > minChaseDistance)
        {
            estadoActual = Estados.Patrullando;
            return;
        }

        Vector2 direccion = (playerTransform.position - transform.position).normalized;
        rb.velocity = direccion * velocidad;
        Girar(playerTransform.position.x);

        playerMovement.isInAttackRange = distanceToPlayer < minAttackDistance;
    }

    void Girar(float objetivoX = 0)
    {
        if (cuerpoSprite != null)
            cuerpoSprite.flipX = (transform.position.x < objetivoX);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var moveScript = collision.gameObject.GetComponent<Movement>();
            bool enLlamas = moveScript?.currentState == PlayerState.OnFire;

            imOnFire = enLlamas? true : imOnFire;

            if (imOnFire) moveScript.currentState = PlayerState.OnFire;
        }

        if (collision.gameObject.CompareTag("Fire"))
        {
            if (!imOnFire)
            {
                imOnFire = true;
                //AudioManager.instance?.PlaySFX("EnemyOnFire");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minChaseDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minAttackDistance);
    }

}
