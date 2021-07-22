using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public byte[,] board;
    public byte[,] moveBoard;
    public bool isP1Turn;
    public GameObject stone;
    public int p1Pts, p2Pts;
    public int turnCounter;

    public static GameController instance;
    public GameObject messageNoMove;

    TileScript[] tileList;
    List<List<TileScript>> tileBoard;
    bool blockedClick;
    bool isAbleToMove, prevPCantMove;

    void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.instance.isPvIA && GameManager.instance.isIAP1)
        {
            blockedClick = true;
            IAController.instance.isBlocked = false;
        }
        else
        {
            IAController.instance.isBlocked = true;
            blockedClick = false;
        }

        p1Pts = 0;
        p2Pts = 0;
        turnCounter = 1;
        board = new byte[Constants.VERTICAL_BOARD, Constants.HORIZONTAL_BOARD];
        moveBoard = new byte[Constants.VERTICAL_BOARD, Constants.HORIZONTAL_BOARD];

        for(int i=0; i<Constants.VERTICAL_BOARD; i++)
        {
            for(int j=0; j<Constants.HORIZONTAL_BOARD; j++)
            {
                board[i, j] = 0;
                moveBoard[i, j] = 0;
            }
        }

        messageNoMove.SetActive(false);
        isP1Turn = true;
        tileList = FindObjectsOfType<TileScript>();
        tileBoard = new List<List<TileScript>>();
        Array.Sort(tileList,
            delegate (TileScript x, TileScript y) {
            return y.transform.position.z.CompareTo(x.transform.position.z);
        });
        for(int i=0; i<Constants.VERTICAL_BOARD; i++)
        {
            tileBoard.Add(new List<TileScript>());
            for(int j=0; j<Constants.HORIZONTAL_BOARD; j++)
            {
                tileBoard[i].Add(tileList[i * Constants.HORIZONTAL_BOARD + j]);
            }
            tileBoard[i].Sort(delegate (TileScript x, TileScript y)
            {
                return x.transform.position.x.CompareTo(y.transform.position.x);
            });
        }
        for(int i=0; i<tileBoard.Count; i++)
        {
            for(int j=0; j<tileBoard[i].Count; j++)
            {
                tileBoard[i][j].indexX = i;
                tileBoard[i][j].indexY = j;
            }
        }
        InitInstantiate(tileBoard[3][4]);
        InitInstantiate(tileBoard[3][3]);
        InitInstantiate(tileBoard[4][3]);
        InitInstantiate(tileBoard[4][4]);
        moveBoard = Constants.ComputeMovesBoard(board);
        for (int i = 0; i < moveBoard.GetLength(0); i++)
        {
            for (int j = 0; j < moveBoard.GetLength(1); j++)
            {
                if ((!GameManager.instance.isPvIA || !GameManager.instance.isIAP1) && (moveBoard[i, j] == 1 || moveBoard[i, j] == 3))
                    tileBoard[i][j].transform.Find(Constants.AVAILABLE_NAME).gameObject.SetActive(true);
            }
        }
    }

    void InitInstantiate(TileScript tile)
    {
        InstantiateStone(tile, true);
        isP1Turn = !isP1Turn;
    }

    void InstantiateStone(TileScript tile, bool isInit)
    {
        float xPos = tile.transform.position.x;
        float yPos = tile.transform.position.y;
        float zPos = tile.transform.position.z;
        float xScale = tile.transform.localScale.x;
        float yScale = tile.transform.localScale.y;
        float zScale = tile.transform.localScale.z;
        GameObject stonePlaced = Instantiate(stone, new Vector3(xPos - (1 / 2 * xScale), yPos - (1 / 2 * yScale) + Constants.HEIGHT_STONE, zPos - (1 / 2 * zScale)), Quaternion.identity);
        if (isInit)
        {
            stonePlaced.GetComponent<StoneRotation>().isInit = true;
        }
        stonePlaced.transform.parent = tile.transform;
        if (isP1Turn)
        {
            stonePlaced.transform.Rotate(180, 0, 0);
            p1Pts++;
            board[tile.indexX, tile.indexY] = 1;
        }
        else
        {
            p2Pts++;
            board[tile.indexX, tile.indexY] = 2;
        }
    }

    public void Click(TileScript tile, bool clickFromPlayer)
    {
        //If it's player 1 turn and he cannot move
        if (isP1Turn && !(moveBoard[tile.indexX, tile.indexY] == 1 || moveBoard[tile.indexX, tile.indexY] == 3))
            return;
        //If it's player 2 turn and he cannot move
        else if (!isP1Turn && !(moveBoard[tile.indexX, tile.indexY] == 2 || moveBoard[tile.indexX, tile.indexY] == 3))
            return;
        //If the game is P vs IA
        else if (GameManager.instance.isPvIA)
        {
            //If the player clicked but it's IA's turn
            if(clickFromPlayer && isP1Turn == GameManager.instance.isIAP1)
            {
                return;
            }

            //If it's player turn and he is blocked
            if (isP1Turn != GameManager.instance.isIAP1 && (blockedClick || GameManager.instance.isOptionsOpen))
            {
                return;
            }
        }
        //If the game is P vs P and player is blocked
        else if (blockedClick || GameManager.instance.isOptionsOpen)
            return;

        blockedClick = true;

        DeactivateMarkers();
        InstantiateStone(tile, false);
        RotateStones(tile);
        moveBoard = Constants.ComputeMovesBoard(board);
        StartCoroutine(WaitRotation());
    }

    public void Click(Constants.Square s, bool clickFromPlayer)
    {
        Click(tileBoard[s.x][s.y], clickFromPlayer);
    }

    void DeactivateMarkers()
    {
        for (int i = 0; i < Constants.VERTICAL_BOARD; i++)
        {
            for (int j = 0; j < Constants.HORIZONTAL_BOARD; j++)
            {
                tileBoard[i][j].transform.Find(Constants.AVAILABLE_NAME).gameObject.SetActive(false);
            }
        }
    }

    IEnumerator WaitRotation()
    {
        yield return new WaitForSeconds(Constants.TIMER_ROTATION);
        if (p1Pts + p2Pts == Constants.MAX_TILES)
        {
            GameEnd();
        }
        else
        {
            ChangeTurn();
        }
    }

    void GameEnd()
    {
        WinnerScript win = FindObjectOfType<WinnerScript>();
        if (win != null)
        {
            if (p1Pts > p2Pts)
                win.Winner(1);
            else if (p2Pts > p1Pts)
                win.Winner(2);
            else
                win.Winner(0);
        }
    }

    void ChangeTurn()
    {
        //string debugM = "";

        isP1Turn = !isP1Turn;
        GetComponent<UIInGame>().ChangeTurn();
        isAbleToMove = false;
        for (int i = 0; i < moveBoard.GetLength(0); i++)
        {
            for (int j = 0; j < moveBoard.GetLength(1); j++)
            {
                //debugM += board[i, j] + ",";
                if(isP1Turn && (moveBoard[i,j] == 1 || moveBoard[i,j] == 3))
                {
                    if(!GameManager.instance.isPvIA || isP1Turn != GameManager.instance.isIAP1)
                        tileBoard[i][j].transform.Find(Constants.AVAILABLE_NAME).gameObject.SetActive(true);
                    isAbleToMove = true;
                }
                else if(!isP1Turn && (moveBoard[i, j] == 2 || moveBoard[i, j] == 3))
                {
                    if (!GameManager.instance.isPvIA || isP1Turn != GameManager.instance.isIAP1)
                        tileBoard[i][j].transform.Find(Constants.AVAILABLE_NAME).gameObject.SetActive(true);
                    isAbleToMove = true;
                }
            }
            //debugM += "\n";
        }
        //print(debugM);
        //Player can't move. Send message and change turn again
        if (!isAbleToMove)
        {
            StartCoroutine(WaitMessage());
        }
        else
        {
            prevPCantMove = false;
            turnCounter++;
            //If game is PvP, unblock user
            if (!GameManager.instance.isPvIA)
            {
                blockedClick = false;
            }
            else
            {
                //Unblock IA if it's its turn
                if(isP1Turn == GameManager.instance.isIAP1)
                {
                    IAController.instance.isBlocked = false;
                }
                //Unblock user if it's its turn
                else
                {
                    blockedClick = false;
                }
            }
        }
    }

    public IEnumerator WaitMessage()
    {
        messageNoMove.SetActive(true);
        yield return new WaitForSeconds(Constants.TIMER_MESSAGE);
        if (!prevPCantMove)
        {
            prevPCantMove = true;
            messageNoMove.SetActive(false);
            ChangeTurn();
        }
        //No player can move. Game ends
        else
        {
            messageNoMove.SetActive(false);
            GameEnd();
        }
    }

    void ChangePoint()
    {
        if (isP1Turn)
        {
            p1Pts++;
            p2Pts--;
        }
        else
        {
            p2Pts++;
            p1Pts--;
        }
    }

    void RotateStones(TileScript tile)
    {
        for(int i=-1; i<=1; i++)
        {
            for(int j=-1; j<=1; j++)
            {
                int posX = i + tile.indexX, posY = j + tile.indexY;
                int counter = 1;
                bool notCalculated = true;
                //Neighbour is inside board, is not the tile itself and is owned by another player
                if (posX >= 0 && posX < board.GetLength(0) && posY >= 0 && posY < board.GetLength(1) && !(i == 0 && j == 0)
                    && board[posX, posY] != 0 && board[posX, posY] != board[tile.indexX, tile.indexY])
                {
                    //Search in that direction for a square owned by the other player
                    while (posX + counter * i >= 0 && posX + counter * i < board.GetLength(0)
                        && posY + counter * j >= 0 && posY + counter * j < board.GetLength(1) && notCalculated)
                    {
                        //Empty square, flip not valid
                        if (board[posX + counter * i, posY + counter * j] == 0)
                        {
                            notCalculated = false;
                        }

                        //Square owned by player. Flip stones until this point
                        else if(board[posX + counter * i, posY + counter * j] == board[tile.indexX, tile.indexY])
                        {
                            counter--;
                            while(counter >= 0)
                            {
                                Transform stoneChild = tileBoard[posX + counter * i][posY + counter * j].transform.Find(Constants.STONE_NAME);
                                if (isP1Turn)
                                    board[posX + counter * i, posY + counter * j] = 1;
                                else
                                    board[posX + counter * i, posY + counter * j] = 2;
                                ChangePoint();
                                stoneChild.GetComponent<StoneRotation>().needRotation = true;
                                stoneChild.GetComponent<StoneRotation>().flipSound = true;
                                counter--;
                            }
                            notCalculated = false;
                        }
                        counter++;
                    }
                }
            }
        }
    }
}
