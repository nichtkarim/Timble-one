using UnityEngine;
using System.Collections;
using System.Linq;

public class MainGameLogic : MonoBehaviour
{
    public Transform[] cups;
    public Transform ball;

    private Transform correctCup;

    void Start()
    {
        Debug.Log("Willkommen zum Becher-Spiel!");
        StartCoroutine(NewRound());
    }

    IEnumerator NewRound()
    {
        // 1. Zufällig Cup bestimmen
        correctCup = cups[Random.Range(0, cups.Length)];
        //float newBallX = correctCup.transform.parent.position.x + correctCup.position.x;

        ball.position = new Vector3(correctCup.position.x, ball.position.y, ball.position.z);
        
        
        Debug.Log("Der ball hat koordinaten: " + ball.position);
        Debug.Log("Der korrekte Cup hat " + correctCup.position);
        //ball.SetPositionAndRotation(correctCup.position + Vector3.down * 0.3f, Quaternion.identity);       
        Debug.Log("Der Ball ist unter dem Cup: " + correctCup.name);


        // Ball unter Cup setzen
        //ball.position = correctCup.position + Vector3.down * 0.3f; 

      
        yield return new WaitForSeconds(5);

        StartCoroutine(MoveAllCupsDown(cups, -0.4f, 0.7f));
        yield return new WaitForSeconds(5); //better fix this. PProbably use aync await

        ball.parent = correctCup;
        // 2. Mischen
        yield return StartCoroutine(shuffleCupsWithBall());
        ball.parent = null;
        // Jetzt wartet das Spiel auf Klick des Spielers
    }

    IEnumerator shuffleCupsWithBall()
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
                yield return null;
            }
        }
    }

    public void CupSelected(Transform chosenCup)
    {
        if (chosenCup == correctCup)
        {
            Debug.Log("✅ Richtig!");
        }
        else
        {
            Debug.Log("❌ Falsch!");
        }

        StartCoroutine(NewRound());
    }

    public IEnumerator MoveAllCupsDown(Transform[] cups, float amount, float duration)
{
    foreach (Transform cup in cups)
    {
        yield return StartCoroutine(moveUpOrDown(cup, amount, duration));
    }
}

    public IEnumerator moveUpOrDown(Transform cup, float amount, float duration)
{
    Vector3 startPos = cup.position;
    Vector3 endPos = startPos + new Vector3(0, amount, 0);

    float t = 0;
    while (t < 1)
    {
        t += Time.deltaTime / duration;
        cup.position = Vector3.Lerp(startPos, endPos, t);
        yield return null;
    }
}

}
