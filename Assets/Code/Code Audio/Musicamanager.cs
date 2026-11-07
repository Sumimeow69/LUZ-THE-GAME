using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Musicamanager : MonoBehaviour
{
        [SerializeField] private AudioClip musica;
        [SerializeField, Range(0f, 1f)] private float volumen = 0.4f;

        private void Awake()
        {
            AudioSource fuente = GetComponent<AudioSource>();

            fuente.playOnAwake = false;
            fuente.loop = true;
            fuente.spatialBlend = 0f;
            fuente.volume = volumen;
            fuente.clip = musica;

            if (musica != null)
                fuente.Play();
        }
    }

