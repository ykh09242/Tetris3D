using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    float prevTime;
    float fallTime = 1f;

    bool gameOver = false;

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
    Vector2 initialPosition;
#endif

    private void Start()
    {
        if (ButtonInputs.Instance != null)
            ButtonInputs.Instance.SetActiveBlock(gameObject, this);
        fallTime = GameManager.Instance.ReadFallSpeed();
    }

    private void Update()
    {
        if (GameManager.Instance.ReadGameIsOver()) return;
        if (Time.time - prevTime > fallTime)
        {
            transform.position += Vector3.down;

            if (!CheckValidMove())
            {
                transform.position += Vector3.up;
                //DELETE LAYER IF POSSIBLE
                Playfield.Instance.DeleteLayer();
                enabled = false;
                //CREATE A NEW TETRIS BLOCK
                foreach (Transform child in transform)
                {
                    Vector3Int pos = Playfield.Instance.Round(child.position);
                    if (pos.y == 9)
                    {
                        gameOver = true;
                        break;
                    }
                }

                if (gameOver)
                {
                    GameManager.Instance.SetGameIsOver();
                }
                else
                {
                    Playfield.Instance.SpawnNewBlock();
                }
            }
            else
            {
                //UPDATE THE GRID
                Playfield.Instance.UpdateGrid(this);
            }

            prevTime = Time.time;
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetInput(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetInput(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetInput(Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetInput(Vector3.back);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetRotationInput(new Vector3(0, 90, 0));
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetSpeed();
        }
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                initialPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                var direction = touch.position - initialPosition;

                if (direction.x >= 100f)
                {
                    SetInput(Vector3.right);
                } else if (direction.x <= -100)
                {
                    SetInput(Vector3.left);
                }

                if (direction.y >= 100f)
                {
                    SetInput(Vector3.forward);
                }
                else if (direction.y <= -100)
                {
                    SetInput(Vector3.back);
                }
            }
        }
#endif
    }

    public void SetInput(Vector3 direction)
    {
        transform.position += direction;
        if (!CheckValidMove())
        {
            transform.position -= direction;
        }
        else
        {
            Playfield.Instance.UpdateGrid(this);
        }
    }

    public void SetRotationInput(Vector3 rotation)
    {
        transform.Rotate(rotation, Space.World);
        if (!CheckValidMove())
        {
            transform.Rotate(-rotation, Space.World);
        }
        else
        {
            Playfield.Instance.UpdateGrid(this);
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
            if (t != null && t.parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    public void SetSpeed()
    {
        fallTime = 0.05f;
    }
}
