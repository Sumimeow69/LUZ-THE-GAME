using UnityEngine;
using UnityEngine.SceneManagement;

public class MovEnemigo : MonoBehaviour
{
    [SerializeField] private Transform[] puntos;
    [SerializeField] private float speed = 2f;

    private int puntoActual = 0;

    private void Update()
    {
        Transform destino = puntos[puntoActual];

        transform.position = Vector2.MoveTowards(transform.position,destino.position,speed * Time.deltaTime); //seguramente para hacer un mov variado tenga algo que ver con estas funciones similares como el Lerp o PingPong !!!

        if (Vector2.Distance(transform.position, destino.position) < 0.01f)
        {
            puntoActual++;

            if (puntoActual >= puntos.Length)
            {
                puntoActual = 0;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Playerinteract player = other.gameObject.GetComponent<Playerinteract>();

            if (player != null && player.EstaDentroDelCuadro())
            {
                return;
            }

            SceneManager.LoadScene(2);
        }
    }
}
