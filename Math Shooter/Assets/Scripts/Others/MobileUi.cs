using UnityEngine;

// MobileUI — renderiza os botões de controle na tela para mobile/web.
// Adicione este script em qualquer GameObject da cena (ex: o mesmo do GameManager).
// Ele se auto-desabilita em desktop quando não há toque disponível,
// mas pode ser forçado via inspetor.
public class MobileUI : MonoBehaviour
{
    public static MobileUI instance;

    [Header("Forçar controles mobile mesmo em desktop (testes)")]
    public bool forcarMobile = false;

    private bool isMobile;

    // Referência ao player (buscada automaticamente)
    private PlayerController player;

    // Texturas dos botões
    private Texture2D texBotao;
    private Texture2D texBotaoPress;
    private Texture2D texBotaoAtira;
    private Texture2D texBotaoAtiraPress;

    // Estado de pressionado (para highlight visual)
    private bool pressEsq = false;
    private bool pressDir = false;
    private bool pressAtira = false;

    // IDs de touch por botão (para multi-touch correto)
    private int touchIdEsq = -1;
    private int touchIdDir = -1;
    private int touchIdAtira = -1;

    void Awake()
    {
        instance = this;
        isMobile = forcarMobile
                   || Application.isMobilePlatform
                   || SystemInfo.deviceType == DeviceType.Handheld
                   || (Application.platform == RuntimePlatform.WebGLPlayer
                       && SystemInfo.deviceType != DeviceType.Desktop);
    }

    void Start()
    {
        CriarTexturas();
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (!isMobile) return;
        if (player == null) player = FindObjectOfType<PlayerController>();
        if (GameManager.instance != null && !GameManager.instance.JogoRodando()) return;

        ProcessarTouch();
    }

    void ProcessarTouch()
    {
        // Reset dos estados a cada frame e reavalia os toques ativos
        bool novoEsq = false;
        bool novoDir = false;
        bool novoAtira = false;

        Rect rEsq, rDir, rAtira;
        CalcularRects(out rEsq, out rDir, out rAtira);

        for (int ti = 0; ti < Input.touchCount; ti++)
        {
            Touch touch = Input.GetTouch(ti);
            // Inverte Y pois Input.touch usa Y de baixo, mas Rect usa Y de cima
            Vector2 pos = new Vector2(touch.position.x, Screen.height - touch.position.y);

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (touch.fingerId == touchIdEsq) { touchIdEsq = -1; }
                if (touch.fingerId == touchIdDir) { touchIdDir = -1; }
                if (touch.fingerId == touchIdAtira) { touchIdAtira = -1; }
                continue;
            }

            if (rEsq.Contains(pos))
            {
                novoEsq = true;
                if (touchIdEsq == -1 && touch.phase == TouchPhase.Began)
                    touchIdEsq = touch.fingerId;
            }
            if (rDir.Contains(pos))
            {
                novoDir = true;
                if (touchIdDir == -1 && touch.phase == TouchPhase.Began)
                    touchIdDir = touch.fingerId;
            }
            if (rAtira.Contains(pos))
            {
                novoAtira = true;
                if (touchIdAtira == -1 && touch.phase == TouchPhase.Began)
                {
                    touchIdAtira = touch.fingerId;
                    player?.PressionarAtira();   // disparo é evento pontual
                }
            }
        }

        // Atualiza estado contínuo de movimento
        if (pressEsq != novoEsq) { pressEsq = novoEsq; player?.PressionarEsquerda(novoEsq); }
        if (pressDir != novoDir) { pressDir = novoDir; player?.PressionarDireita(novoDir); }
        pressAtira = novoAtira;
    }

    void DesenharSimbolo(Rect r, string simbolo, int fontSize)
    {
        GUIStyle s = new GUIStyle();
        s.fontSize = fontSize;
        s.fontStyle = FontStyle.Bold;
        s.normal.textColor = Color.white;
        s.alignment = TextAnchor.MiddleCenter;
        // Sombra sutil
        GUIStyle sombra = new GUIStyle(s);
        sombra.normal.textColor = new Color(0f, 0f, 0f, 0.5f);
        GUI.Label(new Rect(r.x + 2, r.y + 2, r.width, r.height), simbolo, sombra);
        GUI.Label(r, simbolo, s);
    }

    // ── Layout dos botões ──────────────────────────────────────────────
    // Responsivo: tamanho relativo à tela para funcionar em qualquer resolução

    void CalcularRects(out Rect rEsq, out Rect rDir, out Rect rAtira)
    {
        float W = Screen.width;
        float H = Screen.height;
        float btnSize = Mathf.Min(W, H) * 0.18f;
        btnSize = Mathf.Clamp(btnSize, 60f, 130f);
        float margem = btnSize * 0.25f;
        float baseY = H - btnSize - margem;

        // esquerda e direita ficam fora da tela (não usadas com acelerômetro)
        rEsq = new Rect(-500, baseY, btnSize, btnSize);
        rDir = new Rect(-500, baseY, btnSize, btnSize);

        // botão atirar — centralizado na parte inferior
        rAtira = new Rect(W * 0.5f - btnSize * 0.5f, baseY, btnSize, btnSize);
    }

    void OnGUI()
    {
        bool mostrar = isMobile
                       && GameManager.instance != null
                       && GameManager.instance.JogoRodando();
        if (!mostrar) return;

        Rect rEsq, rDir, rAtira;
        CalcularRects(out rEsq, out rDir, out rAtira);

        // só mostra botão atirar
        GUI.DrawTexture(rAtira, pressAtira ? texBotaoAtiraPress : texBotaoAtira);
        DesenharSimbolo(rAtira, "🔥", 32);
    }

    // ── Cria texturas programaticamente (sem assets externos) ──────────

    void CriarTexturas()
    {
        Color corBotao = new Color(0.976f, 0.451f, 0.086f, 0.55f);  // laranja MAT-IA
        Color corBotaoPress = new Color(0.976f, 0.451f, 0.086f, 0.85f);
        Color corAtira = new Color(0.15f, 0.60f, 1.00f, 0.60f);  // azul
        Color corAtiraPress = new Color(0.15f, 0.60f, 1.00f, 0.90f);

        texBotao = CriarTexRounded(80, corBotao, new Color(1f, 1f, 1f, 0.15f));
        texBotaoPress = CriarTexRounded(80, corBotaoPress, new Color(1f, 1f, 1f, 0.30f));
        texBotaoAtira = CriarTexRounded(80, corAtira, new Color(1f, 1f, 1f, 0.15f));
        texBotaoAtiraPress = CriarTexRounded(80, corAtiraPress, new Color(1f, 1f, 1f, 0.30f));
    }

    // Textura quadrada com bordas levemente arredondadas via gradiente de alpha
    Texture2D CriarTexRounded(int size, Color cor, Color bordaCor)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float meio = size * 0.5f;
        float raio = size * 0.42f;  // raio do círculo interno
        float borda = size * 0.06f;  // largura da borda

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - meio;
                float dy = y - meio;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                Color c;
                if (dist < raio - borda)
                    c = cor;
                else if (dist < raio)
                    c = Color.Lerp(cor, bordaCor, (dist - (raio - borda)) / borda);
                else
                    c = new Color(0, 0, 0, 0); // transparente fora do círculo

                tex.SetPixel(x, y, c);
            }
        tex.Apply();
        return tex;
    }

    void OnDestroy()
    {
        Destroy(texBotao);
        Destroy(texBotaoPress);
        Destroy(texBotaoAtira);
        Destroy(texBotaoAtiraPress);
    }
}