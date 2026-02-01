using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI - Icono Jugador")]
    public Image iconoJugador;
    public Color colorNormal = Color.white;
    public Color colorAlerta = new Color(1f, 0.5f, 0f);

    [Header("Game Over")]
    public GameObject canvasGameOver;

    [Header("Diálogos")]
    public DialogueManager dialogueManager; // Referencia al DialogueManager
    public Dialogo dialogoInicio;           // Diálogo inicial
    public Dialogo[] dialogosObjetos;       // Diálogos al recoger objetos (4)

    private bool juegoTerminado;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (canvasGameOver != null) canvasGameOver.SetActive(false);
        SetEnemyAlert(false);
        juegoTerminado = false;
        Time.timeScale = 1f;

        // Inicia diálogo inicial con imagen final
        if (dialogueManager != null && dialogoInicio != null)
        {
            dialogueManager.IniciarDialogo(dialogoInicio, true); // ← true = diálogo inicial
        }
    }



    // ================= ALERTA ENEMIGO =================
    public void SetEnemyAlert(bool alerta)
    {
        if (iconoJugador == null) return;
        iconoJugador.color = alerta ? colorAlerta : colorNormal;
    }

    // ================= GAME OVER =================
    public void GameOver()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        if (canvasGameOver != null) canvasGameOver.SetActive(true);
        Time.timeScale = 0f;
    }

    // ================= BOTONES UI =================
    public void ResetGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    // ================= DIÁLOGOS POR OBJETO =================
    public void ActivarDialogoPorObjeto(int indice)
    {
        if (dialogueManager != null && dialogosObjetos != null && indice >= 0 && indice < dialogosObjetos.Length)
        {
            dialogueManager.IniciarDialogo(dialogosObjetos[indice]);
        }
    }
}
