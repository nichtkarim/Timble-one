using UnityEngine;

public class IntuitionSystem
{
    int maxIntuition = 100;
    int currentIntuition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IntuitionSystem()
    {
        currentIntuition = maxIntuition;
    }
    public void  setCurrentIntuition(int damage)
    {
       
        if (currentIntuition > 0)
        {
             currentIntuition -= damage;
      
        }
    }
    public int getCurrentIntuition()
    {
        return currentIntuition;
    }
    public int getMaxIntuition()
    {
        return maxIntuition;
    }
}
