using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface FileData
{
    public string GetPath();
}

[System.Serializable]
public struct HighscoreData : FileData
{
    public int[] bestTimes;
    
    public HighscoreData(Game game)
    {
        bestTimes = game.bestTimes;
    }

    public string GetPath()
    {
        return "/highscores.txt";
    }
}