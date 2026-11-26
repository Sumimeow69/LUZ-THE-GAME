using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class AjoloteEnFila : MonoBehaviour
{
    public GameObject Origen { get; private set; }

    private Playerinteract jugador;

    public void Inicializar(Playerinteract propietario, GameObject origen)
    {
        jugador = propietario;
        Origen = origen;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.timeScale == 0f || jugador == null)
            return;

        // Reconocer a Hikari por sus componentes, sin depender de su nombre o tag.
        if (other.GetComponentInParent<IAZone>() == null &&
            other.GetComponentInParent<MovEnemigo>() == null)
        {
            return;
        }

        jugador.PerderAjolote(Origen);
    }
}
