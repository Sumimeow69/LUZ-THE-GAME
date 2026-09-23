using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Playerinteract : MonoBehaviour
{
    [SerializeField] private TMP_Text textoAjolotes;
    [SerializeField] private int ajolotesTotales = 4;

    private int ajolotesRecolectados = 0;

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

        textoAjolotes.text = "Ajolotes: 0/" + ajolotesTotales;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (cuadroActual != null)
            {
                Interactuar();
            }
        }
    }

    private void Interactuar()
    {
        if (!cuadroActual.IsInside)
        {
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

        if (other.CompareTag("Ajolote"))
        {
            ajolotesRecolectados++;

            textoAjolotes.text = "Ajolotes: " + ajolotesRecolectados + "/" + ajolotesTotales;

            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
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