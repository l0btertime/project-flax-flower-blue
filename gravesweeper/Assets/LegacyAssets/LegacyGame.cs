using UnityEngine;

public class LegacyGame : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;

    private LegacyBoard board;
    private LegacyCell[,] state;
    private bool gameover;

    //keeps the mine amount between 0 and the total area of the board
    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    private void Awake()
    {
        board = GetComponentInChildren<LegacyBoard>();
    }

    private void Start()
    {
        NewGame();
    }

    //state is set to an array of the specific width and height (can be set in unity)
    //cells, mines, and numbers are generated
    //the camera is set to the center of the board
    //the board is drawn
    private void NewGame()
    {
        state = new LegacyCell[width, height];
        gameover = false;

        GenerateLegacyCells();
        GenerateMines();
        GenerateNumbers();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);
    }

    //generates cells with positions at specified x and y coords
    //initializes all created cells to empty
    private void GenerateLegacyCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                LegacyCell cell = new LegacyCell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = LegacyCell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }

    //generates mines at randomized positions
    //ensures mines are never placed in the same cells or off the board
    private void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++)
        { 
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while (state[x, y].type == LegacyCell.Type.Mine)
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = 0;
                    }
                }
            }

            state[x, y].type = LegacyCell.Type.Mine;
        }
    }

    //generates number cells
    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                LegacyCell cell = state[x, y];

                if (cell.type == LegacyCell.Type.Mine)
                {
                    continue;
                }

                cell.number = CountMines(x, y);
                
                if (cell.number > 0)
                {
                    cell.type = LegacyCell.Type.Number;
                }

                state[x, y] = cell;
            }
        }
    }

    //counts how many mines surround a specific cell
    private int CountMines(int cellX, int cellY)
    {
        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = cellX + adjacentX;
                int y = cellY + adjacentY;

                if (GetLegacyCell(x, y).type == LegacyCell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    //restarts the game when the R key is pressed
    //flags a cell on right click
    //reveals a cell on left click
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
        else if (!gameover)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
        }
    }

    //flags a cell
    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        LegacyCell cell = GetLegacyCell(cellPosition.x, cellPosition.y);

        if (cell.type == LegacyCell.Type.Invalid || cell.revealed)
        {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
    }

    //reveals a cell
    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        LegacyCell cell = GetLegacyCell(cellPosition.x, cellPosition.y);

        if (cell.type == LegacyCell.Type.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }

        switch (cell.type)
        {
            case LegacyCell.Type.Mine:
                Explode(cell);
                break;

            case LegacyCell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;

            default:
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }
        
        board.Draw(state);
    }

    //used to reveal all adjacent empty spaces (flood the area) to an empty space
    private void Flood(LegacyCell cell)
    {
        if (cell.revealed) return;
        if (cell.type == LegacyCell.Type.Mine || cell.type == LegacyCell.Type.Invalid) return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        if (cell.type == LegacyCell.Type.Empty)
        {
            Flood(GetLegacyCell(cell.position.x - 1, cell.position.y));
            Flood(GetLegacyCell(cell.position.x + 1, cell.position.y));
            Flood(GetLegacyCell(cell.position.x, cell.position.y - 1));
            Flood(GetLegacyCell(cell.position.x, cell.position.y + 1));
        }
    }

    //if a mine is clicked then the mine explodes, showing all other mines on the board
    //the user loses and the game ends
    private void Explode(LegacyCell cell)
    {
        Debug.Log("Game Over!");
        gameover = true;

        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x, cell.position.y] = cell;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if (cell.type == LegacyCell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    //if all non-mine cells have been revealed then the user wins and the game ends
    //flags all mines on the board
    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            { 
                LegacyCell cell = state[x, y];
                
                if (cell.type != LegacyCell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }

        Debug.Log("Winner!");
        gameover = true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                LegacyCell cell = state[x, y];

                if (cell.type == LegacyCell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    //returns the cell clicked on if it's valid
    //returns a new (and thus invalid) cell if not
    private LegacyCell GetLegacyCell(int x, int y)
    {
        if (IsValid(x, y))
        {
            return state[x, y];
        } else
        {
            return new LegacyCell();
        }
    }

    //checks that the cell is within the board
    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
