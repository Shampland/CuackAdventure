using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadePanel;        // Panel negro
    public float velocidadFade = 1f;

    private void Awake()
    {
        if (fadePanel == null)
        {
            Debug.LogError("No hay panel asignado al SceneFader");
            return;
        }

        fadePanel.gameObject.SetActive(true);

        // Aseguramos que empieza totalmente opaco
        Color c = fadePanel.color;
        c.a = 1f;
        fadePanel.color = c;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color color = fadePanel.color;

        while (color.a > 0f)
        {
            color.a = Mathf.MoveTowards(color.a, 0f, Time.unscaledDeltaTime * velocidadFade);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0f;
        fadePanel.color = color;
        fadePanel.gameObject.SetActive(false);
    }

    public void CargarEscena(string nombreEscena)
    {
        StartCoroutine(FadeOut(nombreEscena));
    }

    IEnumerator FadeOut(string nombreEscena)
    {
        fadePanel.gameObject.SetActive(true);
        Color color = fadePanel.color;
        color.a = 0f;
        fadePanel.color = color;

        while (color.a < 1f)
        {
            color.a = Mathf.MoveTowards(color.a, 1f, Time.unscaledDeltaTime * velocidadFade);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;

        SceneManager.LoadScene(nombreEscena);
    }
}
