using UnityEngine;

public class sonido3d : MonoBehaviour
{
    [SerializeField] private AudioSource fuente;
    [SerializeField] private AudioClip sonido;
    [SerializeField, Min(0.1f)] private float intervalo = 5f;

    private float tiempo;

    private void OnEnable()
    {
        tiempo = 0f;
    }

    private void Update()
    {
        tiempo += Time.deltaTime;

        if (tiempo < intervalo)
            return;

        tiempo = 0f;

        if (fuente != null && sonido != null && !fuente.isPlaying)
        {
            fuente.PlayOneShot(sonido);
        }
    }
}
