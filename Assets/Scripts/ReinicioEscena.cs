using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Recarga la escena al pulsar la tecla R para volver a ver la máquina desde el principio.
/// </summary>
public class ReinicioEscena : MonoBehaviour
{
    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            Reiniciar();
#else
        if (Input.GetKeyDown(KeyCode.R))
            Reiniciar();
#endif
    }

    void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
