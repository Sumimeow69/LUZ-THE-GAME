using UnityEngine;

public class cuadroscript : MonoBehaviour
{

    [SerializeField] private Transform puntoSalida;

    [SerializeField] private GameObject aviso;

    [SerializeField] private GameObject eventoCuadro;

    public bool IsInside = false;

    private void Awake()
    {
        
        if (aviso != null)
        {
            aviso.SetActive(false);
        }

        if (eventoCuadro != null)
        {
            eventoCuadro.SetActive(false);
        }
    }

    public void EntrarAlCuadro()
    {
        IsInside = true;
        aviso.SetActive(false);
        eventoCuadro.SetActive(true);

    }

    public void SalirDelCuadro()
    {
        IsInside = false;
        aviso.SetActive(true);
        eventoCuadro.SetActive(false);
    }
    public Transform GetPuntoSalida()
    {
        return puntoSalida;
    }

    public void ActivarEvento()
    {
        IsInside = true;

        if (aviso != null)
        {
            aviso.SetActive(false);
        }

        if (eventoCuadro != null)
        {
            eventoCuadro.SetActive(true);
        }
    }

    public void DesactivarEvento()
    {
        IsInside = false;
        if (eventoCuadro != null)
        {
            eventoCuadro.SetActive(false);
        }
    }


    public void MostrarAviso()
    {
        if (aviso != null)
        {
            aviso.SetActive(true);
        }
    }

    public void OcultarAviso()
    {
        if (aviso != null)
        {
            aviso.SetActive(false);
        }

    }

}
