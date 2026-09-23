using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private cuadroscript cuadroDestino;

    public void Teletransportar()
    {
        Playerinteract player = FindFirstObjectByType<Playerinteract>();

        if (player != null && cuadroDestino != null)
        {
            player.TeletransportarACuadro(cuadroDestino);
        }
    }
}
