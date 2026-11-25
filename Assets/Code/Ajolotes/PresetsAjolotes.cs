using System;
using System.Collections.Generic;
using UnityEngine;

public class PresetsAjolotes : MonoBehaviour
{
    [Serializable]
    public class Preset
    {
        public string nombre;

        [Tooltip("Ajolotes de la escena que estarán disponibles en este preset.")]
        public GameObject[] ajolotes;
    }

    [Header("Referencias")]
    [SerializeField] private Playerinteract jugador;

    [Header("Objetivo de este nivel")]
    [SerializeField, Min(1)] private int necesariosParaGanar = 4;

    [Header("Se elige un preset al azar al comenzar")]
    [SerializeField] private Preset[] presets;

    private void Start()
    {
        if (jugador == null || presets == null || presets.Length == 0)
        {
            Debug.LogError(
                "Asigna el jugador y al menos un preset de ajolotes.",
                this
            );
            return;
        }

        // Validar todos: cualquiera puede salir elegido.
        foreach (Preset preset in presets)
        {
            if (preset == null || preset.ajolotes == null)
            {
                Debug.LogError("Hay un preset sin configurar.", this);
                return;
            }

            var unicos = new HashSet<GameObject>();

            foreach (GameObject ajolote in preset.ajolotes)
            {
                if (ajolote == null || !unicos.Add(ajolote))
                {
                    Debug.LogError(
                        $"El preset '{preset.nombre}' tiene un campo vacío " +
                        "o el mismo ajolote repetido.",
                        this
                    );
                    return;
                }
            }

            if (unicos.Count < necesariosParaGanar)
            {
                Debug.LogError(
                    $"El preset '{preset.nombre}' tiene {unicos.Count} " +
                    $"ajolotes, pero este nivel requiere {necesariosParaGanar}.",
                    this
                );
                return;
            }
        }

        int indice = UnityEngine.Random.Range(0, presets.Length);
        Preset elegido = presets[indice];

        jugador.ConfigurarObjetivoAjolotes(necesariosParaGanar);

        // Primero apagar todos los ajolotes de todos los presets.
        foreach (Preset preset in presets)
        {
            foreach (GameObject ajolote in preset.ajolotes)
            {
                ajolote.SetActive(false);
            }
        }

        // Después encender solamente los del preset elegido.
        foreach (GameObject ajolote in elegido.ajolotes)
        {
            ajolote.SetActive(true);
        }
    }
}
