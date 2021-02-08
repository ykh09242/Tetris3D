using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInputs : MonoBehaviour
{
    public static ButtonInputs Instance;

    GameObject activeBlock;
    TetrisBlock activeTetris;

    bool moveIsOn = true;

    void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SetActiveBlock(GameObject block, TetrisBlock tetris)
    {
        activeBlock = block;
        activeTetris = tetris;
    }

    public void MoveBlock(string direction)
    {
        if (activeBlock != null)
        {
            if (direction == "left")
            {
                activeTetris.SetInput(Vector3.left);
            }
            else if (direction == "right")
            {
                activeTetris.SetInput(Vector3.right);
            }
            else if (direction == "forward")
            {
                activeTetris.SetInput(Vector3.forward);
            }
            else if (direction == "back")
            {
                activeTetris.SetInput(Vector3.back);
            }
        }
    }

    public void RotateBlock(string rotation)
    {
        if (activeBlock != null)
        {
            //X ROTATION
            if (rotation == "posX")
            {
                activeTetris.SetRotationInput(new Vector3(90, 0, 0));
            }
            if (rotation == "negX")
            {
                activeTetris.SetRotationInput(new Vector3(-90, 0, 0));
            }
            //Y ROTATION
            if (rotation == "posY")
            {
                activeTetris.SetRotationInput(new Vector3(0, 90, 0));
            }
            if (rotation == "negY")
            {
                activeTetris.SetRotationInput(new Vector3(0, -90, 0));
            }
            //Z ROTATION
            if (rotation == "posZ")
            {
                activeTetris.SetRotationInput(new Vector3(0, 0, 90));
            }
            if (rotation == "negZ")
            {
                activeTetris.SetRotationInput(new Vector3(0, 0, -90));
            }
        }
    }

    public void SetHighSpeed()
    {
        if (activeTetris != null)
            activeTetris.SetSpeed();
    }

    public void ResetGame()
    {
        if (Playfield.Instance != null)
            Playfield.Instance.ResetPlayfield();
    }
}
