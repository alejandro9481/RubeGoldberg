using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Construye la escena completa de la máquina de Rube Goldberg (16 toques) y la guarda en Assets/Scenes.
/// Se ejecuta desde el menú Taller o por línea de comandos con -executeMethod ConstructorEscena.Construir
/// </summary>
public static class ConstructorEscena
{
    const string CarpetaMateriales = "Assets/Materials";
    const string CarpetaFisica = "Assets/PhysicsMaterials";
    const string CarpetaEscenas = "Assets/Scenes";
    const string RutaEscena = CarpetaEscenas + "/RubeGoldberg.unity";
    const float Z = 0f;
    const float Profundidad = 2f;

    static Material mSuelo, mPlataforma, mRampa, mBola1, mBola2, mBola3, mBola4, mDomino, mDomino1, mTrampolin,
        mPendulo, mSoporte, mBalancin, mEmbudo, mLanzador, mBoton, mTrampilla, mCubo, mMeta, mBandera, mTope, mCubeta;
    static PhysicsMaterial fRebote, fTrampolin, fPendulo;

    [MenuItem("Taller/Construir escena Rube Goldberg")]
    public static void Construir()
    {
        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CrearMateriales();
        CrearIluminacionYCamara();

        var raiz = new GameObject("Maquina").transform;

        // ---------- Suelo ----------
        Caja("Suelo", new Vector3(4.5f, -0.5f, Z), new Vector3(64f, 1f, 10f), 0f, mSuelo, raiz);

        // ---------- NIVEL 1: rampa inicial, dominós y bola 2 ----------
        var nivel1 = Grupo("Nivel 1 - Rampa y dominos", raiz);

        var rampa1 = Rampa("Rampa 1", new Vector2(-21f, 20.2f), new Vector2(-14f, 18f), 0.4f, mRampa, nivel1);
        AgregarToque(rampa1, 1, "Rampa inicial");

        Caja("Plataforma A", new Vector3(-13.15f, 17.8f, Z), new Vector3(2.3f, 0.4f, Profundidad), 0f, mPlataforma, nivel1);
        Caja("Soporte A", new Vector3(-13f, 8.8f, Z), new Vector3(0.5f, 17.6f, 0.5f), 0f, mSoporte, nivel1);

        var bola1 = Bola("Bola 1", new Vector3(-20.3f, 21f, Z), 0.5f, mBola1, 1f);

        // Cubeta bajo el hueco (recoge la bola 1 después de golpear el primer dominó)
        Caja("Cubeta fondo", new Vector3(-11.1f, 14.85f, Z), new Vector3(2.6f, 0.3f, Profundidad), 0f, mCubeta, nivel1);
        Caja("Cubeta pared izquierda", new Vector3(-12.25f, 16.3f, Z), new Vector3(0.3f, 2.6f, Profundidad), 0f, mCubeta, nivel1);
        Caja("Cubeta pared derecha", new Vector3(-9.95f, 15.8f, Z), new Vector3(0.3f, 1.6f, Profundidad), 0f, mCubeta, nivel1);

        Caja("Plataforma B", new Vector3(-7.2f, 16.8f, Z), new Vector3(6f, 0.4f, Profundidad), 0f, mPlataforma, nivel1);
        Caja("Soporte B", new Vector3(-7.2f, 8.3f, Z), new Vector3(0.5f, 16.6f, 0.5f), 0f, mSoporte, nivel1);

        for (int i = 0; i < 6; i++)
        {
            float x = -9.8f + 0.9f * i;
            bool primero = i == 0;
            var domino = Domino("Dominó " + (i + 1), x, 17f, primero ? mDomino1 : mDomino, primero ? 3f : 1f, primero ? fRebote : null, nivel1);
            if (primero) AgregarToque(domino, 2, "Dominós");
        }

        Caja("Pedestal", new Vector3(-4.5f, 17.15f, Z), new Vector3(0.6f, 0.3f, 1.2f), 0f, mTope, nivel1);
        var bola2 = Bola("Bola 2", new Vector3(-4.5f, 17.8f, Z), 0.5f, mBola2, 1f);
        AgregarToque(bola2, 3, "Bola 2 empujada");

        // ---------- NIVEL 2: trampolín, rampa 2, péndulo y bola 3 ----------
        var nivel2 = Grupo("Nivel 2 - Trampolin y pendulo", raiz);

        var trampolin = Caja("Trampolín", new Vector3(-3.3f, 13.2f, Z), new Vector3(2.6f, 0.3f, Profundidad), -20f, mTrampolin, nivel2);
        trampolin.GetComponent<Collider>().sharedMaterial = fTrampolin;
        AgregarToque(trampolin, 4, "Trampolín");
        Caja("Soporte trampolín", new Vector3(-3.3f, 6.5f, Z), new Vector3(0.4f, 13f, 0.4f), 0f, mSoporte, nivel2);

        var rampa2 = Rampa("Rampa 2", new Vector2(0f, 13.6f), new Vector2(8f, 12.2f), 0.4f, mRampa, nivel2);
        AgregarToque(rampa2, 5, "Rampa 2");
        Caja("Soporte rampa 2", new Vector3(1.5f, 6.5f, Z), new Vector3(0.4f, 13f, 0.4f), 0f, mSoporte, nivel2);

        Caja("Plataforma C", new Vector3(10.9f, 12f, Z), new Vector3(6.2f, 0.4f, Profundidad), 0f, mPlataforma, nivel2);
        Caja("Tope bajo", new Vector3(10.7f, 12.25f, Z), new Vector3(0.2f, 0.1f, Profundidad), 0f, mTope, nivel2);

        CrearPendulo(new Vector3(10f, 16.2f, Z), 3.5f, 0.35f, nivel2);

        var bola3 = Bola("Bola 3", new Vector3(11.9f, 12.7f, Z), 0.5f, mBola3, 1f);
        AgregarToque(bola3, 7, "Bola 3 golpeada");

        // ---------- NIVEL 3: embudo, balancín, rampa 3, lanzador y botón ----------
        var nivel3 = Grupo("Nivel 3 - Embudo, balancin y lanzador", raiz);

        var embudoIzq = Rampa("Embudo izquierdo", new Vector2(12.5f, 10.5f), new Vector2(15f, 8f), 0.25f, mEmbudo, nivel3);
        AgregarToque(embudoIzq, 8, "Embudo");
        var embudoDer = Rampa("Embudo derecho", new Vector2(16.4f, 8f), new Vector2(18.5f, 10.5f), 0.25f, mEmbudo, nivel3);
        AgregarToque(embudoDer, 8, "Embudo");

        CrearBalancin(new Vector3(12.3f, 6.3f, Z), 7f, nivel3);

        var rampa3 = Rampa("Rampa 3", new Vector2(16.2f, 5.3f), new Vector2(25.3f, 4.6f), 0.4f, mRampa, nivel3);
        AgregarToque(rampa3, 10, "Rampa 3");
        Caja("Tope rampa 3", new Vector3(25.4f, 5.1f, Z), new Vector3(0.2f, 1.2f, Profundidad), 0f, mTope, nivel3);

        // Lanzador (resorte): trigger que imprime la velocidad de lanzamiento
        var lanzador = new GameObject("Lanzador");
        lanzador.transform.SetParent(nivel3, false);
        lanzador.transform.position = new Vector3(24.5f, 5.2f, Z);
        var trigLanzador = lanzador.AddComponent<BoxCollider>();
        trigLanzador.isTrigger = true;
        trigLanzador.size = new Vector3(0.6f, 1.2f, 1.4f);
        var compLanzador = lanzador.AddComponent<Lanzador>();
        compLanzador.velocidadLanzamiento = new Vector3(-1.5f, 13f, 0f);
        AgregarToque(lanzador, 11, "Lanzador");
        var resorte = Caja("Resorte", new Vector3(24.5f, 4.68f, Z), new Vector3(0.7f, 0.25f, 1.2f), -5.6f, mLanzador, lanzador.transform);
        Object.DestroyImmediate(resorte.GetComponent<Collider>());

        // Plataforma del botón (donde aterriza la bola lanzada)
        Caja("Plataforma botón", new Vector3(20.7f, 10.3f, Z), new Vector3(3f, 0.4f, Profundidad), 0f, mPlataforma, nivel3);
        Caja("Pared botón", new Vector3(19.1f, 12.3f, Z), new Vector3(0.2f, 4f, Profundidad), 0f, mPlataforma, nivel3);
        Caja("Soporte botón", new Vector3(20.7f, 7.6f, 1.2f), new Vector3(0.4f, 5f, 0.4f), 0f, mSoporte, nivel3);

        var boton = new GameObject("Botón");
        boton.transform.SetParent(nivel3, false);
        boton.transform.position = new Vector3(20.7f, 10.9f, Z);
        var trigBoton = boton.AddComponent<BoxCollider>();
        trigBoton.isTrigger = true;
        trigBoton.size = new Vector3(2.8f, 0.6f, 1.4f);
        var compBoton = boton.AddComponent<Boton>();
        AgregarToque(boton, 12, "Botón");
        var botonVisual = Caja("Botón visual", new Vector3(20.7f, 10.55f, Z), new Vector3(1.2f, 0.1f, 1.2f), 0f, mBoton, boton.transform);
        Object.DestroyImmediate(botonVisual.GetComponent<Collider>());
        compBoton.indicador = botonVisual.GetComponent<Renderer>();

        // ---------- NIVEL 4: trampilla, rampa 4, torre y meta ----------
        var nivel4 = Grupo("Nivel 4 - Trampilla, torre y meta", raiz);

        var trampilla = CrearTrampilla(new Vector3(28.3f, 8.6f, Z), 2f, nivel4);
        compBoton.trampilla = trampilla.GetComponent<Trampilla>();
        Caja("Soporte trampilla", new Vector3(29.75f, 6.2f, Z), new Vector3(0.5f, 4.9f, 1.2f), 0f, mSoporte, nivel4);
        Bola("Bola 4", new Vector3(28f, 9.2f, Z), 0.5f, mBola4, 1f);

        var rampa4 = Rampa("Rampa 4", new Vector2(30.5f, 3.9f), new Vector2(17.5f, 1.6f), 0.4f, mRampa, nivel4);
        AgregarToque(rampa4, 13, "Rampa 4");

        // Tramo plano para la torre y, justo después, una bajada para que la bola no se frene con los cubos caídos
        Caja("Plataforma E", new Vector3(15.9f, 1.4f, Z), new Vector3(3.4f, 0.4f, Profundidad), 0f, mPlataforma, nivel4);
        Rampa("Bajada E", new Vector2(10.3f, 0.75f), new Vector2(14.2f, 1.6f), 0.4f, mPlataforma, nivel4);

        CrearCompuerta(new Vector3(14.7f, 3.1f, Z), 1.3f, nivel4);

        var rampaFinal = Rampa("Rampa final", new Vector2(10.4f, 0.75f), new Vector2(7f, 0.2f), 0.3f, mRampa, nivel4);
        AgregarToque(rampaFinal, 15, "Rampa final");

        Caja("Meta base", new Vector3(5f, 0.05f, Z), new Vector3(4.2f, 0.1f, Profundidad), 0f, mMeta, nivel4);
        Caja("Meta pared", new Vector3(3f, 1f, Z), new Vector3(0.3f, 2f, Profundidad), 0f, mMeta, nivel4);
        Caja("Mástil", new Vector3(3f, 3f, Z), new Vector3(0.1f, 2f, 0.1f), 0f, mSoporte, nivel4);
        Caja("Bandera", new Vector3(3.5f, 3.7f, Z), new Vector3(0.9f, 0.5f, 0.05f), 0f, mBandera, nivel4);

        var meta = new GameObject("Meta");
        meta.transform.SetParent(nivel4, false);
        meta.transform.position = new Vector3(5f, 0.8f, Z);
        var trigMeta = meta.AddComponent<BoxCollider>();
        trigMeta.isTrigger = true;
        trigMeta.size = new Vector3(3f, 1.2f, 1.6f);
        AgregarToque(meta, 16, "Meta");

        // ---------- Interfaz y gestor ----------
        CrearInterfazYGestor();

        // ---------- Guardar ----------
        Directory.CreateDirectory(CarpetaEscenas);
        EditorSceneManager.SaveScene(escena, RutaEscena);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(RutaEscena, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Escena construida y guardada en " + RutaEscena);
    }

    // ------------------------------------------------------------------
    // Elementos compuestos
    // ------------------------------------------------------------------

    static void CrearPendulo(Vector3 pivote, float largo, float radioBola, Transform padre)
    {
        var pendulo = new GameObject("Péndulo");
        pendulo.transform.SetParent(padre, false);
        pendulo.transform.position = pivote;

        var rb = pendulo.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.sleepThreshold = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var bisagra = pendulo.AddComponent<HingeJoint>();
        bisagra.axis = Vector3.forward;
        bisagra.anchor = Vector3.zero;
        bisagra.useLimits = false;

        var cuerda = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cuerda.name = "Cuerda";
        Object.DestroyImmediate(cuerda.GetComponent<Collider>());
        cuerda.transform.SetParent(pendulo.transform, false);
        cuerda.transform.localPosition = new Vector3(0f, -largo * 0.5f, 0f);
        cuerda.transform.localScale = new Vector3(0.08f, largo * 0.5f, 0.08f);
        cuerda.GetComponent<Renderer>().sharedMaterial = mSoporte;

        var bola = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bola.name = "Bola péndulo";
        bola.transform.SetParent(pendulo.transform, false);
        bola.transform.localPosition = new Vector3(0f, -largo, 0f);
        bola.transform.localScale = Vector3.one * radioBola * 2f;
        bola.GetComponent<Renderer>().sharedMaterial = mPendulo;
        bola.GetComponent<Collider>().sharedMaterial = fPendulo;

        AgregarToque(pendulo, 6, "Péndulo");

        // Estructura que sostiene el péndulo (los postes van detrás para no estorbar el vaivén)
        Caja("Viga péndulo", pivote + new Vector3(0f, 0.25f, 0f), new Vector3(3.4f, 0.25f, 0.25f), 0f, mSoporte, padre);
        Caja("Poste péndulo izquierdo", new Vector3(pivote.x - 1.6f, 14.4f, 1.1f), new Vector3(0.25f, 4.4f, 0.25f), 0f, mSoporte, padre);
        Caja("Poste péndulo derecho", new Vector3(pivote.x + 1.6f, 14.4f, 1.1f), new Vector3(0.25f, 4.4f, 0.25f), 0f, mSoporte, padre);
        Caja("Travesaño izquierdo", new Vector3(pivote.x - 1.6f, pivote.y + 0.25f, 0.55f), new Vector3(0.25f, 0.25f, 1.35f), 0f, mSoporte, padre);
        Caja("Travesaño derecho", new Vector3(pivote.x + 1.6f, pivote.y + 0.25f, 0.55f), new Vector3(0.25f, 0.25f, 1.35f), 0f, mSoporte, padre);
    }

    static void CrearBalancin(Vector3 pivote, float largo, Transform padre)
    {
        var balancin = Caja("Balancín", pivote, new Vector3(largo, 0.2f, 1.6f), 0f, mBalancin, padre, false);
        var rb = balancin.AddComponent<Rigidbody>();
        rb.mass = 0.5f; // ligera para que se incline rápido cuando cae la bola
        rb.sleepThreshold = 0f;

        var bisagra = balancin.AddComponent<HingeJoint>();
        bisagra.axis = Vector3.forward;
        bisagra.anchor = Vector3.zero;
        bisagra.useLimits = true;
        bisagra.limits = new JointLimits { min = -12f, max = 12f };
        AgregarToque(balancin, 9, "Balancín");

        // Soporte detrás del balancín (no toca la tabla)
        Caja("Soporte balancín", new Vector3(pivote.x, pivote.y * 0.5f, 1.2f), new Vector3(0.4f, pivote.y, 0.4f), 0f, mSoporte, padre);
        var eje = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        eje.name = "Eje balancín";
        Object.DestroyImmediate(eje.GetComponent<Collider>());
        eje.transform.SetParent(padre, false);
        eje.transform.position = pivote + new Vector3(0f, 0f, 0.6f);
        eje.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        eje.transform.localScale = new Vector3(0.12f, 0.6f, 0.12f);
        eje.GetComponent<Renderer>().sharedMaterial = mSoporte;
    }

    /// <summary>Compuerta basculante: una tabla que cuelga de una bisagra; la bola la empuja, pasa por debajo y la tabla vuelve a caer.</summary>
    static void CrearCompuerta(Vector3 bisagraPos, float largo, Transform padre)
    {
        var compuerta = Caja("Compuerta", bisagraPos + new Vector3(0f, -largo * 0.5f, 0f), new Vector3(0.12f, largo, 1.2f), 0f, mCubo, padre, false);
        var rb = compuerta.AddComponent<Rigidbody>();
        rb.mass = 0.2f;
        rb.sleepThreshold = 0f;

        var bisagra = compuerta.AddComponent<HingeJoint>();
        bisagra.axis = Vector3.forward;
        bisagra.anchor = new Vector3(0f, 0.5f, 0f); // borde superior (coordenadas locales)
        bisagra.useLimits = true;
        bisagra.limits = new JointLimits { min = -120f, max = 120f };
        AgregarToque(compuerta, 14, "Compuerta");

        // Marco: dos postes detrás y un travesaño que sostiene la bisagra
        Caja("Poste compuerta", new Vector3(bisagraPos.x, bisagraPos.y * 0.5f, 1.2f), new Vector3(0.2f, bisagraPos.y + 0.2f, 0.2f), 0f, mSoporte, padre);
        Caja("Travesaño compuerta", bisagraPos + new Vector3(0f, 0.15f, 0.55f), new Vector3(0.2f, 0.2f, 1.5f), 0f, mSoporte, padre);
    }

    static GameObject CrearTrampilla(Vector3 centro, float largo, Transform padre)
    {
        var trampilla = Caja("Trampilla", centro, new Vector3(largo, 0.2f, 1.6f), 0f, mTrampilla, padre, false);
        var rb = trampilla.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.isKinematic = true;

        var bisagra = trampilla.AddComponent<HingeJoint>();
        bisagra.axis = Vector3.forward;
        bisagra.anchor = new Vector3(0.5f, 0f, 0f); // borde derecho (coordenadas locales)
        bisagra.useLimits = true;
        bisagra.limits = new JointLimits { min = -88f, max = 88f };

        trampilla.AddComponent<Trampilla>();
        return trampilla;
    }

    static void CrearInterfazYGestor()
    {
        var fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var escalador = canvasGo.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920f, 1080f);
        escalador.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGo.transform, false);
        var imagenPanel = panel.AddComponent<Image>();
        imagenPanel.color = new Color(0f, 0f, 0f, 0.55f);
        var rtPanel = panel.GetComponent<RectTransform>();
        // Abajo a la izquierda: es la zona de la pantalla que queda libre de piezas de la máquina
        rtPanel.anchorMin = new Vector2(0f, 0f);
        rtPanel.anchorMax = new Vector2(0f, 0f);
        rtPanel.pivot = new Vector2(0f, 0f);
        rtPanel.anchoredPosition = new Vector2(20f, 20f);
        rtPanel.sizeDelta = new Vector2(420f, 560f);

        var titulo = Texto("Título", panel.transform, fuente, 30, FontStyle.Bold, "Máquina de Rube Goldberg", new Vector2(15f, -12f), new Vector2(390f, 40f));
        var contador = Texto("Contador", panel.transform, fuente, 28, FontStyle.Bold, "Toques: 0 / 16", new Vector2(15f, -55f), new Vector2(390f, 36f));
        contador.color = new Color(1f, 0.85f, 0.2f);
        var lista = Texto("Lista", panel.transform, fuente, 20, FontStyle.Normal, "", new Vector2(15f, -98f), new Vector2(390f, 420f));
        var ayuda = Texto("Ayuda", panel.transform, fuente, 18, FontStyle.Italic, "Pulsa R para reiniciar", new Vector2(15f, -525f), new Vector2(390f, 30f));
        ayuda.color = new Color(0.8f, 0.8f, 0.8f);

        var final = Texto("Final", canvasGo.transform, fuente, 64, FontStyle.Bold, "¡Máquina completada!", Vector2.zero, new Vector2(1200f, 100f));
        var rtFinal = final.GetComponent<RectTransform>();
        rtFinal.anchorMin = new Vector2(0.5f, 0.5f);
        rtFinal.anchorMax = new Vector2(0.5f, 0.5f);
        rtFinal.pivot = new Vector2(0.5f, 0.5f);
        rtFinal.anchoredPosition = new Vector2(0f, 300f);
        final.alignment = TextAnchor.MiddleCenter;
        final.color = new Color(0.1f, 0.75f, 0.2f);
        final.gameObject.AddComponent<Outline>().effectColor = Color.black;
        final.gameObject.SetActive(false);

        var gestor = new GameObject("Gestor");
        var contadorComp = gestor.AddComponent<ContadorToques>();
        contadorComp.textoContador = contador;
        contadorComp.textoLista = lista;
        contadorComp.textoFinal = final;
        gestor.AddComponent<ReinicioEscena>();
    }

    static Text Texto(string nombre, Transform padre, Font fuente, int tamano, FontStyle estilo, string contenido, Vector2 posicion, Vector2 tamanoRect)
    {
        var go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        var texto = go.AddComponent<Text>();
        texto.font = fuente;
        texto.fontSize = tamano;
        texto.fontStyle = estilo;
        texto.text = contenido;
        texto.color = Color.white;
        texto.alignment = TextAnchor.UpperLeft;
        texto.horizontalOverflow = HorizontalWrapMode.Overflow;
        texto.verticalOverflow = VerticalWrapMode.Overflow;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = posicion;
        rt.sizeDelta = tamanoRect;
        return texto;
    }

    // ------------------------------------------------------------------
    // Primitivas
    // ------------------------------------------------------------------

    static Transform Grupo(string nombre, Transform padre)
    {
        var go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        return go.transform;
    }

    static GameObject Caja(string nombre, Vector3 centro, Vector3 tamano, float rotZ, Material mat, Transform padre, bool estatico = true)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = nombre;
        go.transform.position = centro;
        go.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
        go.transform.localScale = tamano;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        go.isStatic = estatico;
        if (padre != null) go.transform.SetParent(padre, true);
        return go;
    }

    /// <summary>Caja inclinada cuya cara superior va exactamente de "desde" a "hasta".</summary>
    static GameObject Rampa(string nombre, Vector2 desde, Vector2 hasta, float grosor, Material mat, Transform padre)
    {
        if (desde.x > hasta.x) { var tmp = desde; desde = hasta; hasta = tmp; }
        var d = hasta - desde;
        float largo = d.magnitude;
        float angulo = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
        var normal = new Vector2(-d.y, d.x).normalized;
        if (normal.y < 0f) normal = -normal;
        var centro = (desde + hasta) * 0.5f - normal * (grosor * 0.5f);
        return Caja(nombre, new Vector3(centro.x, centro.y, Z), new Vector3(largo, grosor, Profundidad), angulo, mat, padre);
    }

    static GameObject Bola(string nombre, Vector3 centro, float radio, Material mat, float masa)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = nombre;
        go.transform.position = centro;
        go.transform.localScale = Vector3.one * radio * 2f;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        var rb = go.AddComponent<Rigidbody>();
        rb.mass = masa;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.sleepThreshold = 0f;
        return go;
    }

    static GameObject Domino(string nombre, float x, float ySuelo, Material mat, float masa, PhysicsMaterial fisica, Transform padre)
    {
        var go = Caja(nombre, new Vector3(x, ySuelo + 0.8f, Z), new Vector3(0.15f, 1.6f, 0.8f), 0f, mat, padre, false);
        var rb = go.AddComponent<Rigidbody>();
        rb.mass = masa;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        if (fisica != null) go.GetComponent<Collider>().sharedMaterial = fisica;
        return go;
    }

    static void AgregarToque(GameObject go, int orden, string nombre)
    {
        var toque = go.AddComponent<Toque>();
        toque.orden = orden;
        toque.nombre = nombre;
    }

    // ------------------------------------------------------------------
    // Materiales, luz y cámara
    // ------------------------------------------------------------------

    static void CrearMateriales()
    {
        Directory.CreateDirectory(CarpetaMateriales);
        Directory.CreateDirectory(CarpetaFisica);

        mSuelo = Mat("Suelo", new Color(0.30f, 0.32f, 0.36f));
        mPlataforma = Mat("Plataforma", new Color(0.62f, 0.64f, 0.68f));
        mRampa = Mat("Rampa", new Color(0.45f, 0.60f, 0.80f));
        mSoporte = Mat("Soporte", new Color(0.40f, 0.36f, 0.33f));
        mBola1 = Mat("Bola 1", new Color(0.90f, 0.15f, 0.15f));
        mBola2 = Mat("Bola 2", new Color(0.98f, 0.60f, 0.10f));
        mBola3 = Mat("Bola 3", new Color(0.20f, 0.75f, 0.30f));
        mBola4 = Mat("Bola 4", new Color(0.20f, 0.45f, 0.95f));
        mDomino = Mat("Domino", new Color(0.95f, 0.93f, 0.85f));
        mDomino1 = Mat("Domino rebote", new Color(0.95f, 0.80f, 0.35f));
        mTrampolin = Mat("Trampolin", new Color(0.85f, 0.20f, 0.65f));
        mPendulo = Mat("Pendulo", new Color(1.00f, 0.85f, 0.10f));
        mBalancin = Mat("Balancin", new Color(0.60f, 0.40f, 0.20f));
        mEmbudo = Mat("Embudo", new Color(0.20f, 0.70f, 0.70f));
        mLanzador = Mat("Lanzador", new Color(0.95f, 0.35f, 0.10f));
        mBoton = Mat("Boton", new Color(0.90f, 0.10f, 0.10f));
        mTrampilla = Mat("Trampilla", new Color(0.55f, 0.35f, 0.65f));
        mCubo = Mat("Compuerta", new Color(0.60f, 0.30f, 0.75f));
        mMeta = Mat("Meta", new Color(0.15f, 0.55f, 0.25f));
        mBandera = Mat("Bandera", new Color(0.95f, 0.10f, 0.10f));
        mTope = Mat("Tope", new Color(0.25f, 0.25f, 0.28f));
        mCubeta = Mat("Cubeta", new Color(0.50f, 0.55f, 0.60f));

        fRebote = Fisica("Rebote", 0.6f, 0.8f);
        fTrampolin = Fisica("Trampolin", 0.4f, 0.7f);
        fPendulo = Fisica("Pendulo", 0.3f, 0.9f);
    }

    static Material Mat(string nombre, Color color)
    {
        string ruta = CarpetaMateriales + "/" + nombre + ".mat";
        // El proyecto usa URP (Universal 3D); el shader Standard se vería magenta
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        var existente = AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if (existente != null)
        {
            existente.shader = shader;
            existente.color = color;
            EditorUtility.SetDirty(existente);
            return existente;
        }
        var mat = new Material(shader);
        mat.color = color;
        AssetDatabase.CreateAsset(mat, ruta);
        return mat;
    }

    static PhysicsMaterial Fisica(string nombre, float friccion, float rebote)
    {
        string ruta = CarpetaFisica + "/" + nombre + ".physicMaterial";
        var existente = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(ruta);
        if (existente != null) return existente;
        var fis = new PhysicsMaterial(nombre)
        {
            dynamicFriction = friccion,
            staticFriction = friccion,
            bounciness = rebote,
            frictionCombine = PhysicsMaterialCombine.Average,
            bounceCombine = PhysicsMaterialCombine.Maximum
        };
        AssetDatabase.CreateAsset(fis, ruta);
        return fis;
    }

    static void CrearIluminacionYCamara()
    {
        var luzGo = new GameObject("Directional Light");
        var luz = luzGo.AddComponent<Light>();
        luz.type = LightType.Directional;
        luz.intensity = 1.1f;
        luz.shadows = LightShadows.Soft;
        luz.color = new Color(1f, 0.97f, 0.92f);
        luzGo.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

        RenderSettings.skybox = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Skybox.mat");
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.55f, 0.57f, 0.62f);

        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.fieldOfView = 50f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.80f, 0.87f, 0.95f);
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 200f;
        camGo.transform.position = new Vector3(4.8f, 10.5f, -33.5f);
        camGo.transform.rotation = Quaternion.identity;
        camGo.AddComponent<AudioListener>();
    }
}
