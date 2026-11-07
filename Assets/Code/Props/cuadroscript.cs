using UnityEngine;
using System.Collections;

public class cuadroscript : MonoBehaviour
{
    [SerializeField] private Transform puntoSalida;
    [SerializeField] private GameObject aviso;

    [Header("Contenido que aparece dentro del cuadro")]
    [SerializeField] private GameObject eventoCuadro;
    [SerializeField] private SpriteRenderer spriteIntegrado;
    [SerializeField] private GameObject grupoBotones;

    [Header("Duraciones::")]
    [Header("Entrada desde fuera")]
    [SerializeField, Min(0f)] private float fadeSprite = 0.15f;
    [SerializeField, Min(0f)] private float fadeBotones = 0.1f;

    [Header("Llegada por TP")]
    [SerializeField, Min(0f)] private float esperaCamaraTP = 0.3f;
    [SerializeField, Min(0f)] private float fadeSpriteTP = 0.4f;
    [SerializeField, Min(0f)] private float fadeBotonesTP = 0.2f;

    [Header("Default Botones")]
    [SerializeField, Min(0f)] private float retrasoBotones = 0.1f;

    public bool IsInside = false;
    public bool EnTransicion { get; private set; }
    public bool ListoParaTP { get; private set; }

    private SpriteRenderer[] spritesBotones;
    private Animator[] animadoresBotones;
    private ButtonScript[] botones;

    private void Awake()
    {
        spritesBotones = grupoBotones.GetComponentsInChildren<SpriteRenderer>(true);
        animadoresBotones = grupoBotones.GetComponentsInChildren<Animator>(true);
        botones = grupoBotones.GetComponentsInChildren<ButtonScript>(true);

        PrepararOculto();
        OcultarAviso();
    }

    private void PrepararOculto()
    {
        ListoParaTP = false;

        foreach (ButtonScript boton in botones)
            boton.SetDisponible(false);

        foreach (Animator animador in animadoresBotones)
            animador.enabled = false;

        if (spriteIntegrado != null)
            PonerAlpha(spriteIntegrado, 0f);

        foreach (SpriteRenderer sprite in spritesBotones)
            PonerAlpha(sprite, 0f);

        grupoBotones.SetActive(false);
        eventoCuadro.SetActive(false);
    }

    public IEnumerator EntrarConTransicion(bool esTP = false)
    {
        EnTransicion = true;
        PrepararOculto();

        IsInside = true;
        OcultarAviso();

        eventoCuadro.SetActive(true);

        float duracionSprite = esTP ? fadeSpriteTP : fadeSprite;

        if (esTP && esperaCamaraTP > 0f)
        {
            yield return new WaitForSeconds(esperaCamaraTP);
        }

        // Fade del sprite integrado en el cuadro.
        if (spriteIntegrado != null)
        {
            yield return Fade(
                new SpriteRenderer[] { spriteIntegrado },
                duracionSprite
            );
        }

        // Retraso configurable antes de mostrar las flechas.
        yield return new WaitForSeconds(retrasoBotones);

        // Botones visibles directamente, sin fade.
        foreach (SpriteRenderer sprite in spritesBotones)
            PonerAlpha(sprite, 1f);

        grupoBotones.SetActive(true);

        // Activar el parpadeo.
        foreach (Animator animador in animadoresBotones)
        {
            animador.enabled = true;
            animador.Rebind();
            animador.Update(0f);
        }

        // Habilitar la interacción.
        EnTransicion = false;
        ListoParaTP = true;

        foreach (ButtonScript boton in botones)
            boton.SetDisponible(true);
    }

    private IEnumerator Fade(SpriteRenderer[] sprites, float duracion)
    {
        foreach (SpriteRenderer sprite in sprites)
            PonerAlpha(sprite, 0f);

        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float alpha = Mathf.Clamp01(tiempo / duracion);

            foreach (SpriteRenderer sprite in sprites)
                PonerAlpha(sprite, alpha);

            yield return null;
        }

        foreach (SpriteRenderer sprite in sprites)
            PonerAlpha(sprite, 1f);
    }

    private void PonerAlpha(SpriteRenderer sprite, float alpha)
    {
        Color color = sprite.color;
        color.a = alpha;
        sprite.color = color;
    }

    public void SalirDelCuadro()
    {
        DesactivarEvento();
        MostrarAviso();
    }

    public void ActivarEvento()
    {
        StopAllCoroutines();
        StartCoroutine(EntrarConTransicion());
    }

    public void DesactivarEvento()
    {
        StopAllCoroutines();
        IsInside = false;
        EnTransicion = false;
        PrepararOculto();
    }

    public Transform GetPuntoSalida()
    {
        return puntoSalida;
    }

    public void MostrarAviso()
    {
        if (aviso != null)
            aviso.SetActive(true);
    }

    public void OcultarAviso()
    {
        if (aviso != null)
            aviso.SetActive(false);
    }
}