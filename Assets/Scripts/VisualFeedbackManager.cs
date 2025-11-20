using UnityEngine;
using System.Collections;

/// <summary>
/// Verwaltet visuelles Feedback wie Partikel, Screen Shake, Slow Motion, etc.
/// </summary>
public class VisualFeedbackManager : MonoBehaviour
{
    public static VisualFeedbackManager Instance;

    [Header("Screen Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeIntensity = 0.3f;
    [SerializeField] private Camera mainCamera;

    [Header("Particle Effects")]
    [SerializeField] private GameObject correctCupParticles;
    [SerializeField] private GameObject wrongCupParticles;
    [SerializeField] private GameObject healParticles;
    [SerializeField] private GameObject damageParticles;
    [SerializeField] private GameObject itemUseParticles;

    [Header("Slow Motion Settings")]
    [SerializeField] private float slowMotionScale = 0.3f;
    [SerializeField] private float slowMotionDuration = 0.5f;

    [Header("Flash Settings")]
    [SerializeField] private Color correctFlashColor = Color.green;
    [SerializeField] private Color wrongFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.2f;

    private Vector3 originalCameraPosition;
    private Coroutine shakeCoroutine;
    private GameObject flashOverlay;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("✅ VisualFeedbackManager Instance erstellt!");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.localPosition;
            Debug.Log("✅ Kamera gefunden: " + mainCamera.name);
        }
        else
        {
            Debug.LogError("❌ KEINE KAMERA GEFUNDEN! Bitte Main Camera im Inspector zuweisen!");
        }

        CreateFlashOverlay();
    }

    #region Screen Shake

    /// <summary>
    /// Startet Screen Shake mit Standard-Einstellungen
    /// </summary>
    public void ShakeCamera()
    {
        ShakeCamera(shakeDuration, shakeIntensity);
    }

    /// <summary>
    /// Startet Screen Shake mit benutzerdefinierten Werten
    /// </summary>
    public void ShakeCamera(float duration, float intensity)
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("❌ ShakeCamera: Keine Kamera vorhanden!");
            return;
        }

        Debug.Log("📳 Screen Shake gestartet!");
        
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            mainCamera.transform.localPosition = originalCameraPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalCameraPosition;
    }

    #endregion

    #region Particle Effects

    /// <summary>
    /// Spielt Partikel-Effekt für richtigen Cup
    /// </summary>
    public void PlayCorrectCupEffect(Vector3 position)
    {
        if (correctCupParticles != null)
        {
            GameObject particles = Instantiate(correctCupParticles, position, Quaternion.identity);
            Destroy(particles, 3f);
        }
        else
        {
            // Fallback: Erstelle einfache Partikel
            CreateSimpleParticles(position, Color.green, 20);
        }
    }

    /// <summary>
    /// Spielt Partikel-Effekt für falschen Cup
    /// </summary>
    public void PlayWrongCupEffect(Vector3 position)
    {
        if (wrongCupParticles != null)
        {
            GameObject particles = Instantiate(wrongCupParticles, position, Quaternion.identity);
            Destroy(particles, 3f);
        }
        else
        {
            // Fallback: Erstelle einfache Partikel
            CreateSimpleParticles(position, Color.red, 20);
        }
    }

    /// <summary>
    /// Spielt Heilungs-Effekt
    /// </summary>
    public void PlayHealEffect(Vector3 position)
    {
        if (healParticles != null)
        {
            GameObject particles = Instantiate(healParticles, position, Quaternion.identity);
            Destroy(particles, 3f);
        }
        else
        {
            CreateSimpleParticles(position, Color.cyan, 15);
        }
    }

    /// <summary>
    /// Spielt Schadens-Effekt
    /// </summary>
    public void PlayDamageEffect(Vector3 position)
    {
        if (damageParticles != null)
        {
            GameObject particles = Instantiate(damageParticles, position, Quaternion.identity);
            Destroy(particles, 3f);
        }
        else
        {
            CreateSimpleParticles(position, Color.red, 15);
        }
    }

    /// <summary>
    /// Spielt Item-Nutzungs-Effekt
    /// </summary>
    public void PlayItemUseEffect(Vector3 position)
    {
        if (itemUseParticles != null)
        {
            GameObject particles = Instantiate(itemUseParticles, position, Quaternion.identity);
            Destroy(particles, 3f);
        }
        else
        {
            CreateSimpleParticles(position, Color.yellow, 12);
        }
    }

    /// <summary>
    /// Erstellt einfache Partikel als Fallback
    /// </summary>
    private void CreateSimpleParticles(Vector3 position, Color color, int count)
    {
        Debug.Log("✨ Erstelle Partikel an Position: " + position + " mit Farbe: " + color);
        
        GameObject particleObj = new GameObject("SimpleParticles");
        particleObj.transform.position = position;

        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = color;
        main.startSize = 0.2f;
        main.startSpeed = 3f;
        main.startLifetime = 1f;
        main.maxParticles = count;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, count) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;

        Destroy(particleObj, 2f);
    }

    #endregion

    #region Screen Flash

    private void CreateFlashOverlay()
    {
        // Erstelle ein Canvas für den Flash-Effekt
        GameObject canvasObj = new GameObject("FlashCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        UnityEngine.UI.CanvasScaler scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Erstelle das Flash-Panel
        flashOverlay = new GameObject("FlashPanel");
        flashOverlay.transform.SetParent(canvasObj.transform, false);

        UnityEngine.UI.Image image = flashOverlay.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(1, 1, 1, 0);

        RectTransform rt = flashOverlay.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        DontDestroyOnLoad(canvasObj);
    }

    /// <summary>
    /// Screen Flash für korrekten Cup
    /// </summary>
    public void FlashCorrect()
    {
        StartCoroutine(FlashCoroutine(correctFlashColor));
    }

    /// <summary>
    /// Screen Flash für falschen Cup
    /// </summary>
    public void FlashWrong()
    {
        StartCoroutine(FlashCoroutine(wrongFlashColor));
    }

    private IEnumerator FlashCoroutine(Color color)
    {
        if (flashOverlay == null) yield break;

        UnityEngine.UI.Image image = flashOverlay.GetComponent<UnityEngine.UI.Image>();
        if (image == null) yield break;

        // Fade in
        float elapsed = 0f;
        while (elapsed < flashDuration / 2)
        {
            float alpha = Mathf.Lerp(0, 0.3f, elapsed / (flashDuration / 2));
            image.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fade out
        elapsed = 0f;
        while (elapsed < flashDuration / 2)
        {
            float alpha = Mathf.Lerp(0.3f, 0, elapsed / (flashDuration / 2));
            image.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        image.color = new Color(color.r, color.g, color.b, 0);
    }

    #endregion

    #region Slow Motion

    /// <summary>
    /// Aktiviert Slow Motion für kurze Zeit
    /// </summary>
    public void TriggerSlowMotion()
    {
        TriggerSlowMotion(slowMotionDuration, slowMotionScale);
    }

    /// <summary>
    /// Aktiviert Slow Motion mit benutzerdefinierten Werten
    /// </summary>
    public void TriggerSlowMotion(float duration, float timeScale)
    {
        StartCoroutine(SlowMotionCoroutine(duration, timeScale));
    }

    private IEnumerator SlowMotionCoroutine(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    #endregion

    #region Object Pulse/Scale

    /// <summary>
    /// Lässt ein Objekt kurz aufblinken
    /// </summary>
    public void PulseObject(Transform target, float scale = 1.3f, float duration = 0.3f)
    {
        StartCoroutine(PulseCoroutine(target, scale, duration));
    }

    private IEnumerator PulseCoroutine(Transform target, float scale, float duration)
    {
        if (target == null) yield break;

        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * scale;

        // Scale up
        float elapsed = 0f;
        while (elapsed < duration / 2)
        {
            target.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / (duration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Scale down
        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            target.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / (duration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = originalScale;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Kombinierter Effekt: Richtiger Cup
    /// </summary>
    public void TriggerCorrectCupFeedback(Vector3 position, Transform cupTransform = null)
    {
        Debug.Log("🎉 TriggerCorrectCupFeedback aufgerufen! Position: " + position);
        PlayCorrectCupEffect(position);
        FlashCorrect();
        
        if (cupTransform != null)
        {
            PulseObject(cupTransform, 1.2f, 0.3f);
        }
    }

    /// <summary>
    /// Kombinierter Effekt: Falscher Cup
    /// </summary>
    public void TriggerWrongCupFeedback(Vector3 position, Transform cupTransform = null)
    {
        Debug.Log("💥 TriggerWrongCupFeedback aufgerufen! Position: " + position);
        PlayWrongCupEffect(position);
        FlashWrong();
        ShakeCamera(0.5f, 0.2f);
        
        if (cupTransform != null)
        {
            PulseObject(cupTransform, 1.2f, 0.3f);
        }
    }

    #endregion
}
