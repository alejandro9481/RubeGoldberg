using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lleva la cuenta de los toques de la máquina y actualiza la interfaz.
/// </summary>
public class ContadorToques : MonoBehaviour
{
    public static ContadorToques instancia;

    [Header("Interfaz")]
    public Text textoContador;
    public Text textoLista;
    public Text textoFinal;

    readonly SortedDictionary<int, Toque> toques = new SortedDictionary<int, Toque>();
    readonly HashSet<int> completados = new HashSet<int>();
    int total;

    void Awake()
    {
        instancia = this;

        // Varios objetos pueden compartir el mismo orden (las dos paredes del embudo, los cubos de la torre)
        foreach (var toque in FindObjectsByType<Toque>(FindObjectsSortMode.None))
        {
            if (!toques.ContainsKey(toque.orden))
                toques[toque.orden] = toque;
        }

        total = toques.Count;

        if (textoFinal != null)
            textoFinal.gameObject.SetActive(false);

        Refrescar();
    }

    public void Registrar(Toque toque, GameObject causante)
    {
        if (completados.Contains(toque.orden)) return;

        completados.Add(toque.orden);
        Debug.Log($"[Toque {completados.Count}/{total}] #{toque.orden} {toque.nombre} <- {causante.name} (t={Time.timeSinceLevelLoad:F2}s)");
        Refrescar();

        if (completados.Count >= total)
        {
            Debug.Log($"[Maquina completada] t={Time.timeSinceLevelLoad:F2}s");
            if (textoFinal != null)
                textoFinal.gameObject.SetActive(true);
        }
    }

    void Refrescar()
    {
        if (textoContador != null)
            textoContador.text = $"Toques: {completados.Count} / {total}";

        if (textoLista != null)
        {
            var sb = new StringBuilder();
            foreach (var par in toques)
            {
                sb.Append(completados.Contains(par.Key) ? "[x] " : "[  ] ");
                sb.Append(par.Key).Append(". ").Append(par.Value.nombre).Append('\n');
            }
            textoLista.text = sb.ToString();
        }
    }
}
