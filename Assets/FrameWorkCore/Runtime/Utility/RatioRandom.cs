using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatioRandom
{
    public List<int> listRatio = new();
    public int totalValue = 0;
    public int randomSeed;

    private List<int> listRandomIndex = new();

    public void AddRatio(int ratio)
    {
        listRatio.Add(ratio);
        totalValue += ratio;
    }

    public int RandomIndex()
    {
        if (listRatio.Count == 0)
        {
            return -1;
        }
        int randomV = 0;
        if (randomSeed != 0)
        {
            randomV = new System.Random(randomSeed).Next(0, totalValue);
        }
        else
        {
            randomV = Random.Range(0, totalValue);
        }

        int index = 0;
        while (randomV >= listRatio[index])
        {
            randomV -= listRatio[index];
            ++index;
        }

        return index;
    }

    public List<int> RandomMultipleIndex(int count, bool bRepeatable = false)
    {
        if (listRatio.Count == 0)
        {
            return listRandomIndex;
        }

        if ((listRatio.Count < count) && (!bRepeatable))
        {
            return listRandomIndex;
        }

        System.Random random = new System.Random(randomSeed);

        for (int i = 0; i < count; ++i)
        {
            int randomV = 0;
            if (randomSeed != 0)
            {
                randomV = random.Next(0, totalValue);
            }
            else
            {
                randomV = Random.Range(0, totalValue);
            }

            int index = 0;
            while (randomV >= listRatio[index])
            {
                if (listRatio[index] > 0)
                {
                    randomV -= listRatio[index];
                }

                ++index;
            }
            listRandomIndex.Add(index);

            if (!bRepeatable)
            {
                totalValue -= listRatio[index];
                listRatio[index] = -1;
            }
        }

        return listRandomIndex;
    }

    public void Clear()
    {
        listRatio.Clear();
        totalValue = 0;
        randomSeed = 0;
        listRandomIndex.Clear();
    }
}
