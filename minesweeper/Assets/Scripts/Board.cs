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
    public float cellSize = 1; // the scale of each tile
    public GameObject tileObject;
    public GameObject[,] tiles;
    

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

    // new methods:

    // celltoworld
    // worldtocell
    // set tile
    // get tile

    // redo draw function
    // pretty sure the board needs to store all the tiles somehow
    // and we need something to handle scaling

    public Vector2Int WorldToCell(Vector2 worldPos)
    {
        Vector2Int cellPos = new Vector2Int((int)(worldPos.x / cellSize), 
                                            (int)(worldPos.x / cellSize));
        return cellPos;
    }
    public Vector2 CellToWorld(Vector2Int cellPos)
    {
        Vector2 worldPos = (Vector2) cellPos * cellSize;
        return worldPos;
    }

    public GameObject GetTileObject(Vector2Int position)
    {
        return tiles[position.x, position.y];
    }
    public void SetTile(Vector2Int position, Sprite tileSprite, bool underground)
    {
        SpriteRenderer r = GetTileObject(position).GetComponent<SpriteRenderer>();
        r.sprite = tileSprite;
        if (underground)
        {
           // r.sortingLayerID = 1;
            r.sortingLayerName = "Underground";
        }
        else
        {
           // r.sortingLayerID = 2;
            r.sortingLayerName = "Overground";
        }
    }

    public void SetTile(GameObject tile, Sprite tileSprite, bool underground)
    {
        SpriteRenderer r = tile.GetComponent<SpriteRenderer>();
        r.sprite = tileSprite;
        if (underground)
        {
           // r.sortingLayerID = 1;
            r.sortingLayerName = "Underground";
        }
        else
        {
            //r.sortingLayerID = 2;
            r.sortingLayerName = "Overground";
        }
    }
    public void FixSize(int size)
    {
        float defaultSize = 16;
        float scale = defaultSize / size;
        cellSize = scale;
    }
    public void GenerateBoard(int width, int height)
    {
        tiles = new GameObject[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject newTile = Instantiate(tileObject, cellSize * CellToWorld(new Vector2Int(x, y)), tileObject.transform.rotation);
                if (isDark(x, y)) SetTile(newTile, darkTiles.tileUnknown.sprite, false); 
                else SetTile(newTile, lightTiles.tileUnknown.sprite, false);
                newTile.transform.SetParent(this.transform);
                newTile.transform.localScale = cellSize * new Vector3(1, 1, 1);
                newTile.GetComponent<SpriteRenderer>().sortingOrder = (height - y) + (width - x);
                tiles[x, y] = newTile;
            }
        }
    }

}

