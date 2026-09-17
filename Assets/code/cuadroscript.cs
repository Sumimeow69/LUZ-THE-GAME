using UnityEngine;

public class cuadroscript : MonoBehaviour
{

    [SerializeField] private GameObject cuadroPurificado;

    [SerializeField] private GameObject aviso;

    public bool IsInside = false;

    private void Awake()
    {

        cuadroPurificado.SetActive(false);

        
        if (aviso != null)
        {
            aviso.SetActive(false);
        }
    }

    public void EntrarAlCuadro()
    {
        IsInside = true;
        cuadroPurificado.SetActive(true);
        aviso.SetActive(false);
    }

    public void SalirDelCuadro()
    {
        IsInside = false;
        cuadroPurificado.SetActive(false);
        aviso.SetActive(true);
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
