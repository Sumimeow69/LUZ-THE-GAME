using UnityEngine;
using UnityEngine.SceneManagement;

public class scenemanagerboton : MonoBehaviour
{
    [SerializeField] private int indice;

    public static int nivelParaReplay = -1;

    public void GoToScene()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene(indice);
    }

    public void REPLAY()
    {
        if (nivelParaReplay >= 0)
        {
            SceneManager.LoadScene(nivelParaReplay);
        }
    }
}
