using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoaderBehavior : MonoBehaviour
{
    public Game game;
    private void Awake()
    {
        HighscoreData hsData = new HighscoreData();
        int[] array = new int[3];
        array[0] = -1;
        array[1] = -1;
        array[2] = -1;
        hsData.bestTimes = array;
        FileData defaultData = hsData;
        HighscoreData data = (HighscoreData) SaveSystem.LoadData(new HighscoreData().GetPath(), defaultData);
        game.bestTimes = data.bestTimes;
    }

    public void SaveData()
    {
        FileData data = new HighscoreData(game);
        SaveSystem.SaveData(data);
    }
}
