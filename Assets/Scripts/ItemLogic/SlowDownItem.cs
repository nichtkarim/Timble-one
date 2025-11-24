using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class SlowDownItem : GameItem
{
    // itemName, icon, description werden von GameItem geerbt - NICHT hier definieren!

    // Beispiel: Item-Effekt ausführen
    
    public override void Use()
    {
      
        
        // Verlangsamung aktivieren
        if (MainGameLogic.Instance != null && MainGameLogic.Instance.Player != null)
        {
            MainGameLogic.Instance.roundBasedModifier = 0.5f;
            
            // Visuelles Feedback für Item-Nutzung
            if (VisualFeedbackManager.Instance != null && Camera.main != null)
            {
                Vector3 feedbackPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;
                VisualFeedbackManager.Instance.PlayItemUseEffect(feedbackPos);
                VisualFeedbackManager.Instance.TriggerSlowMotion(0.5f, 0.3f);
            }
        }
        else
        {
            Debug.LogError("MainGameLogic oder Player nicht gefunden!");
        }
    }
}

