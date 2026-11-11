using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class TutorialSprites : MonoBehaviour
{
    [SerializeField] private GameObject[] paginas;
    [SerializeField] private UnityEngine.UI.Button botonAnterior;
    [SerializeField] private UnityEngine.UI.Button botonSiguiente;

    private int paginaActual;
    private EventSystem sistemaEventos;
    private InputAction accionMover;
    private GameObject seleccionAnterior;
    private GameObject primeraSeleccionAnterior;
    private bool derechaPresionada;

    private void OnEnable()
    {
        sistemaEventos = EventSystem.current;
        if (sistemaEventos == null)
        {
            sistemaEventos = FindFirstObjectByType<EventSystem>();
        }

        if (sistemaEventos != null)
        {
            seleccionAnterior = sistemaEventos.currentSelectedGameObject;
            primeraSeleccionAnterior = sistemaEventos.firstSelectedGameObject;
            sistemaEventos.firstSelectedGameObject = null;

            var moduloUI = sistemaEventos.GetComponent<InputSystemUIInputModule>();
            accionMover = moduloUI != null ? moduloUI.move?.action : null;
        }

        // Las flechas solo navegan entre ellas, aunque el menu siga activo.
        if (botonAnterior != null)
        {
            botonAnterior.navigation = new UnityEngine.UI.Navigation
            {
                mode = UnityEngine.UI.Navigation.Mode.Explicit,
                selectOnRight = botonSiguiente
            };
        }

        if (botonSiguiente != null)
        {
            botonSiguiente.navigation = new UnityEngine.UI.Navigation
            {
                mode = UnityEngine.UI.Navigation.Mode.Explicit,
                selectOnLeft = botonAnterior
            };
        }

        paginaActual = 0;
        ActualizarPaginas();
    }

    private void Update()
    {
        bool haciaDerecha = EstaPresionadaDerecha();
        bool nuevaPulsacion = haciaDerecha && !derechaPresionada;
        derechaPresionada = haciaDerecha;

        if (sistemaEventos == null || botonSiguiente == null)
        {
            return;
        }

        var seleccionado = sistemaEventos.currentSelectedGameObject;
        bool sinSeleccion = seleccionado == null || !seleccionado.activeInHierarchy;

        if (sinSeleccion && nuevaPulsacion && botonSiguiente.IsActive()
            && botonSiguiente.IsInteractable())
        {
            botonSiguiente.Select();
        }
    }

    private bool EstaPresionadaDerecha()
    {
        Vector2 movimiento = accionMover != null
            ? accionMover.ReadValue<Vector2>()
            : Vector2.zero;
        return movimiento.x > 0.5f && movimiento.x > Mathf.Abs(movimiento.y);
    }

    public void Siguiente()
    {
        if (paginas == null || paginas.Length == 0)
        {
            return;
        }

        if (paginaActual == paginas.Length - 1)
        {
            gameObject.SetActive(false);
            return;
        }

        paginaActual++;
        ActualizarPaginas();
    }

    public void Anterior()
    {
        if (paginaActual > 0)
        {
            paginaActual--;
            ActualizarPaginas();
        }
    }

    private void ActualizarPaginas()
    {
        int cantidadPaginas = paginas != null ? paginas.Length : 0;
        for (int i = 0; i < cantidadPaginas; i++)
        {
            if (paginas[i] != null)
            {
                paginas[i].SetActive(i == paginaActual);
            }
        }

        if (botonAnterior != null)
        {
            botonAnterior.interactable = true;
            botonAnterior.gameObject.SetActive(paginaActual > 0);
        }

        if (botonSiguiente != null)
        {
            botonSiguiente.interactable = true;
            botonSiguiente.gameObject.SetActive(cantidadPaginas > 0);
        }

        // Volver a la primera pagina deja de nuevo ambas flechas sin seleccion.
        if (paginaActual == 0 && sistemaEventos != null)
        {
            sistemaEventos.SetSelectedGameObject(null);
            // Mantener derecha no cuenta como una nueva pulsacion.
            derechaPresionada = EstaPresionadaDerecha();
        }
    }

    private void OnDisable()
    {
        if (sistemaEventos == null)
        {
            return;
        }

        sistemaEventos.firstSelectedGameObject = primeraSeleccionAnterior;
        var destino = seleccionAnterior != null && seleccionAnterior.activeInHierarchy
            ? seleccionAnterior
            : primeraSeleccionAnterior;
        sistemaEventos.SetSelectedGameObject(
            destino != null && destino.activeInHierarchy ? destino : null);
        accionMover = null;
    }
}
