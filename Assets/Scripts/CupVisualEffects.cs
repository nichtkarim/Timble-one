using UnityEngine;
using System.Collections;

/// <summary>
/// Fügt visuelle Effekte zu Cups hinzu (Glow, Bounce, Trail, etc.)
/// </summary>
public class CupVisualEffects : MonoBehaviour
{
    [Header("Glow Settings")]
    [SerializeField] private Material glowMaterial;
    [SerializeField] private Color glowColor = Color.yellow;
    [SerializeField] private float glowIntensity = 2f;

    [Header("Bounce Settings")]
    [SerializeField] private float bounceHeight = 0.2f;
    [SerializeField] private float bounceDuration = 0.3f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 180f;

    private Renderer cupRenderer;
    private Material originalMaterial;
    private Vector3 originalPosition;
    private bool isGlowing = false;

    void Awake()
    {
        cupRenderer = GetComponent<Renderer>();
        if (cupRenderer != null)
        {
            originalMaterial = cupRenderer.material;
        }
        originalPosition = transform.localPosition;
    }

    #region Glow Effect

    /// <summary>
    /// Aktiviert Glow-Effekt
    /// </summary>
    public void EnableGlow()
    {
        if (isGlowing || cupRenderer == null) return;

        isGlowing = true;

        // Wenn kein Glow-Material zugewiesen, verwende Emission
        if (glowMaterial != null)
        {
            cupRenderer.material = glowMaterial;
        }
        else if (cupRenderer.material.HasProperty("_EmissionColor"))
        {
            cupRenderer.material.EnableKeyword("_EMISSION");
            cupRenderer.material.SetColor("_EmissionColor", glowColor * glowIntensity);
        }
    }

    /// <summary>
    /// Deaktiviert Glow-Effekt
    /// </summary>
    public void DisableGlow()
    {
        if (!isGlowing || cupRenderer == null) return;

        isGlowing = false;
        cupRenderer.material = originalMaterial;
    }

    /// <summary>
    /// Glow für kurze Zeit
    /// </summary>
    public void GlowTemporary(float duration)
    {
        StartCoroutine(GlowTemporaryCoroutine(duration));
    }

    private IEnumerator GlowTemporaryCoroutine(float duration)
    {
        EnableGlow();
        yield return new WaitForSeconds(duration);
        DisableGlow();
    }

    #endregion

    #region Bounce Effect

    /// <summary>
    /// Lässt Cup hüpfen
    /// </summary>
    public void Bounce()
    {
        StartCoroutine(BounceCoroutine());
    }

    private IEnumerator BounceCoroutine()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 peakPos = startPos + Vector3.up * bounceHeight;

        // Up
        float elapsed = 0f;
        while (elapsed < bounceDuration / 2)
        {
            transform.localPosition = Vector3.Lerp(startPos, peakPos, elapsed / (bounceDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Down
        elapsed = 0f;
        while (elapsed < bounceDuration / 2)
        {
            transform.localPosition = Vector3.Lerp(peakPos, startPos, elapsed / (bounceDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;
    }

    #endregion

    #region Rotation Effect

    /// <summary>
    /// Rotiert Cup um Y-Achse
    /// </summary>
    public void SpinOnce()
    {
        StartCoroutine(SpinCoroutine(360f));
    }

    /// <summary>
    /// Rotiert Cup mit angegebenen Grad
    /// </summary>
    public void Spin(float degrees)
    {
        StartCoroutine(SpinCoroutine(degrees));
    }

    private IEnumerator SpinCoroutine(float degrees)
    {
        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, degrees, 0);

        float duration = Mathf.Abs(degrees) / rotationSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRotation;
    }

    #endregion

    #region Shake Effect

    /// <summary>
    /// Schüttelt den Cup
    /// </summary>
    public void Shake(float duration = 0.5f, float intensity = 0.1f)
    {
        StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float z = Random.Range(-1f, 1f) * intensity;

            transform.localPosition = originalPos + new Vector3(x, 0, z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    #endregion

    #region Highlight Effect

    /// <summary>
    /// Hebt Cup visuell hervor (für Hinweise)
    /// </summary>
    public void Highlight(float duration = 2f)
    {
        StartCoroutine(HighlightCoroutine(duration));
    }

    private IEnumerator HighlightCoroutine(float duration)
    {
        EnableGlow();
        Bounce();
        yield return new WaitForSeconds(duration);
        DisableGlow();
    }

    #endregion

    #region Wobble Effect

    /// <summary>
    /// Wackelt den Cup hin und her
    /// </summary>
    public void Wobble(float duration = 0.5f)
    {
        StartCoroutine(WobbleCoroutine(duration));
    }

    private IEnumerator WobbleCoroutine(float duration)
    {
        Quaternion originalRotation = transform.localRotation;
        float elapsed = 0f;
        float frequency = 10f;
        float amplitude = 10f;

        while (elapsed < duration)
        {
            float angle = Mathf.Sin(elapsed * frequency) * amplitude * (1 - elapsed / duration);
            transform.localRotation = originalRotation * Quaternion.Euler(0, 0, angle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = originalRotation;
    }

    #endregion

    void OnDestroy()
    {
        // Cleanup
        if (cupRenderer != null && originalMaterial != null)
        {
            cupRenderer.material = originalMaterial;
        }
    }
}
