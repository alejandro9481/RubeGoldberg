using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// Prueba de la máquina: carga la escena, la deja correr y comprueba que se completan los 16 toques.
/// Se ejecuta desde Window > General > Test Runner > PlayMode.
/// </summary>
public class PruebaMaquina
{
    [UnityTest]
    public IEnumerator LaMaquinaCompletaTodosLosToques()
    {
        var registros = new List<string>();
        bool completada = false;
        Application.LogCallback alRecibirLog = (mensaje, pila, tipo) =>
        {
            if (mensaje.StartsWith("[Toque")) registros.Add(mensaje);
            if (mensaje.StartsWith("[Maquina completada]")) completada = true;
        };
        Application.logMessageReceived += alRecibirLog;

#if UNITY_EDITOR
        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
            "Assets/Scenes/RubeGoldberg.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
        SceneManager.LoadScene("RubeGoldberg");
#endif

        float limite = 60f;
        float inicio = Time.time;
        while (!completada && Time.time - inicio < limite)
            yield return null;

        Application.logMessageReceived -= alRecibirLog;
        Assert.IsTrue(completada, "La máquina no completó los toques en " + limite + " s. Registrados: " + string.Join(" | ", registros));
        Assert.AreEqual(16, registros.Count);
    }
}
