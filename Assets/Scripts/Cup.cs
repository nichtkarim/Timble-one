using UnityEngine;
using System;

public class Cup : MonoBehaviour, IClickable, IHoverable
{
    public string cupName;
    public bool isCorrectCup = false;
    public void OnClick()
    {
        if (!MainGameLogic.PlayerCanClick)
        {
            return;
        }
        else
        {
            Debug.Log("Cup wurde geklickt: " + name);
            MainGameLogic.Instance.OnCupSelected(this);
        }
    }
     public void OnHoverEnter()
    {
         if (!MainGameLogic.PlayerCanClick)
        {
            return;
        }
        else
        {
            if (gameObject.GetComponent<Outline>() == null)
            {
                 var outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 4f;
            }
            else
            {
                return;
            }
            
        }
     
    }

    public void OnHoverExit()
    {
        if (!MainGameLogic.PlayerCanClick)
        {
            return;
        }
        else
        {
            if (gameObject.GetComponent<Outline>() != null)
            {
                Destroy(gameObject.GetComponent<Outline>());
            }
        }
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