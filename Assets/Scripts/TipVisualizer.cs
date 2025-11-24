using UnityEngine;
using System.Collections;

/// <summary>
/// Zeigt visuelles Feedback für Intuition-Tipps auf Cups
/// </summary>
public class TipVisualizer : MonoBehaviour
{
    public static TipVisualizer Instance;

    [Header("Tip Effect Settings")]
    public Color tipGlowColor = new Color(0f, 1f, 1f, 0.5f); // Cyan mit Alpha
    public float glowIntensity = 2f;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.3f;

    private Transform currentTipCup = null;
    private Material originalMaterial;
    private Material glowMaterial;
    private Renderer cupRenderer;
    private Coroutine glowCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Zeigt Glow-Effekt auf dem Cup mit dem Tipp
    /// </summary>
    public void ShowTipOnCup(Transform cupTransform)
    {
        // Entferne alten Effekt falls vorhanden
        RemoveTipEffect();

        currentTipCup = cupTransform;
        cupRenderer = cupTransform.GetComponent<Renderer>();

        if (cupRenderer == null)
        {
            Debug.LogError("❌ Cup hat keinen Renderer!");
            return;
        }

        // Speichere Original-Material
        originalMaterial = cupRenderer.material;

        // Erstelle Glow-Material
        glowMaterial = new Material(originalMaterial);
        glowMaterial.EnableKeyword("_EMISSION");
        glowMaterial.SetColor("_EmissionColor", tipGlowColor * glowIntensity);

        // Starte Pulsieren
        glowCoroutine = StartCoroutine(PulseGlow());

        Debug.Log($"💡 Tipp-Effekt auf {cupTransform.name} angezeigt");
    }

    /// <summary>
    /// Entfernt den Tipp-Effekt
    /// </summary>
    public void RemoveTipEffect()
    {
        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
            glowCoroutine = null;
        }

        if (cupRenderer != null && originalMaterial != null)
        {
            cupRenderer.material = originalMaterial;
            Debug.Log("💡 Tipp-Effekt entfernt");
        }

        currentTipCup = null;
        cupRenderer = null;
    }

    private IEnumerator PulseGlow()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime * pulseSpeed;
            
            // Sinuswelle für Pulsieren
            float pulse = 1f + Mathf.Sin(time) * pulseAmount;
            Color emissionColor = tipGlowColor * glowIntensity * pulse;
            
            if (glowMaterial != null)
            {
                glowMaterial.SetColor("_EmissionColor", emissionColor);
            }

            yield return null;
        }
    }
}
