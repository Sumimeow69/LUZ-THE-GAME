using UnityEngine;
using UnityEngine.InputSystem;

public class PausaScript : MonoBehaviour
{
    [SerializeField] private GameObject panelPausa;

    private bool enPausa;
    private float escalaTiempoAnterior;


    [SerializeField] private UnityEngine.UI.Image visualBotonPausa;
    private void Start()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (enPausa)
                Reanudar();
            else
                Pausar();
        }
    }
    private void MostrarBotonPausa(bool mostrar)
    {
        if (visualBotonPausa == null)
            return;

        Color color = visualBotonPausa.color;
        color.a = mostrar ? 1f : 0f;
        visualBotonPausa.color = color;

        visualBotonPausa.raycastTarget = mostrar;
    }
    public void Pausar()
    {
        // Evitar abrir la pausa si otro sistema ya congeló el juego.
        if (enPausa || panelPausa == null || Time.timeScale == 0f)
            return;

        escalaTiempoAnterior = Time.timeScale;
        enPausa = true;

        Time.timeScale = 0f;
        panelPausa.SetActive(true);
        MostrarBotonPausa(false);
    }

    public void Reanudar()
    {
        if (!enPausa)
            return;

        Time.timeScale = escalaTiempoAnterior;
        enPausa = false;

        MostrarBotonPausa(true);

        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    private void OnDisable()
    {
        // Restaurar el tiempo si se desactiva el gestor
        // o se cambia de escena durante la pausa.
        Reanudar();
    }
}
