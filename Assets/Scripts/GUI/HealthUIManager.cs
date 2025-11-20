using UnityEngine;
using TMPro;

public class HealthUIManager : MonoBehaviour
{
    [Header("Custom Health Bars")]
    public CustomHealthBar playerHealthBar;
    public CustomHealthBar dealerHealthBar;
    
 //   [Header("Round Info")]
 //   public TextMeshProUGUI roundText;

    void Start()
    {
        // Warte bis MainGameLogic existiert
        if (MainGameLogic.Instance != null)
        {
            SetupHealthBars();
        }
    }

    void Update()
    {
        // Falls MainGameLogic erst später geladen wird
        if (MainGameLogic.Instance != null && playerHealthBar.targetHealth == null)
        {
            SetupHealthBars();
        }
        
        // Update Round Info
      //  if (roundText != null && MainGameLogic.Instance != null)
      //  {
           // roundText.text = $"Runde {MainGameLogic.Instance.GetCurrentRound()}";
       // }
    }

    private void SetupHealthBars()
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.SetHealthSystem(MainGameLogic.Instance.Player);
        }
        
        if (dealerHealthBar != null)
       {
         dealerHealthBar.SetHealthSystem(MainGameLogic.Instance.Dealer);
       
       
      }  
    }
}