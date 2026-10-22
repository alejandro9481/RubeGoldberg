using UnityEngine;

/// <summary>
/// Zona trigger que actúa como resorte: a todo cuerpo que entra le asigna una velocidad de lanzamiento.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Lanzador : MonoBehaviour
{
    public Vector3 velocidadLanzamiento = new Vector3(-2f, 12.5f, 0f);

    void OnTriggerEnter(Collider otro)
    {
        var cuerpo = otro.attachedRigidbody;
        if (cuerpo == null) return;

        cuerpo.linearVelocity = velocidadLanzamiento;
        cuerpo.angularVelocity = Vector3.zero;
        Debug.Log($"[Lanzador] {otro.name} lanzado con velocidad {velocidadLanzamiento}");
    }
}
