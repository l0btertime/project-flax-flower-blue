using UnityEngine;

public class WindowLimiter : MonoBehaviour
{
    public GameObject grid;
    public int PPU;
    public int minimumWidth;
    public int minimumHeight;

    private void Start()
    {
        minimumWidth = (int) grid.GetComponent<Grid>().cellSize.x * (int) grid.GetComponent<Game>().width * PPU;
        minimumHeight = (int) grid.GetComponent<Grid>().cellSize.y * (int) grid.GetComponent<Game>().height * PPU;
    }
    void Update()
    {
        //Screen.SetResolution(800, 800,
            //(int)Mathf.Clamp(Screen.currentResolution.width, minimumWidth, Mathf.Infinity),
            //(int)Mathf.Clamp(Screen.currentResolution.height, minimumHeight, Mathf.Infinity),
          //  false);
    }
}
