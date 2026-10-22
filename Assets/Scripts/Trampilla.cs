using UnityEngine;

/// <summary>
/// Trampilla con bisagra: empieza cinemática (cerrada) y al abrirse pasa a dinámica
/// para que caiga por su propio peso y suelte lo que tenga encima.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Trampilla : MonoBehaviour
{
    Rigidbody cuerpo;
    bool abierta;

    void Awake()
    {
        cuerpo = GetComponent<Rigidbody>();
        cuerpo.isKinematic = true;
    }

    public void Abrir()
    {
        if (abierta) return;

        abierta = true;
        cuerpo.isKinematic = false;
        cuerpo.WakeUp();
        Debug.Log("[Trampilla] abierta");
    }
}
