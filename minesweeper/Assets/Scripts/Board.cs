using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileRegistry
{
    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;
}
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; } //game may need to read the tilemap but doesnt need to change it

    //all sprites assigned to tile types in unity
    public TileRegistry defaultTiles;
    public TileRegistry lightTiles;
    public TileRegistry darkTiles;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(Cell[,] state)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                Tile tile = GetTile(cell, isDark(x, y) ? lightTiles : darkTiles);
                if (tile == null) tile = GetTile(cell, defaultTiles);
                tilemap.SetTile(cell.position, tile);
            }
        }
    }

    private Tile GetTile(Cell cell, TileRegistry registry)
    {
        if (cell.revealed)
        {
            return GetRevealedTile(cell, registry);
        }
        else if (cell.flagged)
        {
            return registry.tileFlag;
        } else
        {
            return registry.tileUnknown;
        }
    }

    private Tile GetRevealedTile(Cell cell, TileRegistry registry)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return registry.tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? registry.tileExploded : registry.tileMine;
            case Cell.Type.Number: return GetNumberTile(cell, registry);
            default: return null;
        }
    }

    private Tile GetNumberTile(Cell cell, TileRegistry registry)
    {
        switch (cell.number)
        {
            case 1: return registry.tileNum1;
            case 2: return registry.tileNum2;
            case 3: return registry.tileNum3;
            case 4: return registry.tileNum4;
            case 5: return registry.tileNum5;
            case 6: return registry.tileNum6;
            case 7: return registry.tileNum7;
            case 8: return registry.tileNum8;
            default: return null;
        }
    }

    public bool isDark(int x, int y)
    {
        return ((x % 2 == 0) != (y % 2 == 0));
    }
}
