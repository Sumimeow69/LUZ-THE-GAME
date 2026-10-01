using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public float velocidadPatrulla = 10f;
    public float velocidadPersecucion = 10f;

    private NavMeshAgent agent;
    private EstadoIA estadoActual = EstadoIA.Patrullando;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = velocidadPatrulla;
        IrAlSiguientePunto();
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
    }

    void DeteccionJugador()
    {
        Collider2D jugadorDetectado = Physics2D.OverlapCircle(transform.position, radioDeteccion, capaJugador);

        if (jugadorDetectado != null)
        {
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
                estadoActual = EstadoIA.Patrullando;
                agent.speed = velocidadPatrulla;
                timerPerdidaVista = 0f;
                IrAlSiguientePunto();
            }
        }
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