using UnityEngine;

public class scenemanagerboton : MonoBehaviour
{
    

    public void RESTART()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
