using UnityEngine;
using System;

public class Cup : MonoBehaviour, IClickable
{
    public string cupName;
    public bool isCorrectCup = false; 
    public void OnClick()
    {
        Debug.Log("Cup wurde geklickt: " + name);

        // Wichtig:
        MainGameLogic.Instance.OnCupSelected(this);
    }
}

/*
public class Cup : MonoBehaviour, IClickable
{

    public static event Action<Cup> OnAnyCupSelected;
    public string cupName;
    public bool isCorrectCup = false;

    public void OnClick()
    {
        Debug.Log(cupName + " ausgewaehlt");
        OnAnyCupSelected?.Invoke(this);
        if (isCorrectCup)
        {
            MainGameLogic.Instance.OnCorrectCupSelected(this);
        }
        else
        {
            MainGameLogic.Instance.OnWrongCupSelected(this);
        }
    }
}
*/