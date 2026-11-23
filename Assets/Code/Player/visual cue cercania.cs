using UnityEngine;

public class visualcuecercania : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform jugador;
    [SerializeField] private Transform hikari;
    [SerializeField] private SpriteRenderer punta;

    [Header("Distancias seteabl")]
    [SerializeField, Min(0.1f)] private float distanciaAviso = 13f;
    [SerializeField, Min(0f)] private float distanciaPeligro = 2f;
    [SerializeField, Min(0f)] private float radioOrbita = 1.2f;

    [Header("Grosor: largo (X) y ancho (Y)")]
    [SerializeField] private Vector2 tamañoLejano = new Vector2(0.25f, 0.3f);
    [SerializeField] private Vector2 tamañoCercano = new Vector2(0.65f, 0.18f);

    [Header("Color")]
    [SerializeField]
    private Color colorLejano = new Color(1f, 0.8f, 0.2f, 0.25f);

    [SerializeField]
    private Color colorCercano =new Color(1f, 0.1f, 0.05f, 0.9f);

    [Header("smooth")]
    [SerializeField, Min(0.1f)] private float suavidad = 6f;
    [SerializeField] private bool indicadorActivo = true;

    private float intensidad;

    private void LateUpdate()
    {
        if (punta == null)
            return;

        if (!indicadorActivo || jugador == null || hikari == null)
        {
            intensidad = 0f;
            punta.enabled = false;
            return;
        }

        Vector2 direccion = hikari.position - jugador.position;
        float distancia = direccion.magnitude;

        float limiteAviso = Mathf.Max(
            distanciaAviso, distanciaPeligro + 0.1f);

        float objetivo = 1f - Mathf.InverseLerp(
            distanciaPeligro, limiteAviso, distancia);

        intensidad = Mathf.MoveTowards(
            intensidad, objetivo, suavidad * Time.deltaTime);

        punta.enabled = intensidad > 0.001f;

        transform.position = jugador.position;

        if (direccion.sqrMagnitude > 0.0001f)
        {
            float angulo = Mathf.Atan2(
                direccion.y, direccion.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angulo);
        }

        punta.transform.localPosition =
            new Vector3(radioOrbita, 0f, 0f);

        Vector2 tamaño = Vector2.Lerp(
            tamañoLejano, tamañoCercano, intensidad);

        punta.transform.localScale =
            new Vector3(tamaño.x, tamaño.y, 1f);

        Color color = Color.Lerp(
            colorLejano, colorCercano, intensidad);

        // Aparece y desaparece progresivamente en el límite del aviso.
        color.a *= Mathf.SmoothStep(0f, 1f, intensidad);

        punta.color = color;
    }

    public void SetIndicadorActivo(bool activo)
    {
        indicadorActivo = activo;
    }
}
