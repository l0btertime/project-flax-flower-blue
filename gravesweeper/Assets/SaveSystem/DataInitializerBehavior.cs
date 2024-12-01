using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DataInitializerBehavior : MonoBehaviour
{
    void Awake()
    {
        HighscoreData hsData = new HighscoreData();
        int[] array = new int[3];
        array[0] = -1;
        array[1] = -1;
        array[2] = -1;
        hsData.bestTimes = array;
        SaveSystem.LoadData((new HighscoreData()).GetPath(), hsData);
    }

}
