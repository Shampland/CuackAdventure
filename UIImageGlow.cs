using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIImageGlow : MonoBehaviour
{
    [Header("Configuración de Brillo")]
    public Color colorBrillo = new Color(1f, 0.85f, 0.3f); // dorado cálido
    public float velocidadBrillo = 2f;
    public float intensidadMin = 0.5f;
    public float intensidadMax = 2f;

    private Image imagen;
    private Color colorBase;
    private float tiempo;

    void Start()
    {
        imagen = GetComponent<Image>();
        colorBase = imagen.color;
    }

    void Update()
    {
        tiempo += Time.deltaTime * velocidadBrillo;
        float intensidad = Mathf.Lerp(intensidadMin, intensidadMax, (Mathf.Sin(tiempo) + 1f) / 2f);

        Color colorFinal = Color.Lerp(colorBase, colorBrillo, 0.5f) * intensidad;
        colorFinal.a = colorBase.a;
        imagen.color = colorFinal;
    }
}
