using UnityEngine;

public struct Cell
{
    //by default cells are created invalid
    public enum Type
    {
        Invalid,
        Empty,
        Mine,
        Number
    }

    public Vector3Int position;
    public Type type;
    public int number;
    public bool revealed;
    public bool flagged;
    public bool exploded;
}
