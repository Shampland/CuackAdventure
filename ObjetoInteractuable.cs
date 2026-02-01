using UnityEngine;

public class ObjetoInteractuable : MonoBehaviour
{
    [Header("UI")]
    public Sprite iconoUI;

    [Header("Sonido")]
    public AudioClip sonidoRecoger;

    [Header("Diálogo")]
    [Tooltip("Índice del diálogo que se mostrará al recoger este objeto")]
    public int indiceDialogo = -1; // -1 = no hay diálogo

    public virtual void Interactuar()
    {
        Debug.Log("Objeto recogido: " + gameObject.name);

        // ------------------- UI -------------------
        UIInventory inventario = FindObjectOfType<UIInventory>();
        if (inventario != null)
        {
            inventario.AgregarObjeto(iconoUI);
        }

        // ------------------- Sonido -------------------
        if (sonidoRecoger != null)
        {
            AudioSource.PlayClipAtPoint(
                sonidoRecoger,
                Camera.main.transform.position
            );
        }

        // ------------------- Desaparecer -------------------
        gameObject.SetActive(false);

        // ------------------- Diálogo -------------------
        if (indiceDialogo >= 0 && GameManager.Instance != null)
        {
            GameManager.Instance.ActivarDialogoPorObjeto(indiceDialogo);
        }
    }
}
