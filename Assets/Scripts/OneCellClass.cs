using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OneCellClass : MonoBehaviour
{
    [ViewOnly] public int cellId = -1;
    [ViewOnly] public TheCellGameMgr.CellTypes cellType = TheCellGameMgr.CellTypes.Undefined;
    [ViewOnly] public TheCellGameMgr.CellSubTypes cellSubType = TheCellGameMgr.CellSubTypes.Empty;
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

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material.color = GetColorByType();
        transform.localScale = new Vector3(80.0f, 80.0f, 80.0f);
    }


    // called once per fixed framerate
    private void FixedUpdate()
    {

    }


    Color GetColorByType()
    {
        Color ret = Color.black;
        switch (cellType)
        {
            case TheCellGameMgr.CellTypes.Undefined:
            case TheCellGameMgr.CellTypes.Start:
                ret = Color.green;
                break;
            case TheCellGameMgr.CellTypes.Exit:
                ret = Color.cyan;
                break;
            case TheCellGameMgr.CellTypes.Safe:
                ret = Color.blue;
                break;
            case TheCellGameMgr.CellTypes.Effect:
                ret = Color.yellow;
                break;
            default:
                ret = Color.red;
                break;
        }
        ret.a = 0.5f;
        return ret;
    }


    // Draw gizmos in editor view
    protected void OnDrawGizmos()
    {
        Gizmos.color = GetColorByType();

        float size = 0.01f;
        bool wire = false;
        if (wire)
        {
            Gizmos.DrawWireCube(transform.position, transform.localScale * size);
        }
        else
        {
            //Gizmos.color = Gizmos.color * new Color(1.0f, 1.0f, 1.0f, 0.5f);
            Gizmos.DrawCube(transform.position, transform.localScale * size);
        }
    }
}
