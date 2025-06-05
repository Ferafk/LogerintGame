using UnityEngine;
using TMPro;

public class Enemy1Behabviour : MonoBehaviour
{
    #region Estados

    private enum Estados
    {
        Patrullando,
        Esperando,
        Huyendo,
        Derrotado
    }

    private Estados estadoActual = Estados.Patrullando;

    #endregion

    #region Variables

    [Header("Velocidades")]
    [SerializeField] private float velocidadNormal = 2f;
    [SerializeField] private float velocidadEnojado = 4f;

    [Header("Puntos")]
    [SerializeField] private Transform[] puntosPatrulla;
    [SerializeField] private Transform[] caminoSeguro;
    [SerializeField] private Transform puntoSeguro;
    [Space(20)]
    [SerializeField] private float distanciaMin = 0.1f;

    [Header("Tiempo de espera")]
    [SerializeField] private float esperaMin = 5f;
    [SerializeField] private float esperaMax = 10f;
    [SerializeField] private float espera = 5f;

    [Header("Debug")]
    [SerializeField] private TextMeshPro debugText;

    private float velocidad;
    private float tiempoespera;
    private float restante = 5f;

    private int indicePatrulla = 0;
    private int indiceCamino = 0;
    private int contadorAgua = 0;

    private bool avanzando = true;
    private bool puertaDetectada = false;
    private bool llegoAlFinalDelCamino = false;

    //debug bool
    bool limiteError = false;

    #endregion

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        #region Debug

        if (puntosPatrulla.Length == 0) Debug.LogWarning($"{name} sin patrulla");

        if (caminoSeguro.Length == 0) Debug.Log($"{name} sin camino seguro");

        if (puntoSeguro == null) Debug.LogWarning($"{name} sin puntoSeguro");

        #endregion

        tiempoespera = Random.Range(esperaMin, esperaMax);
        restante = espera;

        // Girar hacia primer objetivo si existe
        if (estadoActual == Estados.Patrullando && puntosPatrulla.Length > 0)
            Girar(puntosPatrulla[indicePatrulla].position.x);
        else if (estadoActual == Estados.Huyendo && caminoSeguro.Length > 0)
            Girar(caminoSeguro[indiceCamino].position.x);
    }


    void Update()
    {
        ActualizarEstado();
        animator.SetBool("Move", velocidad > 0);

        if (debugText)
            debugText.text = estadoActual.ToString();
    }


    void ActualizarEstado()
    {
        switch (estadoActual)
        {
            case Estados.Patrullando:

                velocidad = velocidadNormal;
                if (puntosPatrulla.Length > 0)
                    MoverHaciaPunto(puntosPatrulla, ref indicePatrulla, true);
                else
                    estadoActual = Estados.Esperando;

                tiempoespera -= Time.deltaTime;
                if (tiempoespera <= 0)
                {
                    estadoActual = Estados.Esperando;
                    restante = espera;
                }

                break;

            case Estados.Esperando:

                velocidad = 0;
                restante -= Time.deltaTime;
                if (restante <= 0)
                {
                    tiempoespera = Random.Range(esperaMin, esperaMax);
                    estadoActual = Estados.Patrullando;
                }

                break;

            case Estados.Huyendo:

                velocidad = velocidadEnojado;
                if (puertaDetectada)
                {
                    if (indiceCamino > 0)
                    {
                        indiceCamino--;
                        MoverHaciaPunto(caminoSeguro, ref indiceCamino);
                    }
                    else
                    {
                        avanzando = true;
                        puertaDetectada = false;
                    }
                    break;
                }

                if (!llegoAlFinalDelCamino && caminoSeguro.Length > 0)
                {
                    MoverHaciaPunto(caminoSeguro, ref indiceCamino);
                }
                else if (puntoSeguro != null)
                {
                    MoverHaciaTransform(puntoSeguro);
                }

                /*if (!puertaDetectada)
                {
                    if (!llegoAlFinalDelCamino)
                    {
                        MoverHaciaPunto(caminoSeguro, ref indiceCamino, true);
                    }
                    else if (puntoSeguro != null)
                    {
                        MoverHaciaTransform(puntoSeguro);
                    }
                }
                else // si hay puerta, retrocede
                {
                    if (indiceCamino > 0)
                    {
                        indiceCamino--;
                        MoverHaciaPunto(caminoSeguro, ref indiceCamino, true);
                    }
                    else
                    {
                        avanzando = true;
                        puertaDetectada = false;
                    }
                }*/
                break;

            case Estados.Derrotado:

                velocidad = 0;

                break;
        }
    }


    void MoverHaciaPunto(Transform[] puntos, ref int indice, bool girar = true)
    {
        if (puntos[indice] == null)
        {
            if (limiteError == false)
            {
                Debug.LogError($"{name} tiene un punto nulo en la lista de puntos");
                limiteError = true;
            }
            return;
        }

        Transform objetivo = puntos[indice];
        transform.position = Vector2.MoveTowards(transform.position, objetivo.position, velocidad * Time.deltaTime);

        if (Vector2.Distance(transform.position, objetivo.position) < distanciaMin)
        {
            switch (estadoActual)
            {
                case Estados.Patrullando:
                    indice = (indice + 1) % puntos.Length;
                    break;

                case Estados.Huyendo when avanzando && indice < puntos.Length - 1:
                    indice++;
                    break;

                case Estados.Huyendo when !avanzando && indice > 0:
                    indice--;
                    break;

                case Estados.Huyendo when avanzando && indice >= puntos.Length - 1:
                    llegoAlFinalDelCamino = true;
                    break;
            }

            if (girar)
                Girar(puntos[indice].position.x);

            //Debug.Log($"{name} está {estadoActual}. Avanzando: {avanzando}, Yendo a punto: {indice}");
        }

        /*
        if (Vector2.Distance(transform.position, objetivo.position) < distanciaMin)
        {
            // Solo avanzar si estás patrullando
            if (estadoActual == Estados.Patrullando)
            {
                indice = (indice + 1) % puntos.Length;
                if (girar) Girar(puntos[indice].position.x);
            }
            else if (estadoActual == Estados.Huyendo && avanzando && indice < puntos.Length - 1)
            {
                indice++;
                if (girar) Girar(puntos[indice].position.x);
            }
            else if (estadoActual == Estados.Huyendo && !avanzando && indice > 0)
            {
                indice--;
                if (girar) Girar(puntos[indice].position.x);
            }
            else if (estadoActual == Estados.Huyendo && avanzando)
            {
                if (indice < puntos.Length - 1)
                {
                    indice++;
                    if (girar) Girar(puntos[indice].position.x);
                }
                else
                {
                    llegoAlFinalDelCamino = true;
                }
            }
            
            Debug.Log($"{name} está {estadoActual}. Avanzando: {avanzando}, LLendo a punto: {indice}");
        }*/
    }

    void MoverHaciaTransform(Transform objetivo)
    {

        if (Vector2.Distance(transform.position, puntoSeguro.position) < distanciaMin)
        {
            estadoActual = Estados.Derrotado;
        }

        transform.position = Vector2.MoveTowards(transform.position, objetivo.position, velocidad * Time.deltaTime);
        Girar(objetivo.position.x);
    }

    void Girar(float objetivoX = 0)
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = (transform.position.x < objetivoX);
    }

    #region Colisiones

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && estadoActual != Estados.Huyendo)
        {
            var moveScript = collision.gameObject.GetComponent<Movement>();
            bool enLlamas = moveScript?.currentState == PlayerState.OnFire;
            //enllamasDetectado = moveScript?.enllamas ?? false;

            if (enLlamas)
            {
                animator.SetTrigger("Damage");
                estadoActual = Estados.Huyendo;
                indiceCamino = 0;
                llegoAlFinalDelCamino = false;
                avanzando = true;
                puertaDetectada = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("descanso") && estadoActual == Estados.Huyendo)
        {
            animator.SetTrigger("Swim");
            estadoActual = Estados.Esperando;
        }

        if (collision.CompareTag("Water"))
        {
            contadorAgua++;
            if (contadorAgua == 1)
            {
                animator.SetTrigger("Swim");
            }
        }

        if (collision.CompareTag("Puerta"))
        {
            puertaDetectada = true;
            avanzando = false;
            Debug.Log($"{name} ha detectado una puerta y regresa. Avanzando: {avanzando}");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Puerta"))
        {
            puertaDetectada = false;
        }

        if (collision.CompareTag("Water"))
        {
            contadorAgua = Mathf.Max(0, contadorAgua - 1);
            if (contadorAgua == 0)
            {
                animator.SetTrigger("Walk");
            }
        }
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < caminoSeguro.Length - 1; i++)
        {
            Gizmos.DrawLine(caminoSeguro[i].position, caminoSeguro[i + 1].position);
        }

        Gizmos.color = Color.green;
        if (puntoSeguro != null && caminoSeguro.Length > 0)
            Gizmos.DrawLine(caminoSeguro[caminoSeguro.Length - 1].position, puntoSeguro.position);
    }
}