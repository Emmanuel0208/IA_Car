using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para manejar escenas

public class PauseManager : MonoBehaviour
{
    // Método que se llama cuando se presiona el botón de reinicio
    public void RestartScene()
    {
        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Escena reiniciada");
    }
}
