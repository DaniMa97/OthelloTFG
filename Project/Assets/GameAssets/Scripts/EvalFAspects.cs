using System;
using System.Collections;
using UnityEngine;

public class EvalFAspects //: MonoBehaviour
{

    private void DebugPrintStability(Constants.Square square)
    {
        Debug.Log("Square["+square.x+","+square.y+"] set as stable."); /* Use this to test inside unity */
        //Console.WriteLine("Square["+square.x+","+square.y+"] set as stable."); /* Use this to test outside unity */
    }
    
    private int CalculateValueOpPl(int playerValue, int opponentValue)
    {
        return 1000 * (playerValue - opponentValue) / (playerValue + opponentValue + 2);
    }

    private int CountAdjacentEmpty(byte[,] board, int posX, int posY)
    {
        int sum = 0;
        for (int a = posX - 1; a <= posX + 1; a++)
        {
            for (int b = posY - 1; b <= posY + 1; b++)
            {
                if (a >= 0 && b >= 0 && a < board.GetLength(0) && b < board.GetLength(1))
                {
                    if (board[a, b] == 0)
                        sum++;
                }
            }
        }
        return sum;
    }

    private int ConvertBase3Base10(string s)
    {
        int num = int.Parse(s);
        int res = 0, counter = 0;
        while(num > 0)
        {
            res += (num % 10) * (int)Math.Pow(3, counter);
            num /= 10;
            counter++;
        }
        return res;
    }


    private char InvertCharString(string s, int pos)
    {
        if (s[pos] == '1')
            return '2';
        else if (s[pos] == '2')
            return '1';
        else if (s[pos] == '0')
            return '0';
        else
            return (char) 0;
    }

    //Stability

    public double EdgeStability(byte[,] board, bool isP1evaluated, bool isP1turn, double[] configurations)
    {
        string top = "", left = "", right = "", bottom = "";
        double sum = 0;

        //Find edges
        //Top & bottom
        for(int i=0; i < board.GetLength(1); i++)
        {
            top += board[0, i];
            bottom += board[board.GetLength(0) - 1, board.GetLength(1) - 1 - i];
        }

        //Left & right
        for(int i = 0; i < board.GetLength(0); i++) 
        {
            left += board[board.GetLength(0) - 1 - i, 0];
            right += board[i, board.GetLength(1) - 1];
        }

        //Table is computed for P1 when P1 has to move, change if neccesary
        //If player to move is not P1, invert all discs
        /*if (!isP1turn)
        {
            string aux1 = "", aux2 = "", aux3 = "", aux4 = "";
            for(int i=0; i<top.Length; i++)
            {
                aux1 += InvertCharString(top, i);
                aux2 += InvertCharString(bottom, i);
                aux3 += InvertCharString(left, i);
                aux4 += InvertCharString(right, i);
            }
            top = aux1;
            bottom = aux2;
            left = aux3;
            right = aux4;
        }*/

        //Find indexes in the table from the string
        int indexT = ConvertBase3Base10(top);
        int indexB = ConvertBase3Base10(bottom);
        int indexL = ConvertBase3Base10(left);
        int indexR = ConvertBase3Base10(right);

        int multiple = 1;

        //Find sum and invert it if player is not P1
        if (!isP1evaluated)
            multiple = -1;

        sum += configurations[indexT] * multiple;
        //Outside Unity debug
        //Console.WriteLine("Partial edge stability (top): " + configurations[indexT] * multiple);
        //Inside Unity debug
        //Debug.Log("Partial edge stability (top): " + configurations[indexT] * multiple);

        sum += configurations[indexB] * multiple;
        //Outside Unity debug
        //Console.WriteLine("Partial edge stability (bottom): " + configurations[indexB] * multiple);
        //Inside Unity debug
        //Debug.Log("Partial edge stability (bottom): " + configurations[indexB] * multiple);

        sum += configurations[indexL] * multiple;
        //Outside Unity debug
        //Console.WriteLine("Partial edge stability (left): " + configurations[indexL] * multiple);
        //Inside Unity debug
        //Debug.Log("Partial edge stability (left): " + configurations[indexL] * multiple);

        sum += configurations[indexR] * multiple;
        //Outside Unity debug
        //Console.WriteLine("Partial edge stability (right): " + configurations[indexR] * multiple);
        //Inside Unity debug
        //Debug.Log("Partial edge stability (right): " + configurations[indexR] * multiple);

        //Outside Unity debug
        //Console.WriteLine("Final edge stability: " + sum);
        //Inside Unity debug
        //Debug.Log("Final edge stability: " + sum);

        return sum;
    }

    public int InternalStability(byte[,] board, bool isP1evaluated)
    {
        //Step 1
        Queue set = new Queue();
        bool[,] stableSquares = new bool[board.GetLength(0), board.GetLength(1)];

        //Step 2: corners
        //Left up corner
        if (board[0, 0] != 0)
        {
            stableSquares[0, 0] = true;
            Constants.Square square = new Constants.Square(0, 0);
            set.Enqueue(square);
            //DebugPrintStability(square);
        }

        //Right up corner
        if (board[0, board.GetLength(1) - 1] != 0)
        {
            stableSquares[0, board.GetLength(1) - 1] = true;
            Constants.Square square = new Constants.Square(0, board.GetLength(1) - 1);
            set.Enqueue(square);
            //DebugPrintStability(square);
        }

        //Left bottom corner
        if (board[board.GetLength(0) - 1, 0] != 0)
        {
            stableSquares[board.GetLength(0) - 1, 0] = true;
            Constants.Square square = new Constants.Square(board.GetLength(0) - 1, 0);
            set.Enqueue(square);
            //DebugPrintStability(square);
        }

        //Right bottom corner
        if (board[board.GetLength(0) - 1, board.GetLength(1) - 1] != 0)
        {
            stableSquares[board.GetLength(0) - 1, board.GetLength(1) - 1] = true;
            Constants.Square square = new Constants.Square(board.GetLength(0) - 1, board.GetLength(1) - 1);
            set.Enqueue(square);
            //DebugPrintStability(square);
        }

        //Step 3
        while(set.Count != 0)
        {
            Constants.Square current = (Constants.Square)set.Dequeue();
            for(int i=current.x-1; i<=current.x+1; i++)
            {
                for(int j=current.y-1; j<=current.y+1; j++)
                {
                    if (!(i == current.x && j == current.y) && (i >= 0 && j >= 0 && i < board.GetLength(0) && j < board.GetLength(1)) && board[i, j] != 0)
                    {
                        Constants.Square process = new Constants.Square(i, j);
                        bool horizontal = false, vertical = false, lrDiagonal = false, rlDiagonal = false;

                        //Check stability in the four directions
                        //Horizontal
                        if (process.y - 1 < 0 || process.y + 1 >= board.GetLength(1))
                            horizontal = true;
                        else if (board[process.x, process.y - 1] == board[process.x, process.y] && stableSquares[process.x, process.y - 1])
                            horizontal = true;
                        else if (board[process.x, process.y + 1] == board[process.x, process.y] && stableSquares[process.x, process.y + 1])
                            horizontal = true;

                        //Vertical
                        if (process.x - 1 < 0 || process.x + 1 >= board.GetLength(0))
                            vertical = true;
                        else if (board[process.x - 1, process.y] == board[process.x, process.y] && stableSquares[process.x - 1, process.y])
                            vertical = true;
                        else if (board[process.x + 1, process.y] == board[process.x, process.y] && stableSquares[process.x + 1, process.y])
                            vertical = true;

                        //Diagonals
                        if (process.x - 1 < 0 || process.x + 1 >= board.GetLength(0) || process.y - 1 < 0 || process.y + 1 >= board.GetLength(1))
                        {
                            lrDiagonal = true;
                            rlDiagonal = true;
                        }
                        else
                        {
                            //Left Right
                            if (board[process.x - 1, process.y - 1] == board[process.x, process.y] && stableSquares[process.x - 1, process.y - 1])
                                lrDiagonal = true;
                            else if (board[process.x + 1, process.y + 1] == board[process.x, process.y] && stableSquares[process.x + 1, process.y + 1])
                                lrDiagonal = true;

                            //Right Left
                            if (board[process.x - 1, process.y + 1] == board[process.x, process.y] && stableSquares[process.x - 1, process.y + 1])
                                rlDiagonal = true;
                            else if (board[process.x + 1, process.y - 1] == board[process.x, process.y] && stableSquares[process.x + 1, process.y - 1])
                                rlDiagonal = true;
                        }

                        if (horizontal && vertical && lrDiagonal && rlDiagonal && !stableSquares[process.x, process.y])
                        {
                            stableSquares[process.x, process.y] = true;
                            set.Enqueue(process);
                            //DebugPrintStability(process);
                        }
                    }
                }
            }            
        }

        //Step 4
        int totalStablesPl = 0, totalStablesOp = 0; ;
        for (int i = 0; i < stableSquares.GetLength(0); i++)
        {
            for (int j = 0; j < stableSquares.GetLength(1); j++)
            {
                if (stableSquares[i, j])
                {
                    if (isP1evaluated)
                    {
                        if (board[i, j] == 1)
                            totalStablesPl++;
                        else if (board[i, j] == 2)
                            totalStablesOp++;
                        else
                        {
                            string debugMes = "";
                            Debug.Log("Some error ocurred in [" + i + "," + j + "] = " + board[i, j]); /* Use this to test inside unity */
                            debugMes += "Board:\n";
                            for (int a = 0; a < board.GetLength(0); a++)
                            {
                                for (int b = 0; b < board.GetLength(1); b++)
                                {
                                    debugMes += board[a, b] + ",";
                                }
                                debugMes += "\n";
                            }

                            debugMes += "Stable squares board:\n";
                            for (int a = 0; a < stableSquares.GetLength(0); a++)
                            {
                                for (int b = 0; b < stableSquares.GetLength(1); b++)
                                {
                                    if (stableSquares[a, b])
                                        debugMes += "1,";
                                    else
                                        debugMes += "0,";
                                }
                                debugMes += "\n";
                            }
                            Debug.Log(debugMes);
                            //Console.WriteLine("Some error ocurred"); /* Use this to test outside unity */
                        }
                    }
                    else
                    {
                        if (board[i, j] == 1)
                            totalStablesOp++;
                        else if (board[i, j] == 2)
                            totalStablesPl++;
                        else
                        {
                            string debugMes = "";
                            Debug.Log("Some error ocurred in [" + i + "," + j + "] = " + board[i, j]); /* Use this to test inside unity */
                            debugMes += "Board:\n";
                            for(int a=0; a<board.GetLength(0); a++)
                            {
                                for(int b=0; b<board.GetLength(1); b++)
                                {
                                    debugMes += board[a, b] + ",";
                                }
                                debugMes += "\n";
                            }

                            debugMes += "Stable squares board:\n";
                            for (int a = 0; a < stableSquares.GetLength(0); a++)
                            {
                                for (int b = 0; b < stableSquares.GetLength(1); b++)
                                {
                                    if (stableSquares[a, b])
                                        debugMes += "1,";
                                    else
                                        debugMes += "0,";
                                }
                                debugMes += "\n";
                            }
                            Debug.Log(debugMes);
                            //Console.WriteLine("Some error ocurred"); /* Use this to test outside unity */
                        }
                    }
                }
            }
        }

        /* Use this to test inside unity */
        //Debug.Log("Stable squares for player: " + totalStablesPl + "\nStable squares for opponent: " + totalStablesOp); 

        /* Use this to test outside unity */
        //Console.WriteLine("Stable squares for player: " + totalStablesPl + "\nStable squares for opponent: " + totalStablesOp); 

        return totalStablesPl - totalStablesOp;
    }

    //Mobility

    public int CurrentMobility(byte[,] moveBoard, bool isP1evaluated)
    {
        //This board is not the current board, but a board representing which player has access to which square
        //0 in a position means no player has access to it, 1 or 2 means p1 or p2 can access it respectively, 3 means both players have access to it
        //This board has to be recalculated after each move
        int pValue = 0, oValue = 0;
        for (int i = 0; i < moveBoard.GetLength(0); i++)
        {
            for (int j = 0; j < moveBoard.GetLength(1); j++)
            {
                switch (moveBoard[i, j])
                {
                    case 0:
                        break;
                    case 1:
                        if (isP1evaluated)
                            pValue += 2;
                        else
                            oValue += 2;
                        break;
                    case 2:
                        if (isP1evaluated)
                            oValue += 2;
                        else
                            pValue += 2;
                        break;
                    case 3:
                        pValue++;
                        oValue++;
                        break;
                    default:
                        Debug.Log("Some error ocurred"); /* Use this to test inside unity */
                        //Console.WriteLine("Some error ocurred"); /* Use this to test outside unity */
                        break;
                }
            }
        }

        //Outside Unity debug
        //Console.WriteLine("Player moves value: " + pValue + "\nOpponent moves value: " + oValue);

        //Inside Unity debug
        //Debug.Log("Player moves value: " + pValue + "\nOpponent moves value: " + oValue);

        return CalculateValueOpPl(pValue, oValue);
    }

    public int PotentialMobility(byte[,] board, bool isP1Evaluated)
    {
        byte opponentId, playerId;
        if (isP1Evaluated)
        {
            opponentId = 2;
            playerId = 1;
        }
        else
        {
            opponentId = 1;
            playerId = 2;
        }

        int opDiscsAdjacent = 0, opEmptySquaresAdjacent = 0, opEmptyAdjacentSum = 0;
        int plDiscsAdjacent = 0, plEmptySquaresAdjacent = 0, plEmptyAdjacentSum = 0;

        for (int i=0; i<board.GetLength(0); i++)
        {
            for(int j=0; j<board.GetLength(1); j++)
            {
                if (board[i, j] == 0)
                {
                    //Check if empty squares are adjacent to opponent disc
                    bool found = false, foundOp = false, foundP = false;
                    for (int a = i - 1; a <= i + 1 && !found; a++)
                    {
                        for (int b = j - 1; b <= j + 1 && !found; b++)
                        {
                            if (a >= 0 && b >= 0 && a < board.GetLength(0) && b < board.GetLength(1))
                            {
                                //Adjacent square is owned by opponent
                                if (board[a, b] == opponentId)
                                {
                                    if (!foundOp)
                                    {
                                        foundOp = true;
                                        opEmptySquaresAdjacent++;
                                        if (foundP)
                                            found = true;
                                    }
                                }
                                //Adjacent square is owned by player
                                else if (board[a, b] == playerId)
                                {
                                    if (!foundP)
                                    {
                                        foundP = true;
                                        plEmptySquaresAdjacent++;
                                        if (foundOp)
                                            found = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (board[i, j] == opponentId)
                {
                    int empty = CountAdjacentEmpty(board, i, j);
                    if (empty != 0)
                    {
                        opDiscsAdjacent++;
                        opEmptyAdjacentSum += empty;
                    }
                }
                else if (board[i, j] == playerId)
                {
                    int empty2 = CountAdjacentEmpty(board, i, j);
                    if (empty2 != 0)
                    {
                        plDiscsAdjacent++;
                        plEmptyAdjacentSum += empty2;
                    }
                }
                else
                {
                    Debug.Log("Some error ocurred"); /* Use this to test inside unity */
                    //Console.WriteLine("Some error ocurred"); /* Use this to test outside unity */
                    break;
                }
            }
        }

        //Outside Unity debug
        //Console.WriteLine("Discs opponent has adjacent: " + opDiscsAdjacent + "\nEmpty squares adjacent to opponent: " + opEmptySquaresAdjacent + "\nSum of empty squares adjacent to each op disc: " + opEmptyAdjacentSum);
        //Console.WriteLine("Discs player has adjacent: " + plDiscsAdjacent + "\nEmpty squares adjacent to player: " + plEmptySquaresAdjacent + "\nSum of empty squares adjacent to each pl disc: " + plEmptyAdjacentSum);

        //Inside Unity debug
        //Debug.Log("Discs opponent has adjacent: " + opDiscsAdjacent + "\nEmpty squares adjacent to opponent: " + opEmptySquaresAdjacent + "\nSum of empty squares adjacent to each op disc: " + opEmptyAdjacentSum);
        //Debug.Log("Discs player has adjacent: " + plDiscsAdjacent + "\nEmpty squares adjacent to player: " + plEmptySquaresAdjacent + "\nSum of empty squares adjacent to each pl disc: " + plEmptyAdjacentSum);

        return CalculateValueOpPl(plDiscsAdjacent, opDiscsAdjacent) + CalculateValueOpPl(plEmptySquaresAdjacent, opEmptySquaresAdjacent) + CalculateValueOpPl(plEmptyAdjacentSum, opEmptyAdjacentSum);
    }
}
