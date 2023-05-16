using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

struct LowestEntropyInfo
{
    public Cell cell;
    public int gridX;
    public int gridY;

    public LowestEntropyInfo(Cell cell, int x, int y)
    {
        this.cell = cell;
        this.gridX = x;
        this.gridY = y;
    }
}


public class GridCreator : MonoBehaviour
{

    [SerializeField] GameObject Block0;

    [SerializeField] GameObject Block1;
    [SerializeField] GameObject Block11;
    [SerializeField] GameObject Block111;
    [SerializeField] GameObject Block1111;


    [SerializeField] GameObject Block2;
    [SerializeField] GameObject Block22;
    [SerializeField] GameObject Block222;
    [SerializeField] GameObject Block2222;


    [SerializeField] GameObject Block3;
    [SerializeField] GameObject Block33;


    [SerializeField] GameObject Block4;
    [SerializeField] GameObject Block44;
    [SerializeField] GameObject Block444;
    [SerializeField] GameObject Block4444;


    [SerializeField] GameObject Block5;
    [SerializeField] GameObject Block55;
    [SerializeField] GameObject Block555;
    [SerializeField] GameObject Block5555;

    [SerializeField] GameObject Block6;
    [SerializeField] GameObject Block66;
    [SerializeField] GameObject Block666;
    [SerializeField] GameObject Block6666;

    [SerializeField] GameObject Stair1;
    [SerializeField] GameObject Stair11;
    [SerializeField] GameObject Stair111;
    [SerializeField] GameObject Stair1111;

    int ROW = 30;
    int COL = 30;

    // this will contain all our tiles and their rules.
    List<Tile> tiles = new List<Tile>();

    List<GameObject> tilesGO;

    // this represents our main grid which holds the information of each Cell, whether it is collapsed or not, and what options are available for that tile.
    List<List<Cell>> grid = new List<List<Cell>>();

    // the gameObjects that we need to instantiate when we make our grid
    List<List<GameObject>> gridGO = new List<List<GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        /*tiles.Add(new Tile(Block0, new List<int>() { 0, 0, 0, 0})); 

        tiles.Add(new Tile(Block1, new List<int>() { 1, 1, 0, 1})); 
        tiles.Add(new Tile(Block11, new List<int>() { 1, 1, 1, 0})); 
        tiles.Add(new Tile(Block111, new List<int>() { 0, 1, 1, 1})); 
        tiles.Add(new Tile(Block1111, new List<int>() { 1, 0, 1, 1})); 


        tiles.Add(new Tile(Block2, new List<int>() { 0, 0, 1, 0})); 
        tiles.Add(new Tile(Block22, new List<int>() { 0, 0, 0, 1})); 
        tiles.Add(new Tile(Block222, new List<int>() { 1, 0, 0, 0})); 
        tiles.Add(new Tile(Block2222, new List<int>() { 0, 1, 0, 0})); 


        tiles.Add(new Tile(Block3, new List<int>() { 0, 1, 0, 1})); 
        tiles.Add(new Tile(Block3, new List<int>() { 1, 0, 1, 0})); 


        tiles.Add(new Tile(Block4, new List<int>() { 0, 2, 1, 1})); 
        tiles.Add(new Tile(Block44, new List<int>() { 0, 2, 1, 1})); 
        tiles.Add(new Tile(Block444, new List<int>() { 0, 2, 1, 1})); 
        tiles.Add(new Tile(Block4444, new List<int>() { 0, 2, 1, 1})); 


        tiles.Add(new Tile(Block5, new List<int>() { 0, 0, 1, 1}));
        tiles.Add(new Tile(Block55, new List<int>() { 0, 0, 1, 1}));
        tiles.Add(new Tile(Block555, new List<int>() { 0, 0, 1, 1}));
        tiles.Add(new Tile(Block5555, new List<int>() { 0, 0, 1, 1}));
        */

        tiles.Add(new Tile(Block0, new List<string>() { "aaa", "aaa", "aaa", "aaa" }));

        tiles.Add(new Tile(Block1, new List<string>() { "aaa", "aab", "ccc", "baa" }));
        tiles.Add(new Tile(Block11, new List<string>() { "baa", "aaa", "aab", "ccc" }));
        tiles.Add(new Tile(Block111, new List<string>() { "ccc", "baa", "aaa", "aab" }));
        tiles.Add(new Tile(Block1111, new List<string>() { "aab", "ccc", "baa", "aaa" }));

        tiles.Add(new Tile(Block2, new List<string>() { "ccc", "ccc", "bab", "ccc" }));
        tiles.Add(new Tile(Block22, new List<string>() { "ccc", "ccc", "ccc", "bab" }));
        tiles.Add(new Tile(Block222, new List<string>() { "bab", "ccc", "ccc", "ccc" }));
        tiles.Add(new Tile(Block2222, new List<string>() { "ccc", "bab", "ccc", "ccc" }));

        tiles.Add(new Tile(Block3, new List<string>() { "ccc", "bab", "ccc", "bab"}));
        tiles.Add(new Tile(Block33, new List<string>() { "bab", "ccc", "bab", "ccc"}));

        tiles.Add(new Tile(Block4, new List<string>() { "aab", "bab", "baa", "aaa"}));
        tiles.Add(new Tile(Block44, new List<string>() { "aaa", "aab", "bab", "baa"}));
        tiles.Add(new Tile(Block444, new List<string>() { "baa", "aaa", "aab", "bab"}));
        tiles.Add(new Tile(Block4444, new List<string>() { "bab", "baa", "aaa", "aab"}));

        tiles.Add(new Tile(Block5, new List<string>() { "ccc", "ccc", "baa", "aab"}));
        tiles.Add(new Tile(Block55, new List<string>() { "aab", "ccc", "ccc", "baa"}));
        tiles.Add(new Tile(Block555, new List<string>() { "baa", "aab", "ccc", "ccc"}));
        tiles.Add(new Tile(Block5555, new List<string>() { "ccc", "baa", "aab", "ccc"}));
        
        /*
        tiles.Add(new Tile(Block6, new List<string>() { "ccc", "aaa", "ccc", "bab"}));
        tiles.Add(new Tile(Block66, new List<string>() { "bab", "ccc", "aaa", "ccc"}));
        tiles.Add(new Tile(Block666, new List<string>() { "ccc", "bab", "ccc", "aaa"}));
        tiles.Add(new Tile(Block6666, new List<string>() { "aaa", "ccc", "bab", "ccc"}));
        */

        tilesGO = new List<GameObject>() { Block0, Block1, Block11, Block111, Block1111, Block2, Block22, Block222, Block2222, Block3, Block33, Block4, Block44, Block444, Block4444, Block5, Block55, Block555, Block5555 };

        // this will generate all the rules for the tiles.
        Tile.GenerateTileRules(tiles);

        GenerateGrid();

        // initialize an empty ROW X COL grid
        for (int i = 0; i < ROW; i++)
        {
            List<Cell> row = new List<Cell>();
            for (int j = 0; j < COL; j++)
            {
                row.Add(new Cell());
            }
            grid.Add(row);
        }

        // reduce a random cell and limit its options
        grid[10][10].options = new List<int>() { 0};
        /*grid[3][3].options = new List<int>() { 0 };
        grid[20][10].options = new List<int>() { 0 };
        grid[15][5].options = new List<int>() { 0 };*/
    }

    // Update is called once per frame
    void Update()
    {
        int lowestEntropy = FindCellWithLowestEntropy();

        if (lowestEntropy < 0 || lowestEntropy > 30)
        {
            return;
        }

        // find all such cells that have the lowest entropy
        List<LowestEntropyInfo> lowestEntropyCells = FindAllCellWithLowestEntropy(lowestEntropy);

        ChooseRandomCellAndCollapse(lowestEntropyCells);
    }

    private void ChooseRandomCellAndCollapse(List<LowestEntropyInfo> allLowestEntropy)
    {
        int randomIndex = UnityEngine.Random.Range(0, allLowestEntropy.Count);

        // choose a random cell from the cells of lowest entropy
        LowestEntropyInfo lowestEntropy = allLowestEntropy[randomIndex];

        // collapse that cell
        lowestEntropy.cell.collapsed = true;

        // since that cell is collapsed it only has one option
        if (lowestEntropy.cell.options.Count == 0) return;
        //Debug.Log(lowestEntropy.cell.options.Count);

        lowestEntropy.cell.options = new List<int>() { lowestEntropy.cell.options[UnityEngine.Random.Range(0, lowestEntropy.cell.options.Count)] };

        Destroy(gridGO[lowestEntropy.gridX][lowestEntropy.gridY]);

        GameObject tileGameObject = Instantiate(tilesGO[lowestEntropy.cell.options[0]], new Vector3(2 * lowestEntropy.gridY, 0, -2 * lowestEntropy.gridX), Quaternion.identity);
        tileGameObject.transform.SetParent(transform);
        gridGO[lowestEntropy.gridX][lowestEntropy.gridY] = tileGameObject;



        // now decrease the entropy of the nearby surrounding it.
        int randomRow = lowestEntropy.gridX;
        int randomCol = lowestEntropy.gridY;

        // decrease the entropy of the cells above the chosen cell
        if (randomRow > 0)
        {
            Cell cellAbove = grid[randomRow - 1][randomCol];

            if (!cellAbove.collapsed)
            {
                List<int> cellOptions = cellAbove.options;
                List<int> constraintOptions = tiles[lowestEntropy.cell.options[0]].up;
                cellAbove.options = Tile.ReturnValidOptions(cellOptions, constraintOptions);

                if (cellAbove.options.Count == 0)
                {
                    GameObject noOptionTile = Instantiate(tiles[0].tile, new Vector3(2 * randomCol, 0, -2 * (randomRow - 1)), Quaternion.identity);
                    cellAbove.collapsed = true;
                }

            }
        }

        if (randomRow < ROW - 1)
        {
            Cell cellDown = grid[randomRow + 1][randomCol];


            if (!cellDown.collapsed)
            {
                List<int> cellOptions = cellDown.options;
                List<int> constraintOptions = tiles[lowestEntropy.cell.options[0]].down;

                cellDown.options = Tile.ReturnValidOptions(cellOptions, constraintOptions);

                if (cellDown.options.Count == 0)
                {
                    GameObject noOptionTile = Instantiate(tiles[0].tile, new Vector3(2 * randomCol, 0, -2 * (randomRow + 1)), Quaternion.identity);
                    cellDown.collapsed = true;
                }
            }
        }
        if (randomCol > 0)
        {
            Cell cellLeft = grid[randomRow][randomCol - 1];

            if (!cellLeft.collapsed)
            {
                List<int> cellOptions = cellLeft.options;
                List<int> constraintOptions = tiles[lowestEntropy.cell.options[0]].left;

                cellLeft.options = Tile.ReturnValidOptions(cellOptions, constraintOptions);

                if (cellLeft.options.Count == 0)
                {
                    GameObject noOptionTile = Instantiate(tiles[0].tile, new Vector3(2 * (randomCol - 1), 0, -2 * (randomRow)), Quaternion.identity);
                    cellLeft.collapsed = true;
                }
            }
        }
        if (randomCol < COL - 1)
        {
            Cell cellRight = grid[randomRow][randomCol + 1];

            if (!cellRight.collapsed)
            {
                List<int> cellOptions = cellRight.options;
                List<int> constraintOptions = tiles[lowestEntropy.cell.options[0]].right;

                cellRight.options = Tile.ReturnValidOptions(cellOptions, constraintOptions);

                if (cellRight.options.Count == 0)
                {
                    GameObject noOptionTile = Instantiate(tiles[0].tile, new Vector3(2 * (randomCol + 1), 0, -2 * (randomRow)), Quaternion.identity);
                    cellRight.collapsed = true;
                }
            }
        }
    }


    private List<LowestEntropyInfo> FindAllCellWithLowestEntropy(int entropy)
    {
        List<LowestEntropyInfo> lowestEntropyCells = new List<LowestEntropyInfo>();

        for (int i = 0; i < ROW; i++)
        {
            for (int j = 0; j < COL; j++)
            {
                Cell cell = grid[i][j];

                if (!cell.collapsed)
                {
                    if (cell.options.Count == entropy)
                    {
                        lowestEntropyCells.Add(new LowestEntropyInfo(cell, i, j));
                    }
                }
            }
        }
        return lowestEntropyCells;
    }

    private int FindCellWithLowestEntropy()
    {
        float lowestEntropy = Mathf.Infinity;

        for (int i = 0; i < ROW; i++)
        {
            for (int j = 0; j < COL; j++)
            {
                Cell cell = grid[i][j];

                if (!cell.collapsed)
                {
                    if (cell.options.Count < lowestEntropy)
                    {
                        lowestEntropy = cell.options.Count;
                    }
                }
            }
        }

        return (int)lowestEntropy;
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < ROW; i++)
        {
            List<GameObject> gridGORow = new List<GameObject>();
            for (int j = 0; j < COL; j++)
            {
                //GameObject tile = null;

                GameObject tile = Instantiate(Block0, new Vector3(j, 0, -i), Quaternion.identity);
                tile.SetActive(false);
                tile.transform.SetParent(transform);

                gridGORow.Add(tile);
            }
            gridGO.Add(gridGORow);
        }
    }
}
