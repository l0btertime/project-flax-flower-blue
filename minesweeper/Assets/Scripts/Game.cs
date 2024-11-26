using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class Game : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;
    public int time;
    public bool firstClick = true;
    public GameObject menu;
    public ScreenShake screenShake;
    public TextMeshProUGUI timerText;

    private Board board;
    private Cell[,] state;
    private bool gameover;

    public int PPU;
    public int padding;

    public GameObject lightParticles;
    public GameObject darkParticles;
    

    //keeps the mine amount between 0 and the total area of the board
    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void FixSize()
    {
        float defaultSize = 16;//- padding;
        float size = width;//- padding;
        float scale = defaultSize / size;
        transform.GetChild(0).localScale = new Vector3(1, 1, 1) * scale;
        float offset = size - defaultSize;
        transform.GetChild(0).localPosition = new Vector3(1, 1, 0) * offset / 2f + new Vector3(0, -1f, 0);
    }
    private void Start()
    {
        /*
        int size = Random.Range(1, 50);
        width = size;
        height = size;
        mineCount = (int) ((float)size * (float)size * 0.3f);
        */
        //FixSize();
        board.FixSize(Mathf.Max(width, height));
        board.GenerateBoard(width, height);
        NewGame();

    }

    //state is set to an array of the specific width and height (can be set in unity)
    //cells, mines, and numbers are generated
    //the camera is set to the center of the board
    //the board is drawn
    public void NewGame()
    {
        state = new Cell[width, height];
        gameover = false;

        GenerateCells();
        //GenerateMines(); !!!
        //GenerateNumbers(); !!!
        
        

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);

        StopAllCoroutines();
        StartCoroutine(Timer());

        firstClick = true;
    }

    private IEnumerator Timer()
    {
        time = 0;
        int currentTime = time;
        timerText.text = time.ToString();

        while (time == currentTime)
        {
            yield return new WaitForSeconds(1);
            time++;
            currentTime = time;
            timerText.text = time.ToString();
        }
    }

    //generates cells with positions at specified x and y coords
    //initializes all created cells to empty
    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
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
            int x = UnityEngine.Random.Range(0, width);
            int y = UnityEngine.Random.Range(0, height);

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = (Vector3Int)board.WorldToCell(worldPosition);

            while (state[x, y].type == Cell.Type.Mine)
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
            if ((x == cellPosition.x &&  y == cellPosition.y))
            {
                i--;
            }
            else if((x == (cellPosition.x + 1) && y == cellPosition.y))
            {
                i--;

            }
            else if((x == (cellPosition.x - 1) && y == cellPosition.y))
            {
                i--;

            }
            else if((x == cellPosition.x && y == (cellPosition.y + 1)))
            {
                i--;

            }
            else if((x == cellPosition.x && y == (cellPosition.y - 1)))
            {
                i--;
            }
            else if ((x == cellPosition.x+1 && y == (cellPosition.y + 1)))
            {
                i--;
            }
            else if ((x == cellPosition.x + 1 && y == (cellPosition.y - 1)))
            {
                i--;
            }
            else if ((x == cellPosition.x - 1 && y == (cellPosition.y + 1)))
            {
                i--;
            }
            else if ((x == cellPosition.x - 1 && y == (cellPosition.y - 1)))
            {
                i--;
            }
            else
            {
                state[x, y].type = Cell.Type.Mine;
            }

        }
    }

    //generates number cells
    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    continue;
                }

                cell.number = CountMines(x, y);
                
                if (cell.number > 0)
                {
                    cell.type = Cell.Type.Number;
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

                if (GetCell(x, y).type == Cell.Type.Mine)
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
                if(firstClick == true)
                {
                    firstClick = false;
                    GenerateMines();
                    GenerateNumbers();
                    Reveal();
                }
                else
                {
                    Reveal();
                }
            }
        }
    }

    //flags a cell
    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = (Vector3Int) board.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
       
        if (cell.type == Cell.Type.Invalid || cell.revealed)
        {
            return;
        }
        if (cell.flagged) AudioManager.Play("Flag"); else AudioManager.Play("FlagDown");
        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
    }

    //reveals a cell
    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = (Vector3Int) board.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }

        switch (cell.type)
        {
            case Cell.Type.Mine:
                AudioManager.Play("Explode");
                screenShake.Shake(0.4f, 1.2f);
                Explode(cell);
                break;

            case Cell.Type.Empty:
                AudioManager.Play("Flood");
                screenShake.Shake(0.16f, 0.3f);
                Flood(cell);
                CheckWinCondition();
                //GameObject particle = Instantiate(board.isDark(cellPosition.x, cellPosition.y) ? lightParticles : darkParticles, board.CellToWorld((Vector2Int) cellPosition), lightParticles.transform.rotation);
                //particle.GetComponent<ParticleSystem>().startSize = board.cellSize;
                break;

            default:
                AudioManager.Play("Dig");
                //screenShake.Shake(0.1f, 0.04f);
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                GameObject particle2 = Instantiate(board.isDark(cellPosition.x, cellPosition.y) ? lightParticles : darkParticles, board.CellToWorld((Vector2Int) cellPosition), lightParticles.transform.rotation);
                particle2.GetComponent<ParticleSystem>().startSize = board.cellSize;
                CheckWinCondition();
                break;
        }
        
        board.Draw(state);
    }

    //used to reveal all adjacent empty spaces (flood the area) to an empty space
    private void Flood(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        GameObject particle = Instantiate(board.isDark(cell.position.x, cell.position.y) ? lightParticles : darkParticles, board.CellToWorld((Vector2Int)cell.position), lightParticles.transform.rotation);
        particle.GetComponent<ParticleSystem>().startSize = board.cellSize;

        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));

            // flood diagonally adjacent squares
            Flood(GetCell(cell.position.x - 1, cell.position.y - 1));
            Flood(GetCell(cell.position.x - 1, cell.position.y + 1));
            Flood(GetCell(cell.position.x + 1, cell.position.y - 1));
            Flood(GetCell(cell.position.x + 1, cell.position.y + 1));
        }
    }

    //if a mine is clicked then the mine explodes, showing all other mines on the board
    //the user loses and the game ends
    private void Explode(Cell cell)
    {
        menu.SetActive(true);
        menu.GetComponentInChildren<TextMeshProUGUI>().text = "You Lose.";
        gameover = true;

        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x, cell.position.y] = cell;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
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
                Cell cell = state[x, y];
                
                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }

        AudioManager.Play("Win");
        menu.SetActive(true);
        menu.GetComponentInChildren<TextMeshProUGUI>().text = "You Win!";
        gameover = true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    //returns the cell clicked on if it's valid
    //returns a new (and thus invalid) cell if not
    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y))
        {
            return state[x, y];
        } else
        {
            return new Cell();
        }
    }

    //checks that the cell is within the board
    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
