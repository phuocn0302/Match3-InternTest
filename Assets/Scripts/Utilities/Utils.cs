using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Utils
{
    public static NormalItem.eNormalType GetRandomNormalType()
    {
        Array values = Enum.GetValues(typeof(NormalItem.eNormalType));
        NormalItem.eNormalType result = (NormalItem.eNormalType)values.GetValue(URandom.Range(0, values.Length));

        return result;
    }

    public static NormalItem.eNormalType GetRandomNormalTypeExcept(NormalItem.eNormalType[] types)
    {
        List<NormalItem.eNormalType> list = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<NormalItem.eNormalType>().Except(types).ToList();

        int rnd = URandom.Range(0, list.Count);
        NormalItem.eNormalType result = list[rnd];

        return result;
    }
    
    public static List<NormalItem.eNormalType> GetBalancedItemPool(int sizeX, int sizeY)
    {
        var allTypes = Enum.GetValues(typeof(NormalItem.eNormalType))
            .Cast<NormalItem.eNormalType>()
            .ToList();

        int totalSlots = sizeX * sizeY;

        List<NormalItem.eNormalType> pool = new List<NormalItem.eNormalType>();
        
        int setsPerType = (totalSlots / 3) / allTypes.Count;
        int remainderSets = (totalSlots / 3) % allTypes.Count;

        foreach (var type in allTypes)
        {
            for (int i = 0; i < setsPerType * 3; i++)
            {
                pool.Add(type);
            }
        }

        for (int i = 0; i < remainderSets; i++)
        {
            var type = allTypes[i];
            for (int k = 0; k < 3; k++) pool.Add(type);
        }
        
        pool = pool.OrderBy(x => UnityEngine.Random.value).ToList();
        return pool;
    }
}
