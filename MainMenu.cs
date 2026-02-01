using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Cambia "Game" por el nombre real de tu escena de juego
        SceneManager.LoadScene("Game");
    }

    public void Credits()
    {
        // Cambia "Credits" por el nombre real de tu escena de créditos
        SceneManager.LoadScene("Credits");
    }

    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void Menu()
    {
        // Cambia "Menu" por el nombre real de tu escena de menú principal
        SceneManager.LoadScene("MenuPrincipal");
    }
}
