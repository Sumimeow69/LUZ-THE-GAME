using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
public enum EstadoIA { Patrullando, Persiguiendo }

[System.Serializable]
public struct ZonaPatrulla
{
    public string nombreZona;
    public Transform[] puntosDeZona;
    public float tiempoEnZona;
}

public class IAZone : MonoBehaviour
{
    [Header("Configuración de Zonas")]
    public List<ZonaPatrulla> zonas;
    private int zonaActualIndex = 0;
    private int puntoActualIndex = 0;
    private float timerZona = 0f;

    [Header("Detección y Persecución")]
    public float radioDeteccion = 4f;
    public LayerMask capaJugador;
    public Transform transformJugador;
    public float tiempoParaPerderVista = 3f;
    private float timerPerdidaVista = 0f;

    [Header("Velocidades")]
    public float velocidadPatrulla = 5f;
    public float velocidadPersecucion = 6f;

    private NavMeshAgent agent;
    private EstadoIA estadoActual = EstadoIA.Patrullando;

    [Header("Iluminacion de Persecucion")]
    [SerializeField] private Light2D luzPersecucion;
    [SerializeField, Range(0f, 1f)] private float opacidadMaxima = 1f;
    [SerializeField, Min(0.01f)] private float velocidadOpacidad = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocidadPatrulla;
        IrAlSiguientePunto();
    }
    private void ActualizarLuzPersecucion()
    {
        if (luzPersecucion == null) return;

        float objetivo = estadoActual == EstadoIA.Persiguiendo
            ? opacidadMaxima
            : 0f;

        Color color = luzPersecucion.color;

        color.a = Mathf.MoveTowards(
            color.a,
            objetivo,
            velocidadOpacidad * Time.deltaTime
        );

        luzPersecucion.color = color;
    }
    void Update()
    {
        DeteccionJugador();

        switch (estadoActual)
        {
            case EstadoIA.Patrullando:
                ActualizarPatrullaje();
                break;

            case EstadoIA.Persiguiendo:
                ActualizarPersecucion();
                break;
        }
        ActualizarLuzPersecucion();
    }

    void DeteccionJugador()
    {
        if (JugadorEstaOculto(transformJugador))
        {
            if (estadoActual == EstadoIA.Persiguiendo)
            {
                OlvidarJugador();
            }
            return;
        }

        Collider2D jugadorDetectado = Physics2D.OverlapCircle(transform.position, radioDeteccion, capaJugador);

        if (jugadorDetectado != null)
        {
            if (JugadorEstaOculto(jugadorDetectado.transform)) return;

            if (transformJugador == null) transformJugador = jugadorDetectado.transform;

            if (estadoActual == EstadoIA.Patrullando)
            {
                estadoActual = EstadoIA.Persiguiendo;
                agent.speed = velocidadPersecucion;
            }

            timerPerdidaVista = 0f;
        }
        else if (estadoActual == EstadoIA.Persiguiendo)
        {
            timerPerdidaVista += Time.deltaTime;

            if (timerPerdidaVista >= tiempoParaPerderVista)
            {
                OlvidarJugador();
            }
        }
    }

    private bool JugadorEstaOculto(Transform objetivo)
    {
        if (objetivo == null) return false;

        Playerinteract jugador = objetivo.GetComponentInParent<Playerinteract>();
        return jugador != null && jugador.EstaDentroDelCuadro();
    }

    private void OlvidarJugador()
    {
        transformJugador = null;
        estadoActual = EstadoIA.Patrullando;
        timerPerdidaVista = 0f;
        timerZona = 0f;
        agent.speed = velocidadPatrulla;
        agent.ResetPath();

        if (zonas.Count == 0) return;

        // Choose another zone instead of returning to the hiding place.
        if (zonas.Count > 1)
        {
            int siguienteZona = Random.Range(0, zonas.Count - 1);
            if (siguienteZona >= zonaActualIndex) siguienteZona++;
            zonaActualIndex = siguienteZona;
        }

        Transform[] puntos = zonas[zonaActualIndex].puntosDeZona;
        puntoActualIndex = puntos.Length > 0 ? Random.Range(0, puntos.Length) : 0;
        IrAlSiguientePunto();
    }

    void ActualizarPatrullaje()
    {
        if (zonas.Count == 0) return;

        ZonaPatrulla zonaActual = zonas[zonaActualIndex];
        timerZona += Time.deltaTime;

        if (timerZona >= zonaActual.tiempoEnZona)
        {
            timerZona = 0f;
            zonaActualIndex = (zonaActualIndex + 1) % zonas.Count;
            puntoActualIndex = 0;
            IrAlSiguientePunto();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            puntoActualIndex = (puntoActualIndex + 1) % zonaActual.puntosDeZona.Length;
            IrAlSiguientePunto();
        }
    }

    void IrAlSiguientePunto()
    {
        if (zonas.Count == 0) return;

        ZonaPatrulla zonaActual = zonas[zonaActualIndex];
        if (zonaActual.puntosDeZona.Length > 0)
        {
            agent.SetDestination(zonaActual.puntosDeZona[puntoActualIndex].position);
        }
    }

    void ActualizarPersecucion()
    {
        if (transformJugador != null)
        {
            agent.SetDestination(transformJugador.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}
