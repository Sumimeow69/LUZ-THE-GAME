using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;


public class Playerinteract : MonoBehaviour
{




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

    private bool teletransportando = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movimiento = GetComponent<movementpj>();
        playerCollider = GetComponent<Collider2D>();
        textoAjolotes.text = "Almacenados: 0/" + ajolotesTotales;
    }

    private void Update()
    {
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
            return; //skip
        }

        ajolotesAlmacenados += ajolotesRecolectados;
        ajolotesRecolectados = 0;


        ajolotesRecolectadosObjetos.Clear();

        textoAjolotes.text = "Almacenados: 0/" + (ajolotesTotales - ajolotesAlmacenados);

        if (ajolotesAlmacenados >= ajolotesTotales)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaSiguiente);
        }
    }
    private void Interactuar()
    {
        if (!cuadroActual.IsInside)
        {
            PerderAjolotes();

            cuadroActual.EntrarAlCuadro();

            spriteRenderer.enabled = false;
            movimiento.enabled = false;
        }
        else
        {
            cuadroActual.SalirDelCuadro();

            spriteRenderer.enabled = true;
            movimiento.enabled = true;
        }
    }

    public void TeletransportarACuadro(cuadroscript destino)
    {
        if (destino == null || teletransportando)
        {
            return;
        }

        teletransportando = true;

        // Desactivar el evento del cuadro actual
        if (cuadroActual != null)
        {
            cuadroActual.DesactivarEvento();
            cuadroActual.OcultarAviso();
        }

        // Obtener punto de salida
        Transform puntoSalida = destino.GetPuntoSalida();

        if (puntoSalida == null)
        {
            teletransportando = false;
            return;
        }

        // Mover jugador
        transform.position = puntoSalida.position;

        // Nuevo cuadroActual
        cuadroActual = destino;

        // Entrar automáticamente al nuevo cuadro
        destino.IsInside = true;

        // Activar evento del destino
        destino.ActivarEvento();

        // Mantener jugador oculto y quieto
        spriteRenderer.enabled = false;
        movimiento.enabled = false;

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

            textoAjolotes.text = "Almacenados: " + ajolotesRecolectados + "/" + (ajolotesTotales - ajolotesAlmacenados);

            other.gameObject.SetActive(false);
        }
    }
    private void PerderAjolotes()
    {
        foreach (GameObject ajolote in ajolotesRecolectadosObjetos)
        {
            ajolote.SetActive(true);
        }

        ajolotesRecolectadosObjetos.Clear();

        ajolotesRecolectados = 0;

        textoAjolotes.text = "Almacenados: 0/" + (ajolotesTotales - ajolotesAlmacenados);
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

                if (cuadro == cuadroActual && !teletransportando)
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
}