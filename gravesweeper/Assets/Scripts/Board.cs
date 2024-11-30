using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class SpriteRegistry
{
    public Sprite tileUnknown;
    public Sprite tileEmpty;
    public Sprite tileMine;
    public Sprite tileExploded;
    public Sprite tileFlag;
    public Sprite tileNum1;
    public Sprite tileNum2;
    public Sprite tileNum3;
    public Sprite tileNum4;
    public Sprite tileNum5;
    public Sprite tileNum6;
    public Sprite tileNum7;
    public Sprite tileNum8;
}
public class Board : MonoBehaviour
{
    //all sprites assigned to tile types in unity
    public SpriteRegistry defaultTiles;
    public SpriteRegistry lightTiles;
    public SpriteRegistry darkTiles;
    public float cellSize = 1; // the scale of each tile
    private float offset = 0;
    public GameObject tileObject;
    public GameObject[,] tiles;
    

    public void Draw(Cell[,] state)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                Sprite tile = GetTile(cell, isDark(x, y) ? lightTiles : darkTiles);
                if (tile == null) tile = GetTile(cell, defaultTiles);
                bool underground = Underground(tile);
                SetTile((Vector2Int) cell.position, tile, underground);
            }
        }
    }

    private bool Underground(Sprite tile)
    {
        bool underground = true;
        SpriteRegistry registry = darkTiles;
        if (tile == registry.tileUnknown || tile == registry.tileFlag) underground = false;
        registry = lightTiles;
        if (tile == registry.tileUnknown || tile == registry.tileFlag) underground = false;
        return underground;
    }

    private Sprite GetTile(Cell cell, SpriteRegistry registry)
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

    private Sprite GetRevealedTile(Cell cell, SpriteRegistry registry)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return registry.tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? registry.tileExploded : registry.tileMine;
            case Cell.Type.Number: return GetNumberTile(cell, registry);
            default: return null;
        }
    }

    private Sprite GetNumberTile(Cell cell, SpriteRegistry registry)
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
        worldPos = worldPos - new Vector2(offset, offset - 1f);
        Vector2Int cellPos = new Vector2Int((int)(worldPos.x / cellSize), 
                                            (int)(worldPos.y / cellSize));
        return cellPos;
    }
    public Vector2 CellToWorld(Vector2Int cellPos)
    {
        Vector2 worldPos = (cellSize * (Vector2)cellPos) + new Vector2(offset, offset - 1f);
        return worldPos;
    }

    public Vector2 CellToWorldCentered(Vector2Int cellPos)
    {
        Vector2 worldPos = (cellSize * (Vector2)cellPos) + new Vector2(offset, offset - 1f) + 0.5f * new Vector2(cellSize, cellSize);
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
        offset = 0.5f * (size - defaultSize);
        cellSize = scale;
    }

    public void ClearBoard()
    {
        foreach(Transform child in this.transform) Destroy(child.gameObject);
    }

    public void GenerateBoard(int width, int height)
    {
        ClearBoard();
        tiles = new GameObject[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject newTile = Instantiate(tileObject, CellToWorld(new Vector2Int(x,y)), tileObject.transform.rotation);
                newTile.transform.SetParent(this.transform);
                if (isDark(x, y)) SetTile(newTile, darkTiles.tileUnknown, false); 
                else SetTile(newTile, lightTiles.tileUnknown, false);
                newTile.transform.SetParent(this.transform);
                newTile.transform.localScale = cellSize * new Vector3(1, 1, 1);
                newTile.GetComponent<SpriteRenderer>().sortingOrder = (height - y) + (width - x);
                tiles[x, y] = newTile;
            }
        }
    }

}

