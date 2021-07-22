using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    /*EvalF eval;
    List<Constants.Square> getPossibleMoves(byte[,] moveBoard, bool isP1turn)
    {
        List<Constants.Square> moves = new List<Constants.Square>();
        int p;
        if (isP1turn)
            p = 1;
        else
            p = 2;
        //string result = "";
        for (byte i = 0; i < moveBoard.GetLength(0); i++)
        {
            for (byte j = 0; j < moveBoard.GetLength(1); j++)
            {
                if (moveBoard[i, j] == p || moveBoard[i, j] == 3)
                {
                    //result += "[" + i + "," + j + "] ";
                    moves.Add(new Constants.Square(i, j));
                }
            }
        }
        //print(result);
        return moves;
    }

    byte[,] ComputeNewBoard(byte[,] oldBoard, Constants.Square pos, bool isP1Turn)
    {
        byte[,] board = (byte[,])oldBoard.Clone();
        if (isP1Turn)
        {
            board[pos.x, pos.y] = 1;
        }
        else
        {
            board[pos.x, pos.y] = 2;
        }

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int posX = i + pos.x, posY = j + pos.y;
                int counter = 1;
                bool notCalculated = true;
                //Neighbour is inside board, is not the tile itself and is owned by another player
                if (posX >= 0 && posX < board.GetLength(0) && posY >= 0 && posY < board.GetLength(1) && !(i == 0 && j == 0)
                    && board[posX, posY] != 0 && board[posX, posY] != board[pos.x, pos.y])
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
                        else if (board[posX + counter * i, posY + counter * j] == board[pos.x, pos.y])
                        {
                            counter--;
                            while (counter >= 0)
                            {
                                if (isP1Turn)
                                    board[posX + counter * i, posY + counter * j] = 1;
                                else
                                    board[posX + counter * i, posY + counter * j] = 2;
                                counter--;
                            }
                            notCalculated = false;
                        }
                        counter++;
                    }
                }
            }
        }
        return board;
    }

    double AlphaBetaTurn(byte[,] board, byte[,] moveBoard, int depth, bool isP1Turn, int turnCounter, bool isNodeAlpha, double alpha, double beta)
    {
        if (isNodeAlpha)
        {
            print("Alpha node with depth " + depth + ", maximizing.");
        }
        else
        {
            print("Beta node with depth " + depth + ", minimizing.");
        }
        //Step 1: If board state is terminal, then return evaluation function
        //Depth check
        if (depth == 0)
        {
            print("Depth reached");
            double b = eval.EvaluationFunction(board, moveBoard, GameManager.instance.isIAP1, isP1Turn, turnCounter);
            print("Return " + b);
            return b;
        }

        //Move check
        bool canMove = false;
        for (int i = 0; i < moveBoard.GetLength(0) && !canMove; i++)
        {
            for (int j = 0; j < moveBoard.GetLength(1) && !canMove; j++)
            {
                if (moveBoard[i, j] != 0)
                    canMove = true;
            }
        }
        if (!canMove)
        {
            print("Can't move");
            return eval.EvaluationFunction(board, moveBoard, GameManager.instance.isIAP1, isP1Turn, turnCounter);
        }

        //Step 2: Expand node
        List<Constants.Square> moves = getPossibleMoves(moveBoard, isP1Turn);
        bool needToChange = false;

        //Step 2.5: If one player cannot move, the expansion of the node produces the same type of node as the original
        if(moves.Count == 0)
        {
            moves = getPossibleMoves(moveBoard, !isP1Turn);
            needToChange = true;
        }

        List<byte[,]> newBoards = new List<byte[,]>();
        for (int i = 0; i < moves.Count; i++)
        {
            if (needToChange)
            {
                newBoards.Add(ComputeNewBoard(board, moves[i], !isP1Turn));
            }
            else
                newBoards.Add(ComputeNewBoard(board, moves[i], isP1Turn));
        }
        List<byte[,]> newMoveBoards = new List<byte[,]>();
        for (int i = 0; i < newBoards.Count; i++)
        {
            newMoveBoards.Add(Constants.ComputeMovesBoard(newBoards[i]));
        }

        //Step 3: Initialize value to return
        double res;
        if (isNodeAlpha)
            res = double.MinValue;
        else
            res = double.MaxValue;

        //Step 4: Find the maximum or the minimum value recursively, removing nodes if needed
        print(moves.Count);
        for (int i = 0; i < newBoards.Count; i++)
        {
            double val;
            if(needToChange)
                val = AlphaBetaTurn(newBoards[i], newMoveBoards[i], depth - 1, isP1Turn, turnCounter + 1, isNodeAlpha, alpha, beta);
            else
                val = AlphaBetaTurn(newBoards[i], newMoveBoards[i], depth - 1, !isP1Turn, turnCounter + 1, !isNodeAlpha, alpha, beta);
            print("Value found for move [" + moves[i].x + "," + moves[i].y + "]: " + val);
            if (isNodeAlpha)
            {
                print("Alpha val: " + alpha + "; val to compare: " + val);
                if (val > res)
                    res = val;
                if (res > alpha)
                    alpha = res;
            }
            else
            {
                print("Beta val: " + beta + "; val to compare: " + val);
                if (val < res)
                    res = val;
                if (res < beta)
                    beta = res;
            }
            print("Alpha val: " + alpha + "; Beta val: " + beta);
            if (alpha >= beta)
            {
                print("Alpha greater than beta. Returning " + res);
                return res;
            }
        }
        print("Returning " + res);
        return res;
    }



    void PerformAlphaBetaTurn(byte[,] board)
    {
        byte[,] moveBoard = Constants.ComputeMovesBoard(board);
        //If IA is P1, then isP1turn is true. Else, it's false
        List<Constants.Square> moves = getPossibleMoves(moveBoard, true);
        if (moves.Count == 0)
        {
            StartCoroutine(GameController.instance.WaitMessage());
        }
        else
        {
            string mes = "Moves:\n";
            for(int i=0; i<moves.Count; i++)
            {
                mes += "[" + moves[i].x + "," + moves[i].y + "], ";
            }
            print(mes);
            Constants.Square selected = new Constants.Square();
            double max = double.MinValue;
            for (int i = 0; i < moves.Count; i++)
            {
                byte[,] newBoard = ComputeNewBoard(board, moves[i], true);
                byte[,] newMoveBoard = Constants.ComputeMovesBoard(newBoard);
                print("AlphaBeta for [" + moves[i].x + "," + moves[i].y + "]");
                double val = AlphaBetaTurn(newBoard, newMoveBoard, Constants.MAX_DEPTH, !true, 55, false, double.MinValue, double.MaxValue);
                print("Value for [" + moves[i].x + "," + moves[i].y + "]:" + val);
                if (val > max)
                {
                    max = val;
                    selected = moves[i];
                }
            }
            print("Click: [" + selected.x + "," + selected.y + "]");
            //GameController.instance.Click(selected);
        }
    }



    private void Start()
    {
        eval = FindObjectOfType<EvalF>();

        byte[,] board = new byte[,] { { 2,2,2,1,1,2,1,1},
                                      { 1,2,2,2,2,2,2,1},
                                      { 1,2,1,1,1,0,2,1},
                                      { 1,1,2,1,2,2,2,1},
                                      { 1,2,2,1,1,1,1,1},
                                      { 1,2,2,2,1,0,1,0},
                                      { 1,2,0,2,1,1,1,1},
                                      { 2,2,2,2,1,0,1,0} };

        PerformAlphaBetaTurn(board);
    }*/
}
