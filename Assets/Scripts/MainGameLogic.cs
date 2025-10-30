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
        StartCoroutine(NewRound());
    }

    IEnumerator NewRound()
    {
        // 1. Zufällig Cup bestimmen
        correctCup = cups[Random.Range(0, cups.Length)];

        // Ball unter Cup setzen
        ball.position = correctCup.position + Vector3.down * 0.3f; 

        // 2. Mischen
        yield return StartCoroutine(ShuffleCups());

        // Jetzt wartet das Spiel auf Klick des Spielers
    }

    IEnumerator ShuffleCups()
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
}
