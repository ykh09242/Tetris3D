using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playfield : MonoBehaviour
{
    public static Playfield Instance;

    [Header("Size")]
    public Vector3Int gridSize;

    [Header("Blocks")]
    public GameObject[] blockList;

    [Header("GhostBlocks")]
    public GameObject[] ghostList;

    [Header("Playfield Visulas")]
    public Transform bottomPlane;
    public Transform N, S, W, E;

    [Header("Material")]
    public Material NSGrid;
    public Material WEGrid;

    public Transform[,,] theGrid;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        theGrid = new Transform[gridSize.x, gridSize.y, gridSize.z];
        SpawnNewBlock();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public Vector3Int Round(Vector3 vec)
    {
        return new Vector3Int(Mathf.RoundToInt(vec.x),
                              Mathf.RoundToInt(vec.y),
                              Mathf.RoundToInt(vec.z));
    }

    public bool CheckInsideGrid(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x &&
               pos.z >= 0 && pos.z < gridSize.z &&
               pos.y >= 0;
    }

    public void UpdateGrid(TetrisBlock block)
    {
        //DELETE POSSIBLE PARENT OBJECTS
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.z; z++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (theGrid[x, y, z] == null || theGrid[x, y, z].parent != block.transform) continue;

                    theGrid[x, y, z] = null;
                }
            }
        }

        //FILL IN ALL CHILD OBJECTS
        foreach (Transform child in block.transform)
        {
            Vector3Int pos = Round(child.position);
            if (pos.y < gridSize.y)
            {
                theGrid[pos.x, pos.y, pos.z] = child;
            }
        }
    }

    public Transform GetTransformOnGridPos(Vector3Int pos)
    {
        if (pos.y > gridSize.y - 1)
        {
            return null;
        }
        else
        {
            return theGrid[pos.x, pos.y, pos.z];
        }
    }

    public void SpawnNewBlock()
    {
        Vector3Int spawnPoint = new Vector3Int((int)(transform.position.x + gridSize.x * 0.5f),
                                               (int)(transform.position.y + gridSize.y),
                                               (int)(transform.position.z + gridSize.z * 0.5f));
        int randomIndex = Random.Range(0, blockList.Length);

        //SPAWN THE BLOCK
        GameObject newBlock = Instantiate(blockList[randomIndex], spawnPoint, Quaternion.identity);
        //GHOST
        GameObject newGhost = Instantiate(ghostList[randomIndex], spawnPoint, Quaternion.identity);
        newGhost.GetComponent<GhostBlock>().SetParent(newBlock);
    }

    public void DeleteLayer()
    {
        int layersCleared = 0;
        for (int y = gridSize.y - 1; y >= 0; y--)
        {
            //CHECK FULL LAYER
            if (CheckFullLayer(y))
            {
                layersCleared++;
                //DELETE SOME BLOCKS
                DeleteLayerAt(y);
                //MOVE ALL DOWN BY 1
                MoveAllLayerDown(y);
            }
        }
        if (layersCleared > 0)
        {
            GameManager.Instance.LayersCleared(layersCleared);
        }
    }

    private bool CheckFullLayer(int y)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.z; z++)
            {
                if (theGrid[x, y, z] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void DeleteLayerAt(int y)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.z; z++)
            {
                Destroy(theGrid[x, y, z].gameObject);
                theGrid[x, y, z] = null;
            }
        }
    }

    private void MoveAllLayerDown(int y)
    {
        for (int i = y; i < gridSize.y; i++)
        {
            MoveOneLayerDown(i);
        }
    }

    private void MoveOneLayerDown(int y)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.z; z++)
            {
                if (theGrid[x, y, z] != null)
                {
                    theGrid[x, y - 1, z] = theGrid[x, y, z];
                    theGrid[x, y, z] = null;
                    theGrid[x, y - 1, z].position += Vector3.down;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (bottomPlane != null)
        {
            //RESIZE BOTTOM PLANE
            Vector3 scaler = new Vector3(gridSize.x * 0.1f, 1f, gridSize.z * 0.1f);
            bottomPlane.localScale = scaler;

            //REPOSITION
            bottomPlane.position = new Vector3(transform.position.x + gridSize.x * 0.5f,
                                               transform.position.y,
                                               transform.position.z + gridSize.z * 0.5f);

            //RETILE MATERIAL
            bottomPlane.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSize.x, gridSize.z);
        }

        if (N != null || S != null)
        {
            if (N != null)
            {
                //RESIZE BOTTOM PLANE
                Vector3 scaler = new Vector3(gridSize.x * 0.1f, 1f, gridSize.y * 0.1f);
                N.localScale = scaler;

                //REPOSITION
                N.position = new Vector3(transform.position.x + gridSize.x * 0.5f,
                                         transform.position.y + gridSize.y * 0.5f,
                                         transform.position.z + gridSize.z);
            }

            if (S != null)
            {
                //RESIZE BOTTOM PLANE
                Vector3 scaler = new Vector3(gridSize.x * 0.1f, 1f, gridSize.y * 0.1f);
                S.localScale = scaler;

                //REPOSITION
                S.position = new Vector3(transform.position.x + gridSize.x * 0.5f,
                                         transform.position.y + gridSize.y * 0.5f,
                                         transform.position.z);
            }

            if (NSGrid != null)
            {
                //RETILE MATERIAL
                NSGrid.mainTextureScale = new Vector2(gridSize.x, gridSize.y);
            }
        }

        if (W != null || E != null)
        {
            if (W != null)
            {
                //RESIZE BOTTOM PLANE
                Vector3 scaler = new Vector3(gridSize.z * 0.1f, 1f, gridSize.y * 0.1f);
                W.localScale = scaler;

                //REPOSITION
                W.position = new Vector3(transform.position.x,
                                         transform.position.y + gridSize.y * 0.5f,
                                         transform.position.z + gridSize.z * 0.5f);
            }

            if (E != null)
            {
                //RESIZE BOTTOM PLANE
                Vector3 scaler = new Vector3(gridSize.z * 0.1f, 1f, gridSize.y * 0.1f);
                E.localScale = scaler;

                //REPOSITION
                E.position = new Vector3(transform.position.x + gridSize.x,
                                         transform.position.y + gridSize.y * 0.5f,
                                         transform.position.z + gridSize.z * 0.5f);
            }

            if (WEGrid != null)
            {
                //RETILE MATERIAL
                WEGrid.mainTextureScale = new Vector2(gridSize.z, gridSize.y);
            }
        }

    }

    public void ResetPlayfield()
    {
        SceneManager.LoadScene(0);
    }
}
