using UnityEngine;

[System.Serializable]
public class Dialogo
{
    [TextArea(2, 5)]
    public string[] frases;          // Frases del diálogo
    public float velocidadEscritura = 0.05f; // Tiempo entre letras
}
