using UnityEngine;
using System.Collections;

public class Utils 
{
    public static int RollDie(int diceCount, int numberOfFaces)
    {
        int output = 0;

        for (int i = 0; i < diceCount; i++)
        {
            output += Random.Range(0, numberOfFaces) + 1;
        }

        return output;
    }
}
