using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class IntuitionItem : GameItem
{
    // itemName, icon, description werden von GameItem geerbt - NICHT hier definieren!

    // Beispiel: Item-Effekt ausführen
    
    public override void Use()
    {
        Debug.Log($"{itemName} wird benutzt!");
        
        // Erhöhe Intuition um 30% (oder 100% für volle Intuition)
        if (IntuitionSystem.Instance != null)
        {
            IntuitionSystem.Instance.IncreaseIntuition(30); // 30% Bonus
            
            // Visuelles Feedback
            if (VisualFeedbackManager.Instance != null && Camera.main != null)
            {
                Vector3 feedbackPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;
                VisualFeedbackManager.Instance.PlayItemUseEffect(feedbackPos);
                VisualFeedbackManager.Instance.FlashScreen(Color.cyan, 0.3f, 0.2f);
            }
        }
        else
        {
            Debug.LogError("IntuitionSystem.Instance nicht gefunden!");
        }
    }
}

