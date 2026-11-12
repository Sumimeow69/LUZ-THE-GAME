using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Musicamanager : MonoBehaviour
{
    [SerializeField] private AudioClip musica;
    [SerializeField, Range(0f, 1f)] private float volumen = 0.4f;

    [Header("Pitch por proximidad")]
    [SerializeField] private Transform jesus;
    [SerializeField] private Transform hikari;
    [SerializeField, Min(0f)] private float distanciaCercana = 1f;
    [SerializeField, Min(0.1f)] private float distanciaLejana = 10f;
    [SerializeField, Min(0.1f)] private float velocidadCambio = 0.5f;

    private AudioSource fuente;

    private void Awake()
    {
        fuente = GetComponent<AudioSource>();

        fuente.playOnAwake = false;
        fuente.loop = true;
        fuente.spatialBlend = 0f;
        fuente.volume = volumen;
        fuente.pitch = 1f;
        fuente.clip = musica;

        if (musica != null)
        {
            fuente.Play();
        }
    }

    private void Update()
    {
        if (jesus == null || hikari == null)
        {
            return;
        }

        float distancia = Vector2.Distance(
            jesus.position, hikari.position);

        float limiteLejano = Mathf.Max(
            distanciaLejana, distanciaCercana + 0.1f);

        float proporcion = Mathf.InverseLerp(
            distanciaCercana, limiteLejano, distancia);

        float pitchObjetivo = Mathf.Lerp(0.5f, 1f, proporcion);

        fuente.pitch = Mathf.MoveTowards(
            fuente.pitch,
            pitchObjetivo,
            velocidadCambio * Time.deltaTime);
    }

    private void OnDisable()
    {
        if (fuente != null)
        {
            fuente.pitch = 1f;
        }
    }
}