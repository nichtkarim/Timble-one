using System;
using UnityEngine;

public class heavybreathing : MonoBehaviour
{
    [SerializeField] GameObject targetObject;
    [SerializeField] private float expandDuration = 1.0f;
    [SerializeField] Vector3 breatheIn;
    [SerializeField] Vector3 breatheOut;
    [SerializeField] bool pulsing = false;
    private float currentTime = 0.0f;
    private bool breathingIn = true;

    void Update()
    {
        Pulse();
    }
    
    private void Pulse()
    {
       if (pulsing)
        {
    
        Vector3 targetScale = breathingIn ? breatheIn : breatheOut;
        Vector3 startScale = breathingIn ? breatheOut : breatheIn;

        currentTime += Time.deltaTime;
        float lerpFactor = currentTime / expandDuration;

            targetObject.transform.localScale = Vector3.Lerp(startScale, targetScale, lerpFactor);
    
        if (lerpFactor >= 1.0f)
            {
   
            breathingIn = !breathingIn;
            currentTime = 0f;
        }
    }
}

}