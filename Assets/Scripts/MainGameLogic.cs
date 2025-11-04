using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class MainGameLogic : MonoBehaviour
{
    public Transform[] cups;
    public Transform ball;

    private Transform correctCup;
    public static MainGameLogic Instance;
    public HealthSystem Player = new HealthSystem(); 
    public HealthSystem Dealer = new HealthSystem();
    private TaskCompletionSource<Cup> clickTaskSource;

    
    
    void Awake()
    {
        Instance = this;
    }


    async void Start()
    {
        Debug.Log("Willkommen zum Becher-Spiel!");
        await NewRoundAsync();
    }

    public void OnCorrectCupSelected(Cup cup)
    {
        Debug.Log(" Richtiger Cup ausgewaehlt: " + cup.name);
        Dealer.takeDamage(1);
    }

    public void OnWrongCupSelected(Cup cup)
    {
        Debug.Log(" Falscher Cup: " + cup.name);

        Player.takeDamage(1);
    }
    public void OnCupSelected(Cup cup)
{
    Debug.Log("CupSelected ausgelöst für: " + cup.name);
    clickTaskSource?.TrySetResult(cup);
}


    private async Task NewRoundAsync()
    {
        Debug.Log("Neue Runde gestartet!");
        // 1. Zufällig Cup bestimmen
        correctCup = cups[Random.Range(0, cups.Length)];
        correctCup.GetComponent<Cup>().isCorrectCup = true;
        //float newBallX = correctCup.transform.parent.position.x + correctCup.position.x;

        ball.position = new Vector3(correctCup.position.x, ball.position.y, ball.position.z);


        Debug.Log("Der ball hat koordinaten: " + ball.position);
        Debug.Log("Der korrekte Cup hat " + correctCup.position);
        //ball.SetPositionAndRotation(correctCup.position + Vector3.down * 0.3f, Quaternion.identity);       
        Debug.Log("Der Ball ist unter dem Cup: " + correctCup.name);


        await MoveAllCupsDown(cups, -0.4f, 0.7f);

        ball.parent = correctCup;
        // 2. Mischen
        await shuffleCups();
        ball.parent = null;

        Cup chosenCup = await WaitForCupClick();
        Debug.Log("Spieler hat ausegwaehlt");

        if (chosenCup == correctCup)
        {
              Debug.Log("Richtiger Cup!");
        }
        else
        {
         Debug.Log("Falscher Cup!");   
        }
        Debug.Log("Auswahl erkannt → Runde vorbei");
    }

    async Task shuffleCups()
    {
        for (int i = 0; i < 6; i++)
        {
            Transform cupA = cups[Random.Range(0, cups.Length)];
            Transform cupB = cups[Random.Range(0, cups.Length)];

            Vector3 posA = cupA.position;
            Vector3 posB = cupB.position;

            float t = 0;
            while (t < 1)
            {

                cupA.position = Vector3.Lerp(posA, posB, t);
                cupB.position = Vector3.Lerp(posB, posA, t);
                t += Time.deltaTime * 2;
                await Task.Yield();
            }
        }
    }


    public async Task MoveAllCupsDown(Transform[] cups, float amount, float duration)
    {
        foreach (Transform cup in cups)
        {
            await moveUpOrDown(cup, amount, duration);
        }
    }

    public async Task moveUpOrDown(Transform cup, float amount, float duration)
    {
        Vector3 startPos = cup.position;
        Vector3 endPos = startPos + new Vector3(0, amount, 0);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            cup.position = Vector3.Lerp(startPos, endPos, t);
            await Task.Yield();
        }
    }

 public Task<Cup> WaitForCupClick()
    {
        Debug.Log("Warte auf Becher-Auswahl...");
        clickTaskSource = new TaskCompletionSource<Cup>();
        return clickTaskSource.Task;
    }
}
