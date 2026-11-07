using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class Playerinteract : MonoBehaviour
{
    [SerializeField] private TMP_Text textoAlmacen;

    [SerializeField] GameObject AvisoAlmacen;
    [SerializeField] private int escenaSiguiente = 1;
    private bool almacenTrigger = false;
    private int ajolotesAlmacenados = 0;


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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movimiento = GetComponent<movementpj>();
        playerCollider = GetComponent<Collider2D>();
        textoAjolotes.text = "Recolectados: 0/" + ajolotesTotales;
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
        if (ajolotesRecolectados <= 0)
        {
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

        ActualizarTextoAlmacen();

        ajolotesRecolectadosObjetos.Clear();

        textoAjolotes.text = "Recolectados: 0/" + (ajolotesTotales - ajolotesAlmacenados);


        // Activar efecto visual del cofre
        if (spriteCofre != null)
        {
            if (efectoCofre != null)
            {
                StopCoroutine(efectoCofre);
            }

            efectoCofre = StartCoroutine(EfectoCofre(Color.green));
        }

        if (ajolotesAlmacenados >= ajolotesTotales)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaSiguiente);
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

            textoAjolotes.text = "Recolectados: " + ajolotesRecolectados + "/" + (ajolotesTotales - ajolotesAlmacenados);

            other.gameObject.SetActive(false);
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
        foreach (GameObject ajolote in ajolotesRecolectadosObjetos)
        {
            ajolote.SetActive(true);
        }

        ajolotesRecolectadosObjetos.Clear();

        ajolotesRecolectados = 0;

        textoAjolotes.text = "Recolectados: 0/" + (ajolotesTotales - ajolotesAlmacenados);
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

    private void ActualizarTextoAlmacen()
    {
        if (textoAlmacen != null)
        {
            textoAlmacen.text =$"<voffset=0.1em>{NumeroConSprites(ajolotesAlmacenados)}  </voffset>  " +$"<space=0.2em>{NumeroConSprites(ajolotesTotales)}";
        }
    }

}