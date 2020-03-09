using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TheCellGameMgr : MonoBehaviour
{
    // Game States
    public enum GameStates
    {
        Undefined = 0,  // not initialized
        Starting,       // just started a new game with new seeds
        Running,        // in a middle of a game
        Finishing,      // just get killed, must respawn in starting cell, don't reset seeds
    }


    // Cell types
    public enum CellTypes
    {
        Undefined = 0,  // not initialized
        Start,          // first cell
        Exit,           // exit cell (winning)
        Safe,           // empty and safe
        Effect,         // a cell with a non deadly effect
        Deadly,         // a cell with a deadly effect
    }


    // Cell sub types
    public enum CellSubTypes
    {
        Fire = 0,
        Gaz,
        Water,
        Lasers,
        Illusion,
        Blind,
        Screen,
        Vortex,
        OneLook,
        Empty,
        Tunnel,
    }


    // Gets the singleton instance.
    public static TheCellGameMgr instance { get; private set; }


    // Gets the singleton instance.
    public static GameStates gameState { get; private set; }

    static int startingSeed = 1966;
    static float startingTime = 0; // Time since the start of a new game in sec
    public OneCellClass cellClassPrefab;
    public List<OneCellClass> allCells; // All the cells as they are distributed
    public List<int> lookupTab = new List<int>(25); // lookup table, hold a map of cell's id
    int playerCellId = 12; // in which place on the chess the player is. Match the lookup table.


    void Awake()
    {
        Debug.Log($"[GameMgr] Awake. {gameState}");
        transform.position = new Vector3(5.0f, 0.5f, 2.0f);
        InitializeNewGame(startingSeed); // for debug purpose we always start with the same seed
        //InitializeNewGame(System.Environment.TickCount);
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"[GameMgr] Start. {gameState}");
    }


    // Return the cell the player is in
    OneCellClass GetCurrentCell()
    {
        OneCellClass current = allCells[lookupTab[playerCellId]];
        return current;
    }


    // Start a new fresh game
    void InitializeNewGame(int gameSeed)
    {
        instance = this;
        gameState = GameStates.Starting;

        startingSeed = gameSeed;
        Random.InitState(startingSeed);

        int firstRandom = (int)(Random.value * 100.0f);
        startingTime = Time.fixedTime;
        Debug.Log($"[GameMgr][{startingTime}] new game initialized with seed {startingSeed}, first random {firstRandom}/19.");

        playerCellId = 12; // replace the player in the middle

        List<int> deadly = new List<int>(8);
        while (deadly.Count < 8)
        {
            int id = (int)(Random.value * 24.0f);
            if ((deadly.Contains(id)) || (id == 12))
            {
                continue;
            }

            deadly.Add(id);
        }
        string toto = "";
        foreach (int id in deadly)
        {
            toto += id.ToString() + " ";
        }
        Debug.Log($"[GameMgr] deadly {toto}");


        List<int> effectCells = new List<int>(6);
        while (effectCells.Count < 6)
        {
            int id = (int)(Random.value * 24.0f);
            if ((id == 12) || deadly.Contains(id) || effectCells.Contains(id))
            {
                continue;
            }

            effectCells.Add(id);
        }

        toto = "";
        foreach(int id in effectCells)
        {
            toto += id.ToString() + " ";
        }
        Debug.Log($"[GameMgr] effectCells {toto}");


        // Init a board
        bool exitChosen = false;
        int exitCount = 0;

        if (allCells == null)
        {
            allCells = new List<OneCellClass>(25);
        }

        for (int i=0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                OneCellClass cell = null;
                int id = i * 5 + j;
                if (allCells.Count == 25)
                {
                    cell = allCells[id];
                }
                else
                {
                    cell = Instantiate<OneCellClass>(cellClassPrefab);
                    cell.cellId = id;
                    allCells.Add(cell);
                    lookupTab.Add(id);
                }
                cell.name = "Cell_" + id;
                float z = i;
                float x = j;
                cell.transform.SetPositionAndRotation(new Vector3(x, 0.0f, z * -1.0f) + transform.position, Quaternion.identity);
                float aRndNb = Random.value;

                if ((i == 2) && (j == 2))
                {
                    cell.InitCell(CellTypes.Start, aRndNb);
                    continue;
                }

                // Choose an exit
                if (!exitChosen)
                {
                    if ((i == 0) || (j == 0) || (i == 4) || (j == 4) || ((i != 2) && (j != 2)))
                    {
                        exitCount++;
                        if (exitCount >= (int)(aRndNb * 20.0f))
                        {
                            cell.InitCell(CellTypes.Exit, aRndNb);
                            exitChosen = true;
                            continue;
                        }
                    }
                }

                // Choose deadly ones
                if (deadly.Contains(id))
                {
                    cell.InitCell(CellTypes.Deadly, aRndNb);
                    continue;
                }

                // Choose effect ones
                if (effectCells.Contains(id))
                {
                    cell.InitCell(CellTypes.Effect, aRndNb);
                    continue;
                }

                cell.InitCell(CellTypes.Safe, aRndNb);
            }
        }

        gameState = GameStates.Running;
    }


    // Update is called once per frame
    void Update()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log($"[GameMgr] not initialized ! {instance}.");
        }

        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            //Cleanup();
            //InitializeNewGame(0);
            InitializeNewGame(System.Environment.TickCount);
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (playerCellId > 4)
            {
                playerCellId -= 5;
            }
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (playerCellId < 20)
            {
                playerCellId += 5;
            }
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (playerCellId % 5 == 4)
            {
                return;
            }
            playerCellId++;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (playerCellId % 5 == 0)
            {
                return;
            }
            playerCellId--;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            TestGetNorth();
            MoveRow(5);
            TestGetNorth();
        }
    }


    // called once per fixed framerate
    private void FixedUpdate()
    {
        //Debug.Log($"[GameMgr][{Time.fixedTime - startingTime}]");
    }


    private void OnDestroy()
    {
        Cleanup();
        //Debug.Log($"[GameMgr][{Time.fixedTime - startingTime}] DESTROY !!!");
    }


    // Delete all cells and clear the list
    private void Cleanup()
    {
        gameState = GameStates.Finishing;

        for (int i = allCells.Count-1; i >= 0; i--)
        {
            OneCellClass cell = allCells[i];
            Destroy(cell);
        }

        allCells.Clear();
    }


    // Return the cell that is on the north wall
    public OneCellClass GetNorth(int current)
    {
        if (current > 4)
        {
            return allCells[lookupTab[current - 5]];
        }

        return null;
    }


    private void OnDrawGizmos()
    {
        if (gameState != GameStates.Running)
            return;

        OneCellClass current = GetCurrentCell();
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(current.transform.position, current.transform.localScale * 0.8f);
    }


    // Move an entire row to the right
    // JowNext: change positions
    void MoveRow(int from)
    {
        List<int> row = new List<int>(5);
        for (int i=0; i < 5; ++i)
        {
            row.Add(lookupTab[from + i]);
        }

        lookupTab[from + 0] = row[4];
        lookupTab[from + 1] = row[0];
        lookupTab[from + 2] = row[1];
        lookupTab[from + 3] = row[2];
        lookupTab[from + 4] = row[3];
    }


    void TestGetNorth()
    {
        string msg = "";
        int id = 10;
        for (int i=0; i < 5; ++i)
        {
            OneCellClass current = GetNorth(id+i);
            if (current != null)
            {
                msg += current.cellId.ToString() + " ";
            }
            else
            {
                msg += " .. ";
            }
        }
        Debug.Log($"[GameMgr] north of 10 -> {msg}");
    }
}
