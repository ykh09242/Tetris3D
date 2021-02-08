using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBlock : MonoBehaviour
{
    GameObject parent;
    TetrisBlock parentTetris;

    public void SetParent(GameObject parent)
    {
        this.parent = parent;
        parentTetris = parent.GetComponent<TetrisBlock>();

        StartCoroutine(RepositionBlock());
    }

    private void PositionGhost()
    {
        transform.position = parent.transform.position;
        transform.rotation = parent.transform.rotation;
    }

    IEnumerator RepositionBlock()
    {
        while (parentTetris.enabled)
        {
            PositionGhost();
            //MOVE DOWNWARDS
            MoveDown();
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
        yield return null;
    }

    private void MoveDown()
    {
        while(CheckValidMove())
        {
            transform.position += Vector3.down;
        }
        if (!CheckValidMove())
        {
            transform.position += Vector3.up;
        }
    }

    private bool CheckValidMove()
    {
        foreach (Transform child in transform)
        {
            Vector3Int pos = Playfield.Instance.Round(child.position);
            if (!Playfield.Instance.CheckInsideGrid(pos))
            {
                return false;
            }
        }

        foreach (Transform child in transform)
        {
            Vector3Int pos = Playfield.Instance.Round(child.position);
            Transform t = Playfield.Instance.GetTransformOnGridPos(pos);

            if (t != null && t.parent == parent.transform)
            {
                return true;
            }

            if (t != null && t.parent != transform)
            {
                return false;
            }
        }
        return true;
    }
}
