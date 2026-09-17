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
            //Entra al cuadro
            cuadroActual.EntrarAlCuadro();

            //Oculta el sprite de jesus
            spriteRenderer.enabled = false;

            //Detener movimiento
            movimiento.enabled = false;
        }
        else
        {
            //Salir cuadro
            cuadroActual.SalirDelCuadro();

            //Activar sprite de jesus
            spriteRenderer.enabled = true;

            //Activar movimiento
            movimiento.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cuadro"))
        {

            cuadroActual = other.GetComponent<cuadroscript>();

            if (cuadroActual != null)
            {
                cuadroActual.MostrarAviso();
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
            if (cuadroActual != null)
            {
                cuadroActual.OcultarAviso();
            }
            cuadroActual = null;
        }
    }

    public bool EstaDentroDelCuadro()
    {
        return cuadroActual != null && cuadroActual.IsInside;
    }
}
