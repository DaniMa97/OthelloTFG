using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public struct Square
    {
        public int x, y;

        public Square(int a, int b)
        {
            x = a;
            y = b;
        }
    }

    public static byte[,] ComputeMovesBoard(byte[,] board)
    {
        byte[,] moveBoard = new byte[board.GetLength(0), board.GetLength(1)];

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                //If square is occupied, no player can move there
                if (board[i, j] != 0)
                    moveBoard[i, j] = 0;
                else
                {
                    //Check neighbourhood of square
                    for (int x = -1; x <= 1 && moveBoard[i, j] != 3; x++)
                    {
                        for (int y = -1; y <= 1 && moveBoard[i, j] != 3; y++)
                        {
                            int posX = i + x, posY = j + y;
                            int counter = 1;
                            //Neighbour square is inside the board, is not the same square and its owned by a player
                            if (posX >= 0 && posX < board.GetLength(0) && posY >= 0 && posY < board.GetLength(1) && !(x == 0 && y == 0)
                                && board[posX, posY] != 0)
                            {
                                //Player that owns neighbour. Opponent can move if the move is valid
                                byte p1 = board[posX, posY];
                                bool notCalculated = true;
                                //Search in that direction for a square owned by the other player
                                while (posX + counter * x >= 0 && posX + counter * x < board.GetLength(0)
                                    && posY + counter * y >= 0 && posY + counter * y < board.GetLength(1) && notCalculated)
                                {
                                    //Empty square, move not valid
                                    if (board[posX + counter * x, posY + counter * y] == 0)
                                    {
                                        notCalculated = false;
                                    }
                                    //Square owned by the other player, move valid
                                    else if (board[posX + counter * x, posY + counter * y] != p1)
                                    {
                                        if (moveBoard[i, j] == 0)
                                        {
                                            if (p1 == 1)
                                                moveBoard[i, j] = 2;
                                            else if (p1 == 2)
                                                moveBoard[i, j] = 1;
                                        }
                                        else if (moveBoard[i, j] == p1)
                                            moveBoard[i, j] = 3;
                                        notCalculated = false;
                                    }
                                    counter++;
                                }
                            }
                        }
                    }
                }
            }
        }

        //Debug message
        /*string debugToPrint = "";
        for (int i = 0; i < moveBoard.GetLength(0); i++)
        {
            for (int j = 0; j < moveBoard.GetLength(1); j++)
            {
                debugToPrint += moveBoard[i, j] + ",";
            }
            debugToPrint += "\n";
        }

        //Outside Unity debug
        //Console.WriteLine(debugToPrint);

        //Inside Unity debug
        Debug.Log(debugToPrint);*/
        return moveBoard;
    }

    public const string TAG_MESSAGE = "Message";
    public const string TAG_WINNER = "Winner";
    public const string TAG_PVPVCPU = "PvPvCPU";
    public const string TAG_PLAY = "Play";
    public const string TAG_BACK = "Back";
    public const string TAG_PLAYP1P2 = "PlayP1P2";
    public const string TAG_SMOOTHER = "Smoother";
    public const string TAG_OPTS_BUTTON = "OptionsButton";
    public const string SMOOTHER_TRIGGER = "LoadScene";
    public const string AVAILABLE_NAME = "AvailablePosition";
    public const string STONE_NAME = "Stone(Clone)";
    public const string GAME_SCENE = "MainGame";
    public const string MAIN_MENU_SCENE = "MainMenu";
    public const int HORIZONTAL_BOARD = 8;
    public const int VERTICAL_BOARD = 8;
    public const int MAX_TILES = 64;
    public const int MAX_MEDIUM_DEPTH = 2;
    public const int MAX_HARD_DEPTH = 4;
    public const int NUM_PAGES_INSTRUCTIONS = 5;
    public const float SPEED = 180 * 2;
    public const float HEIGHT_STONE = 1;
    public const float TIMER_ROTATION = 0.75f;
    public const float TIMER_MESSAGE = 5;
    public const float TIMER_WAIT_TURN_EM = 1f;
    public const float TIMER_WAIT_TURN_H = 0.5f;
    public const float TIMER_WAIT_TEXT = 0.5f;
    public const float FADE_TIME = 0.6f;
}
