using UnityEngine;
using UnityEngine.UI;

public class UIPlayerAlert : MonoBehaviour
{
    public static UIPlayerAlert Instance;

    [Header("Icono del Player")]
    public Image iconoJugador;

    [Header("Colores")]
    public Color colorNormal = Color.white;
    public Color colorAlerta = new Color(1f, 0.5f, 0f); // naranja

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        iconoJugador.color = colorNormal;
    }

    public void SetAlerta(bool alerta)
    {
        iconoJugador.color = alerta ? colorAlerta : colorNormal;
    }
}
