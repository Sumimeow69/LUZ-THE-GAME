using UnityEngine;
using UnityEngine.AI;

public class SpawnHikari : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject hikari;
    [SerializeField] private Transform[] puntosDeSpawn;
    private void Awake()
    {
        int randomSpawn = Random.Range(0, puntosDeSpawn.Length);
        Transform puntoElegido = puntosDeSpawn[randomSpawn];

        NavMeshAgent agent = hikari.GetComponent<NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh)
        {
            agent.Warp(puntoElegido.position);
        }
        else
        {
            hikari.transform.position = puntoElegido.position;
        }

        hikari.SetActive(true);
    }
}
