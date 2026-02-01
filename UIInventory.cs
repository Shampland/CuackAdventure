using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIInventory : MonoBehaviour
{
    public Image[] slots;
    public float duracionAnimacion = 0.2f;

    private int indiceActual = 0;

    private void Start()
    {
        foreach (Image img in slots)
        {
            img.enabled = false;
            img.transform.localScale = Vector3.zero;
        }
    }

    public void AgregarObjeto(Sprite icono)
    {
        if (indiceActual >= slots.Length)
        {
            Debug.Log("Inventario lleno");
            return;
        }

        Image slot = slots[indiceActual];
        slot.sprite = icono;
        slot.enabled = true;

        StartCoroutine(AnimarSlot(slot));

        indiceActual++;
    }

    IEnumerator AnimarSlot(Image slot)
    {
        float tiempo = 0f;
        Vector3 escalaInicial = Vector3.zero;
        Vector3 escalaFinal = Vector3.one;

        // Pop (0 → 1.2)
        while (tiempo < duracionAnimacion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionAnimacion;
            slot.transform.localScale = Vector3.Lerp(
                escalaInicial,
                escalaFinal * 1.2f,
                t
            );
            yield return null;
        }

        // Regresa a escala normal
        slot.transform.localScale = escalaFinal;
    }
}
