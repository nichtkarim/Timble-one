using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Erstellt schwebenden Text für Damage-Numbers, Punkte, etc.
/// </summary>
public class FloatingText : MonoBehaviour
{
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    private TextMeshPro textMesh;
    private Color originalColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            textMesh = gameObject.AddComponent<TextMeshPro>();
        }
        
        originalColor = textMesh.color;
    }

    void Start()
    {
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < lifetime)
        {
            // Nach oben bewegen
            transform.position = startPos + Vector3.up * (moveSpeed * elapsed);

            // Fade out
            float alpha = fadeCurve.Evaluate(elapsed / lifetime);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Erstellt schwebenden Text an einer Position
    /// </summary>
    public static void Create(string text, Vector3 position, Color color, float size = 1f)
    {
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.position = position;

        FloatingText floatingText = textObj.AddComponent<FloatingText>();
        TextMeshPro textMesh = textObj.GetComponent<TextMeshPro>();

        textMesh.text = text;
        textMesh.fontSize = 5 * size;
        textMesh.color = color;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontStyle = FontStyles.Bold;

        // Outline für bessere Lesbarkeit
        textMesh.outlineWidth = 0.2f;
        textMesh.outlineColor = Color.black;

        // Zur Kamera ausrichten
        if (Camera.main != null)
        {
            textObj.transform.rotation = Quaternion.LookRotation(textObj.transform.position - Camera.main.transform.position);
        }
    }

    /// <summary>
    /// Erstellt Damage-Text
    /// </summary>
    public static void CreateDamageText(int damage, Vector3 position)
    {
        Create($"-{damage} HP", position, Color.red, 1.2f);
    }

    /// <summary>
    /// Erstellt Heal-Text
    /// </summary>
    public static void CreateHealText(int heal, Vector3 position)
    {
        Create($"+{heal} HP", position, Color.green, 1.2f);
    }

    /// <summary>
    /// Erstellt Punkte-Text
    /// </summary>
    public static void CreateScoreText(int score, Vector3 position)
    {
        Create($"+{score}", position, Color.yellow, 1f);
    }
}
