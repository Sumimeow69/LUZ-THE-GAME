using System.Collections.Generic;
using UnityEngine;

public class FilaAjolotes : MonoBehaviour
{
    [SerializeField] private GameObject prefabVisual;
    [SerializeField, Min(0.1f)] private float separacion = 0.85f;

    private readonly List<AjoloteEnFila> seguidores = new();
    private readonly List<Vector3> recorrido = new();

    private float longitudRecorrido;
    private Playerinteract jugador;

    private void Awake()
    {
        jugador = GetComponent<Playerinteract>();
        recorrido.Add(transform.position);
    }

    private void LateUpdate()
    {
        Vector3 posicion = transform.position;
        float avance = Vector3.Distance(posicion, recorrido[0]);

        if (avance > 0.001f)
        {
            recorrido.Insert(0, posicion);
            longitudRecorrido += avance;
        }

        // Guardar solamente el camino necesario para la fila.
        float distanciaNecesaria = (seguidores.Count + 2) * separacion;

        while (recorrido.Count > 2)
        {
            int ultimo = recorrido.Count - 1;
            float tramo = Vector3.Distance(
                recorrido[ultimo], recorrido[ultimo - 1]);

            if (longitudRecorrido - tramo < distanciaNecesaria)
                break;

            longitudRecorrido -= tramo;
            recorrido.RemoveAt(ultimo);
        }

        for (int i = 0; i < seguidores.Count; i++)
        {
            seguidores[i].transform.position =
                PosicionEnRecorrido((i + 1) * separacion);
        }
    }

    private Vector3 PosicionEnRecorrido(float distancia)
    {
        for (int i = 1; i < recorrido.Count; i++)
        {
            float tramo = Vector3.Distance(
                recorrido[i - 1], recorrido[i]);

            if (tramo >= distancia)
            {
                return Vector3.Lerp(
                    recorrido[i - 1],
                    recorrido[i],
                    distancia / tramo);
            }

            distancia -= tramo;
        }

        return recorrido[recorrido.Count - 1];
    }

    public void Agregar(GameObject origen)
    {
        GameObject compañero = Instantiate(
            prefabVisual,
            PosicionEnRecorrido((seguidores.Count + 1) * separacion),
            prefabVisual.transform.rotation);

        AjoloteEnFila ajolote = compañero.GetComponent<AjoloteEnFila>();
        ajolote.Inicializar(jugador, origen);
        seguidores.Add(ajolote);
    }

    public void Quitar(GameObject origen)
    {
        for (int i = 0; i < seguidores.Count; i++)
        {
            AjoloteEnFila compañero = seguidores[i];

            if (compañero.Origen != origen)
                continue;

            seguidores.RemoveAt(i);
            compañero.gameObject.SetActive(false);
            Destroy(compañero.gameObject);
            return;
        }
    }

    public void Limpiar()
    {
        foreach (AjoloteEnFila compañero in seguidores)
        {
            if (compañero == null)
                continue;

            compañero.gameObject.SetActive(false);
            Destroy(compañero.gameObject);
        }

        seguidores.Clear();
        recorrido.Clear();
        recorrido.Add(transform.position);
        longitudRecorrido = 0f;
    }

    private void OnDestroy()
    {
        Limpiar();
    }
}