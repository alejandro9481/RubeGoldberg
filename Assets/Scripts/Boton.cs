using UnityEngine;

/// <summary>
/// Botón de presión: cuando un cuerpo entra en su trigger abre la trampilla asociada.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Boton : MonoBehaviour
{
    public Trampilla trampilla;
    public Renderer indicador;
    public Color colorPulsado = new Color(0.2f, 0.9f, 0.3f);

    bool pulsado;

    void OnTriggerEnter(Collider otro)
    {
        if (pulsado || otro.attachedRigidbody == null) return;

        pulsado = true;
        Debug.Log($"[Boton] pulsado por {otro.name}");

        if (indicador != null)
            indicador.material.color = colorPulsado;

        if (trampilla != null)
            trampilla.Abrir();
    }
}
