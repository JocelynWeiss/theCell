using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OneCellClass : MonoBehaviour
{
    [ViewOnly] public TheCellGameMgr.CellTypes cellType = TheCellGameMgr.CellTypes.Undefined;
    public float cellRndSource; // A random number set at init [0..1]

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Initialize the cell
    public void InitCell(TheCellGameMgr.CellTypes type, float rnd)
    {
        if (type == TheCellGameMgr.CellTypes.Undefined)
        {
            Debug.LogError($"[OneCellClass] wrong type for init!");
            return;
        }

        cellType = type;
        cellRndSource = rnd;
        gameObject.SetActive(true);
    }


    // called once per fixed framerate
    private void FixedUpdate()
    {

    }


    protected void OnDrawGizmos()
    {
        switch(cellType)
        {
            case TheCellGameMgr.CellTypes.Undefined:
            case TheCellGameMgr.CellTypes.Start:
                Gizmos.color = Color.green;
                break;
            case TheCellGameMgr.CellTypes.Exit:
                Gizmos.color = Color.cyan;
                break;
            case TheCellGameMgr.CellTypes.Safe:
                Gizmos.color = Color.blue;
                break;
            case TheCellGameMgr.CellTypes.Effect:
                Gizmos.color = Color.yellow;
                break;
            default:
                Gizmos.color = Color.red;
                break;
        }

        float size = 0.8f;
        Gizmos.DrawWireCube(transform.position, transform.localScale * size);
    }
}
