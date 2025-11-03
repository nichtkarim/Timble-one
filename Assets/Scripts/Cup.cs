using UnityEngine;

public class Cup : MonoBehaviour, IClickable
{
    public string cupName;
    public bool isCorrectCup = false;

    public void OnClick()
    {
        Debug.Log(cupName + " ausgewaehlt");
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