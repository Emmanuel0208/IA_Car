using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para manejar escenas

public class PauseManager : MonoBehaviour
{
    // M�todo que se llama cuando se presiona el bot�n de reinicio
    public void RestartScene()
    {
        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Escena reiniciada");
    }
}
