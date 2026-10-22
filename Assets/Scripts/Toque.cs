using UnityEngine;

/// <summary>
/// Marca un elemento de la máquina como un "toque" de la cadena.
/// Se registra la primera vez que algo lo golpea (colisión) o entra en su trigger.
/// </summary>
public class Toque : MonoBehaviour
{
    [Tooltip("Posición de este toque dentro de la cadena (1 = primero)")]
    public int orden = 1;

    [Tooltip("Nombre que se muestra en pantalla")]
    public string nombre = "Toque";

    [Tooltip("Velocidad relativa mínima para contar una colisión (evita los contactos de reposo)")]
    public float velocidadMinima = 0.3f;

    bool registrado;

    void OnCollisionEnter(Collision colision)
    {
        // Solo cuenta un golpe de un objeto con física (una bola, un dominó, el péndulo...)
        if (colision.rigidbody == null) return;
        if (colision.relativeVelocity.magnitude < velocidadMinima) return;

        // Las piezas de un mismo toque (los cubos de la torre, las paredes del embudo) no cuentan entre sí
        var otro = colision.gameObject.GetComponentInParent<Toque>();
        if (otro != null && otro.orden == orden) return;

        Registrar(colision.gameObject);
    }

    void OnTriggerEnter(Collider otro)
    {
        if (otro.attachedRigidbody == null) return;
        Registrar(otro.gameObject);
    }

    void Registrar(GameObject causante)
    {
        if (registrado) return;
        // Ignorar los contactos del primer instante (objetos que se asientan al cargar la escena)
        if (Time.timeSinceLevelLoad < 0.1f) return;

        registrado = true;
        if (ContadorToques.instancia != null)
            ContadorToques.instancia.Registrar(this, causante);
    }
}
