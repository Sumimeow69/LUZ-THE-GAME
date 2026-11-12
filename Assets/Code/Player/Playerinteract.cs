using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class Playerinteract : MonoBehaviour
{

    [Header("Sonidos del almacen")]
    [SerializeField] private AudioSource fuenteAlmacen;
    [SerializeField] private AudioClip sonidoDeposito;
    [SerializeField] private AudioClip sonidoCompletado;
    [SerializeField] private AudioClip sonidoSinAjolotes;

    [SerializeField, Range(0f, 1f)]
    private float volumenAlmacen = 1f;

    private bool completandoNivel = false;

    [Header("ALMACEN SETTINGS")]

    [Tooltip("Opcional. Contador del almacen, con TextMeshPro en el mundo o en un Canvas.")]
    [SerializeField] private TMP_Text textoAlmacen;

    [SerializeField] GameObject AvisoAlmacen;
    [SerializeField] private int escenaSiguiente = 1;
    private bool almacenTrigger = false;
    private int ajolotesAlmacenados = 0;



    [Header("Sonido al recoger ajolotes")]
    [SerializeField] private AudioSource fuenteRecoleccion;
    [SerializeField] private AudioClip sonidoRecoleccion;

    [SerializeField, Range(0f, 1f)]
    private float volumenRecoleccion = 1f;

    [SerializeField]
    private float[] pitchesRecoleccion =
    {
    1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f
};


    [Tooltip("Opcional. Contador de recolectados. Puede quedar en None si la escena no lo usa.")]
    [SerializeField] private TMP_Text textoAjolotes;
    [SerializeField] private int ajolotesTotales = 4;

    private int ajolotesRecolectados = 0;
    private List<GameObject> ajolotesRecolectadosObjetos = new List<GameObject>();


    private cuadroscript cuadroActual;

    private SpriteRenderer spriteRenderer;
    private movementpj movimiento;
    private Collider2D playerCollider;


    [SerializeField] private SpriteRenderer spriteCofre;
    private Coroutine efectoCofre;

    private bool teletransportando = false;



    [Header("Entrada al cuadro")]
    [SerializeField, Min(0f)] private float demoraEntrada = 0.2f;

    private bool entrandoAlCuadro = false;

    private FilaAjolotes fila;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movimiento = GetComponent<movementpj>();
        playerCollider = GetComponent<Collider2D>();
        fila = GetComponent<FilaAjolotes>();
        ActualizarTextoAjolotes();
        ActualizarTextoAlmacen();
    }

    private void Update()
    {

        if (entrandoAlCuadro ||teletransportando ||(cuadroActual != null && cuadroActual.EnTransicion))
        {
            return;
        }


        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (cuadroActual != null)
            {
                Interactuar();
            }
            else if (almacenTrigger)
            {
                if (AvisoAlmacen != null)
                {
                    AvisoAlmacen.SetActive(false);
                }
                DepositarAjolotes();
            }


        }
        


    }
    private void DepositarAjolotes()
    {
        if (completandoNivel)
            return;

        // Caso 1: no lleva ninguno.
        if (ajolotesRecolectados <= 0)
        {
            SonarAlmacen(sonidoSinAjolotes);

            if (spriteCofre != null)
            {
                if (efectoCofre != null)
                    StopCoroutine(efectoCofre);

                efectoCofre = StartCoroutine(EfectoCofre(Color.red));
            }

            return;
        }

        ajolotesAlmacenados += ajolotesRecolectados;
        ajolotesRecolectados = 0;

        fila.Limpiar();
        ajolotesRecolectadosObjetos.Clear();

        ActualizarTextoAlmacen();

        ActualizarTextoAjolotes();

        if (spriteCofre != null)
        {
            if (efectoCofre != null)
                StopCoroutine(efectoCofre);

            efectoCofre = StartCoroutine(EfectoCofre(Color.green));
        }

        // Caso 2: con este depósito completa el total.
        if (ajolotesAlmacenados >= ajolotesTotales)
        {
            completandoNivel = true;

            SonarAlmacen(sonidoCompletado);

            StartCoroutine(CompletarNivel());
        }
        else
        {
            // Caso 3: deposita, pero todavía faltan.
            SonarAlmacen(sonidoDeposito);
        }
    }
    private void Interactuar()
    {
        if (cuadroActual == null || entrandoAlCuadro)
            return;

        if (!cuadroActual.IsInside)
        {
            StartCoroutine(EntrarAlCuadro());
        }
        else
        {
            cuadroActual.SalirDelCuadro();

            spriteRenderer.enabled = true;
            movimiento.enabled = true;
        }
    }

    private IEnumerator EntrarAlCuadro()
    {
        entrandoAlCuadro = true;

        // Guardamos el cuadro para mantener la misma referencia.
        cuadroscript destino = cuadroActual;

        // Se queda quieto, pero todavía es visible y vulnerable.
        movimiento.enabled = false;
        destino.OcultarAviso();

        yield return new WaitForSeconds(demoraEntrada);

        // Recién ahora se considera oculto.
        PerderAjolotes();
        spriteRenderer.enabled = false;

        yield return destino.EntrarConTransicion();

        entrandoAlCuadro = false;
    }

    public void TeletransportarACuadro(cuadroscript destino)
    {
        if (destino == null ||
            destino == cuadroActual ||
            teletransportando ||
            entrandoAlCuadro ||
            cuadroActual == null ||
            !cuadroActual.ListoParaTP)
        {
            return;
        }

        Transform salida = destino.GetPuntoSalida();

        if (salida == null)
            return;

        StartCoroutine(TransicionEntreCuadros(destino, salida));
    }

    private IEnumerator TransicionEntreCuadros(cuadroscript destino,Transform salida)
    {
        teletransportando = true;

        // Apagar el contenido del cuadro anterior.
        cuadroActual.DesactivarEvento();
        cuadroActual.OcultarAviso();

        // Mantener a Jesús oculto y sin movimiento.
        spriteRenderer.enabled = false;
        movimiento.enabled = false;

        // Marcar el destino antes de moverlo para mantenerlo protegido.
        cuadroActual = destino;
        destino.IsInside = true;

        transform.position = salida.position;

        // Sprite del cuadro → flechas → parpadeo y TP.
        yield return destino.EntrarConTransicion(true);

        teletransportando = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cuadro"))
        {
            cuadroscript cuadro = other.GetComponent<cuadroscript>();

            if (cuadro != null)
            {
                // Si esta haciendo un TP, no modificara cuadroActual
                if (!teletransportando)
                {
                    cuadroActual = cuadro;
                }

                // Mostrar el aviso solamente si no esta dentro
                if (cuadro != cuadroActual || !cuadro.IsInside)
                {
                    cuadro.MostrarAviso();
                }
            }
        }

        if (other.CompareTag("Almacen"))
        {
            almacenTrigger = true;

            if (AvisoAlmacen != null)
            {
                AvisoAlmacen.SetActive(true);
            }
        }

        if (other.CompareTag("Ajolote"))
        {
            ajolotesRecolectados++;

            ajolotesRecolectadosObjetos.Add(other.gameObject);

            SonarRecoleccion();

            ActualizarTextoAjolotes();

            other.gameObject.SetActive(false);


            ajolotesRecolectadosObjetos.Add(other.gameObject);
            fila.Agregar();
        }
    }

    private IEnumerator EfectoCofre(Color color)
    {
        spriteCofre.color = color;

        yield return new WaitForSeconds(0.15f);

        spriteCofre.color = Color.white;
        efectoCofre = null;
    }

    private void PerderAjolotes()
    {
        fila.Limpiar();

        foreach (GameObject ajolote in ajolotesRecolectadosObjetos)
        {
            ajolote.SetActive(true);
        }

        ajolotesRecolectadosObjetos.Clear();

        ajolotesRecolectados = 0;

        ActualizarTextoAjolotes();
    }
    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Almacen"))
        {
            almacenTrigger = false;

            if (AvisoAlmacen != null)
            {
                AvisoAlmacen.SetActive(false);
            }
        }

        if (other.CompareTag("Cuadro"))
        {
            cuadroscript cuadro = other.GetComponent<cuadroscript>();

            if (cuadro != null)
            {

                if (cuadro == cuadroActual &&!teletransportando &&!entrandoAlCuadro &&!cuadro.IsInside)
                {
                    cuadroActual.OcultarAviso();
                    cuadroActual = null;
                }
                else
                {
                    cuadro.OcultarAviso();
                }
            }
        }
    }

    public bool EstaDentroDelCuadro()
    {
        return cuadroActual != null && cuadroActual.IsInside;
    }

    private string NumeroConSprites(int numero)
    {
        string resultado = "";

        foreach (char digito in numero.ToString())
        {
            resultado += $"<sprite name=\"{digito}\">";
        }

        return resultado;
    }

    private void ActualizarTextoAjolotes()
    {
        if (textoAjolotes != null)
        {
            textoAjolotes.text = "Recolectados: " + ajolotesRecolectados + "/" +
                (ajolotesTotales - ajolotesAlmacenados);
        }
    }

    private void ActualizarTextoAlmacen()
    {
        if (textoAlmacen != null)
        {
            textoAlmacen.text =$"<voffset=0.1em>{NumeroConSprites(ajolotesAlmacenados)}  </voffset>  " +$"<space=0.2em>{NumeroConSprites(ajolotesTotales)}";
        }
    }



    private void SonarRecoleccion()
    {
        if (fuenteRecoleccion == null ||
            sonidoRecoleccion == null ||
            pitchesRecoleccion == null ||
            pitchesRecoleccion.Length == 0)
        {
            return;
        }

        int progreso = ajolotesAlmacenados + ajolotesRecolectados;
        int total = Mathf.Max(1, ajolotesTotales);

        // Primer ajolote: primera variante.
        // Último ajolote: última variante.
        float proporcion = total > 1
            ? Mathf.Clamp01((progreso - 1f) / (total - 1f))
            : 0f;

        int variante = Mathf.RoundToInt(
            proporcion * (pitchesRecoleccion.Length - 1)
        );

        fuenteRecoleccion.pitch = Mathf.Clamp(
            pitchesRecoleccion[variante],
            0.1f,
            3f
        );

        fuenteRecoleccion.PlayOneShot(
            sonidoRecoleccion,
            volumenRecoleccion
        );
    }



    private void SonarAlmacen(AudioClip clip)
    {
        if (fuenteAlmacen != null && clip != null)
        {
            fuenteAlmacen.PlayOneShot(clip, volumenAlmacen);
        }
    }

    private IEnumerator CompletarNivel()
    {
        // Esperar para que cambiar de escena no corte el sonido final.
        float espera = 0f;

        if (fuenteAlmacen != null && sonidoCompletado != null)
        {
            espera = sonidoCompletado.length /
                     Mathf.Max(0.01f, Mathf.Abs(fuenteAlmacen.pitch));
        }

        yield return new WaitForSecondsRealtime(espera);

        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaSiguiente);
    }


}
