using UnityEngine;

public class Cup : MonoBehaviour, IClickable
{
    public string cupName;

    public void OnClick()
    {
        Debug.Log(cupName + " ausgewaehlt");
    }
}