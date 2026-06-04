using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Ponte entre o C# e o JavaScript (GamePlugin.jslib).
/// Coloque este script em um GameObject chamado "GameResultSender" na cena.
/// </summary>
public class GameResultSender : MonoBehaviour
{
    public static GameResultSender instance;

    [DllImport("__Internal")]
    private static extern void EntrarFullscreen();

    [DllImport("__Internal")]
    private static extern void EnviarResultadoFase(string json);

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Chama o fullscreen via jslib (funciona no WebGL pois é acionado por gesto do usuário).
    /// </summary>
    public void AtivarFullscreen()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        EntrarFullscreen();
#endif
    }

    /// <summary>
    /// Envia os dados da fase para a plataforma via fetch no JavaScript.
    /// </summary>
    public void Enviar(int fase, int pontuacao, int acertos, int erros,
                       int aproveitamento, int tempoTotal,
                       string operacoesErradasJson, bool concluiuFase)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string json = $"{{" +
            $"\"fase\":{fase}," +
            $"\"pontuacao\":{pontuacao}," +
            $"\"acertos\":{acertos}," +
            $"\"erros\":{erros}," +
            $"\"aproveitamento\":{aproveitamento}," +
            $"\"tempo_total\":{tempoTotal}," +
            $"\"operacoes_erradas\":{operacoesErradasJson}," +
            $"\"concluiu_fase\":{(concluiuFase ? "true" : "false")}" +
        $"}}";

        Debug.Log("[GameResultSender] Enviando: " + json);
        EnviarResultadoFase(json);
#else
        Debug.Log("[GameResultSender] (editor) Envio ignorado fora do WebGL.");
#endif
    }
}