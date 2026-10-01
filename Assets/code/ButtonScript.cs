using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonScript : MonoBehaviour
{
    [Header("Config del botón")]

    [SerializeField] private cuadroscript cuadroDestino;

    [SerializeField, Range(0, 3)] private int indiceTecla; // [0] = W ; [1] = A ; [2] = S ; [3] = D !!!!!!!!!!!!!

    private bool ejecutado = false;

    private void Update()
    {
        if (ejecutado || Keyboard.current == null)
        {
            return;
        }

        bool TeclaPresionada = false;
        

        switch (indiceTecla)
        {
            case 0:
                TeclaPresionada = Keyboard.current.wKey.wasPressedThisFrame;
                break;

            case 1:
                TeclaPresionada = Keyboard.current.aKey.wasPressedThisFrame;
                break;

            case 2:
                TeclaPresionada = Keyboard.current.sKey.wasPressedThisFrame;
                break;

            case 3:
                TeclaPresionada = Keyboard.current.dKey.wasPressedThisFrame;
                break;
        }

        if (TeclaPresionada)
        {
            Teletransportar();
        }
    }
    private void OnEnable()
    {
        ejecutado = false;
    }
    public void Teletransportar()
    {
        Playerinteract player = FindFirstObjectByType<Playerinteract>();

        if (player != null && cuadroDestino != null)
        {
            player.TeletransportarACuadro(cuadroDestino);
        }
    }


}
