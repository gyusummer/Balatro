
using System;
using Random = UnityEngine.Random;

public static class Chance
{
    public static bool Roll(float denominator, Action successCallback)
    {
        float r = Random.Range(0, denominator);
        bool isSuccess = r < 1;

        if (isSuccess)
        {
            successCallback();
        }
        
        return isSuccess;
    }
}