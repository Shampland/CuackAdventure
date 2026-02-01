using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public GameObject indicadorE;
    public GameObject imagenFinal; // Solo para diálogo inicial

    private Dialogo dialogoActual;
    private int indiceFrase = 0;
    private bool escribiendo = false;
    private bool esperaE = false;
    private bool mostrandoImagenFinal = false;
    private bool esDialogoInicial = false; // ← nuevo
    private System.Action callbackImagenFinal;

    private void Awake()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (indicadorE != null) indicadorE.SetActive(false);
        if (imagenFinal != null) imagenFinal.SetActive(false);
    }

    // ------------------ Inicia diálogo ------------------
    public void IniciarDialogo(Dialogo dialogo, bool inicial = false)
    {
        if (dialogo == null || dialogo.frases.Length == 0) return;

        dialogoActual = dialogo;
        indiceFrase = 0;
        esDialogoInicial = inicial; // Guardamos si es diálogo inicial

        panelDialogo.SetActive(true);
        PausarJuego(true);

        StartCoroutine(EscribirFrase(dialogoActual.frases[indiceFrase], dialogoActual.velocidadEscritura));
    }

    private void Update()
    {
        if (!esperaE) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            esperaE = false;
            if (indicadorE != null) indicadorE.SetActive(false);

            // Solo el diálogo inicial puede mostrar la imagen final
            if (mostrandoImagenFinal)
            {
                CerrarImagenFinal();
                return;
            }

            indiceFrase++;
            if (indiceFrase < dialogoActual.frases.Length)
            {
                StartCoroutine(EscribirFrase(dialogoActual.frases[indiceFrase], dialogoActual.velocidadEscritura));
            }
            else
            {
                // Fin del diálogo
                if (esDialogoInicial && imagenFinal != null)
                {
                    imagenFinal.SetActive(true);
                    mostrandoImagenFinal = true;

                    // Mantener panel activo para mostrar la imagen
                    panelDialogo.SetActive(true);
                    textoDialogo.text = "";
                    if (indicadorE != null) indicadorE.SetActive(true);
                    esperaE = true;
                }
                else
                {
                    panelDialogo.SetActive(false);
                    PausarJuego(false);
                }
            }
        }
    }

    IEnumerator EscribirFrase(string frase, float velocidad)
    {
        escribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in frase)
        {
            textoDialogo.text += letra;
            yield return new WaitForSecondsRealtime(velocidad);
        }

        escribiendo = false;
        esperaE = true;
        if (indicadorE != null) indicadorE.SetActive(true);
    }

    private void PausarJuego(bool pausar)
    {
        Time.timeScale = pausar ? 0f : 1f;
    }

    private void CerrarImagenFinal()
    {
        if (imagenFinal != null) imagenFinal.SetActive(false);
        mostrandoImagenFinal = false;

        panelDialogo.SetActive(false);
        PausarJuego(false);

        callbackImagenFinal?.Invoke();
    }

    public void SetCallbackImagenFinal(System.Action callback)
    {
        callbackImagenFinal = callback;
    }
}
