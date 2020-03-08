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


    // Gets the singleton instance.
    public static TheCellGameMgr instance { get; private set; }


    // Gets the singleton instance.
    public static GameStates gameState { get; private set; }

    static int startingSeed = 1966;
    static float startingTime = 0; // Time since the start of a new game in sec
    public OneCellClass cellClassPrefab;
    public List<OneCellClass> allCells;


    void Awake()
    {
        Debug.Log($"[GameMgr] Awake. {gameState}");
        InitializeNewGame(startingSeed); // for debug purpose we always start with the same seed
        //InitializeNewGame(System.Environment.TickCount);
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"[GameMgr] Start. {gameState}");
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
                    allCells.Add(cell);
                }
                float x = i;
                float z = j;
                cell.transform.SetPositionAndRotation(new Vector3(-2.0f + x, 2.5f, -2.0f + z), Quaternion.identity);
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
    }


    // called once per fixed framerate
    private void FixedUpdate()
    {
        //Debug.Log($"[GameMgr][{Time.fixedTime - startingTime}]");
    }


    // Delete all cells and clear the list
    private void Cleanup()
    {
        for (int i = allCells.Count-1; i >= 0; i--)
        {
            OneCellClass cell = allCells[i];
            Destroy(cell);
        }

        allCells.Clear();
    }
}
