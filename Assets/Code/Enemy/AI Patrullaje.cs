using UnityEngine;
using UnityEngine.AI;

public class AIPatrullaje : MonoBehaviour
{
    public Transform[] ptsPatrullaje;
    private int puntoActualIndex = 0;
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(ptsPatrullaje[puntoActualIndex].position);
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.1f )
        {
            puntoActualIndex = (puntoActualIndex + 1) % ptsPatrullaje.Length;
            agent.SetDestination(ptsPatrullaje[puntoActualIndex].position);
        }
    }
}
