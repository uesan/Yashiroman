using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNoDouble : Object
{
    private List<int> noDoubleList = new List<int>();
    private int max = 0;
    public static int Seed { get;private set; } = 0;

    public RandomNoDouble(int maxInt)
    {
        max = maxInt;
        InitnoDoubleList(maxInt);
    }

    public static void InsertSeed(int seed)
    {
        Seed = seed;
        Random.InitState(seed);
    }

    private void InitnoDoubleList(int max)
    {
        for (int i = 0; i < max; i++)
        {
            noDoubleList.Add(i);
        }
    }


    public int Next()
    {
        if(noDoubleList.Count == 0)
        {
            Debug.Log("No number exist in array");
            return -1;
        }
        int rtn, arrayNumber;
        arrayNumber = Random.Range(0, noDoubleList.Count);
        rtn = noDoubleList[arrayNumber];
        noDoubleList[arrayNumber] = noDoubleList[noDoubleList.Count - 1];
        noDoubleList.RemoveAt(noDoubleList.Count - 1);
        return rtn;
    }
}
