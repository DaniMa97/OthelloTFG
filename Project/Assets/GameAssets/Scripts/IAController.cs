using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class IAController : MonoBehaviour {

    public static IAController instance;
    EvalF eval;
    Stopwatch stopWatch;


    public bool isBlocked;
    public GameObject messageThinking;


    byte[,] ComputeNewBoard(byte[,] oldBoard, Constants.Square pos, bool isP1Turn)
    {
        byte[,] board = (byte[,]) oldBoard.Clone();
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

        //Debug
        /*string s = "Board previous to change:\n";
        for(int i=0; i<oldBoard.GetLength(0); i++)
        {
            for(int j=0; j<oldBoard.GetLength(1); j++)
            {
                s += oldBoard[i, j] + ",";
            }
            s+="\n";
        }
        s += "Board after change:\n";
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                s += board[i, j] + ",";
            }
            s += "\n";
        }
        print(s);*/
        return board;
    }


	void Awake()
    {
        instance = this;
    }

    void Start()
    {
        eval = FindObjectOfType<EvalF>();
        stopWatch = new Stopwatch();
        messageThinking.SetActive(false);

    }

	// Update is called once per frame
	void Update () {

        //Perform move if game is PvsIA, it's IA's turn and IA can move
        if (GameManager.instance.isPvIA && GameController.instance.isP1Turn == GameManager.instance.isIAP1 && !isBlocked) {
            isBlocked = true;
            StartCoroutine(WaitTurn());
        }
	}

    //Turn of the IA (just selecting the highest eval function in depth 1)
    void PerformTurnDepth1()
    {
        List<Constants.Square> moves = getPossibleMoves(GameController.instance.moveBoard, GameManager.instance.isIAP1);
        if (moves.Count == 0)
        {
            StartCoroutine(GameController.instance.WaitMessage());
        }
        else
        {
            int newCounter = GameController.instance.turnCounter + 1;
            Constants.Square selected = moves[0];

            //Debug
            //print("Calculating value of move [" + moves[0].x + "," + moves[0].y + "]\n");

            byte[,] newBoard = ComputeNewBoard(GameController.instance.board, moves[0], GameController.instance.isP1Turn);
            byte[,] newMoveBoard = Constants.ComputeMovesBoard(newBoard);
            double max = eval.EvaluationFunction(newBoard, newMoveBoard, GameManager.instance.isIAP1, !GameController.instance.isP1Turn, newCounter);

            //Debug
            //print("Value: " + max);


            for (int i=1; i<moves.Count; i++)
            {
                //Debug
                //print("Calculating value of move [" + moves[i].x + "," + moves[i].y + "]\n");

                newBoard = ComputeNewBoard(GameController.instance.board, moves[i], GameController.instance.isP1Turn);
                newMoveBoard = Constants.ComputeMovesBoard(newBoard);
                double val = eval.EvaluationFunction(newBoard, newMoveBoard, GameManager.instance.isIAP1, !GameController.instance.isP1Turn, newCounter);

                //Debug
                //print("Value: " + val);

                if (val > max)
                {
                    max = val;
                    selected = moves[i];
                }
            }

            //Debug
            //print("Position selected: [" + selected.x + "," + selected.y + "]\n");
            GameController.instance.Click(selected, false);
        }
    }

    void PerformMiniMaxTurn(int maxDepth)
    {
        //If IA is P1, then isP1turn is true. Else, it's false
        List<Constants.Square> moves = getPossibleMoves(GameController.instance.moveBoard, GameManager.instance.isIAP1);
        if (moves.Count == 0)
        {
            StartCoroutine(GameController.instance.WaitMessage());
        }
        else
        {
            Constants.Square selected = new Constants.Square();
            double max = double.MinValue;
            for(int i=0; i<moves.Count; i++){
                byte[,] newBoard = ComputeNewBoard(GameController.instance.board, moves[i], GameController.instance.isP1Turn);
                byte[,] newMoveBoard = Constants.ComputeMovesBoard(newBoard);
                double val = MiniMaxTurn(newBoard, newMoveBoard, maxDepth, !GameController.instance.isP1Turn, GameController.instance.turnCounter + 1, false);
                if(val > max)
                {
                    max = val;
                    selected = moves[i];
                }
            }
            //print("Click [" + selected.x + "," + selected.y + "]");
            GameController.instance.Click(selected, false);
        }
    }

    void PerformAlphaBetaTurn(int maxDepth)
    {
        //If IA is P1, then isP1turn is true. Else, it's false
        List<Constants.Square> moves = getPossibleMoves(GameController.instance.moveBoard, GameManager.instance.isIAP1);
        if (moves.Count == 0)
        {
            StartCoroutine(GameController.instance.WaitMessage());
        }
        else
        {
            Constants.Square selected = new Constants.Square();
            double max = double.MinValue;
            for (int i = 0; i < moves.Count; i++)
            {
                byte[,] newBoard = ComputeNewBoard(GameController.instance.board, moves[i], GameController.instance.isP1Turn);
                byte[,] newMoveBoard = Constants.ComputeMovesBoard(newBoard);
                double val = AlphaBetaTurn(newBoard, newMoveBoard, maxDepth, !GameController.instance.isP1Turn, GameController.instance.turnCounter + 1, false, max, double.MaxValue);
                if (val > max)
                {
                    max = val;
                    selected = moves[i];
                }
            }
            //print("Click: [" + selected.x + "," + selected.y + "]");
            GameController.instance.Click(selected, false);
        }
    }


    /*void PerformTurn()
    {
        switch (GameManager.instance.difficulty)
        {
            case 0:
                PerformTurnDepth1();
                break;
            case 1:
                PerformAlphaBetaTurn(Constants.MAX_MEDIUM_DEPTH);
                break;
            case 2:
                PerformAlphaBetaTurn(Constants.MAX_HARD_DEPTH);
                break;
            default:
                print("Error in GameManager Difficulty setting");
                break;
        }

        //PerformMiniMaxTurn();
    }*/

    //Calculate performance average moves
    void PerformTurn()
    {
        double totalAlgorithm1 = 0, totalMinMax3 = 0, totalAB3 = 0, totalMinMax4 = 0, totalAB4 = 0, /*totalMinMax5 = 0,*/ totalAB5 = 0;
        for(int i=0; i<10; i++)
        {
            //Perform algorithm 1
            stopWatch.Start();
            PerformTurnDepth1NoClick();
            stopWatch.Stop();
            totalAlgorithm1 += stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Reset();

            //Perform minimax depth 3
            stopWatch.Start();
            PerformMiniMaxTurnNoClick(3);
            stopWatch.Stop();
            totalMinMax3 += stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Reset();

            //Perform alphabeta depth 3
            stopWatch.Start();
            PerformAlphaBetaTurnNoClick(3);
            stopWatch.Stop();
            totalAB3 += stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Reset();

            //Perform minimax depth 4
            stopWatch.Start();
            PerformMiniMaxTurnNoClick(4);
            stopWatch.Stop();
            totalMinMax4 += stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Reset();

            //Perform alphabeta depth 4
            stopWatch.Start();
            PerformAlphaBetaTurnNoClick(4);
            stopWatch.Stop();
            totalAB4 += stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Reset();

            /*//Perform minimax depth 5
            stopWatch.Start();
            PerformMiniMaxTurnNoClick(5);
            stopWatch.Stop();
            totalMinMax5 += stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Reset();*/

            //Perform alphabeta depth 5
            stopWatch.Start();
            PerformAlphaBetaTurnNoClick(5);
            stopWatch.Stop();
            totalAB5 += stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Reset();
        }
        totalAlgorithm1 /= 10;
        totalMinMax3 /= 10;
        totalAB3 /= 10;
        totalMinMax4 /= 10;
        totalAB4 /= 10;
        //totalMinMax5 /= 10;
        totalAB5 /= 10;

        print("T alg 1: " + totalAlgorithm1);
        print("T minmax3: " + totalMinMax3);
        print("T ab 3: " + totalAB3);
        print("T minmax4: " + totalMinMax4);
        print("T ab 4: " + totalAB4);
        //print("T minmax5: " + totalMinMax5);
        print("T ab 5: " + totalAB5);
        PerformTurnDepth1();
    }

    //Calculate performance
    void PerformTurnDepth1NoClick()
    {
        List<Constants.Square> moves = getPossibleMoves(GameController.instance.moveBoard, GameManager.instance.isIAP1);
        if (moves.Count == 0)
        {
            StartCoroutine(GameController.instance.WaitMessage());
        }
        else
        {
            int newCounter = GameController.instance.turnCounter + 1;
            Constants.Square selected = moves[0];

            //Debug
            //print("Calculating value of move [" + moves[0].x + "," + moves[0].y + "]\n");

            byte[,] newBoard = ComputeNewBoard(GameController.instance.board, moves[0], GameController.instance.isP1Turn);
            byte[,] newMoveBoard = Constants.ComputeMovesBoard(newBoard);
            double max = eval.EvaluationFunction(newBoard, newMoveBoard, GameManager.instance.isIAP1, !GameController.instance.isP1Turn, newCounter);

            //Debug
            //print("Value: " + max);


            for (int i = 1; i < moves.Count; i++)
            {
                //Debug
                //print("Calculating value of move [" + moves[i].x + "," + moves[i].y + "]\n");

                newBoard = ComputeNewBoard(GameController.instance.board, moves[i], GameController.instance.isP1Turn);
                newMoveBoard = Constants.ComputeMovesBoard(newBoard);
                double val = eval.EvaluationFunction(newBoard, newMoveBoard, GameManager.instance.isIAP1, !GameController.instance.isP1Turn, newCounter);

                //Debug
                //print("Value: " + val);

                if (val > max)
                {
                    max = val;
                    selected = moves[i];
                }
            }

            //Debug
            //print("Position selected: [" + selected.x + "," + selected.y + "]\n");
        }
    }

    void PerformMiniMaxTurnNoClick(int maxDepth)
    {
        messageThinking.SetActive(true);
        //If IA is P1, then isP1turn is true. Else, it's false
        List<Constants.Square> moves = getPossibleMoves(GameController.instance.moveBoard, GameManager.instance.isIAP1);
        if (moves.Count == 0)
        {
            StartCoroutine(GameController.instance.WaitMessage());
        }
        else
        {
            Constants.Square selected = new Constants.Square();
            double max = double.MinValue;
            for (int i = 0; i < moves.Count; i++)
            {
                byte[,] newBoard = ComputeNewBoard(GameController.instance.board, moves[i], GameController.instance.isP1Turn);
                byte[,] newMoveBoard = Constants.ComputeMovesBoard(newBoard);
                double val = MiniMaxTurn(newBoard, newMoveBoard, maxDepth, !GameController.instance.isP1Turn, GameController.instance.turnCounter + 1, false);
                if (val > max)
                {
                    max = val;
                    selected = moves[i];
                }
            }
            //print("Click [" + selected.x + "," + selected.y + "]");
        }
    }

    void PerformAlphaBetaTurnNoClick(int maxDepth)
    {
        //If IA is P1, then isP1turn is true. Else, it's false
        List<Constants.Square> moves = getPossibleMoves(GameController.instance.moveBoard, GameManager.instance.isIAP1);
        if (moves.Count == 0)
        {
            StartCoroutine(GameController.instance.WaitMessage());
        }
        else
        {
            Constants.Square selected = new Constants.Square();
            double max = double.MinValue;
            for (int i = 0; i < moves.Count; i++)
            {
                byte[,] newBoard = ComputeNewBoard(GameController.instance.board, moves[i], GameController.instance.isP1Turn);
                byte[,] newMoveBoard = Constants.ComputeMovesBoard(newBoard);
                double val = AlphaBetaTurn(newBoard, newMoveBoard, maxDepth, !GameController.instance.isP1Turn, GameController.instance.turnCounter + 1, false, max, double.MaxValue);
                if (val > max)
                {
                    max = val;
                    selected = moves[i];
                }
            }
            //print("Click: [" + selected.x + "," + selected.y + "]");
        }
    }


    double MiniMaxTurn(byte[,] board, byte[,] moveBoard, int depth, bool isP1Turn, int turnCounter, bool isNodeAlpha){
		
		//Step 1: If board state is terminal, then return evaluation function
		//Depth check
		if(depth == 0){
			return eval.EvaluationFunction(board, moveBoard, GameManager.instance.isIAP1, isP1Turn, turnCounter);
		}
		
		//Move check
		bool canMove = false;
		for(int i = 0; i<moveBoard.GetLength(0) && !canMove; i++){
			for(int j=0; j<moveBoard.GetLength(1) && !canMove; j++){
				if(moveBoard[i,j] != 0)
					canMove = true;
			}
		}
		if(!canMove)
			return eval.EvaluationFunction(board, moveBoard, GameManager.instance.isIAP1, isP1Turn, turnCounter);
		
		//Step 2: Expand node
		List<Constants.Square> moves = getPossibleMoves(moveBoard, isP1Turn);
        bool needToChange = false;

        //Step 2.5: If one player cannot move, the expansion of the node produces the same type of node as the original
        if (moves.Count == 0)
        {
            moves = getPossibleMoves(moveBoard, !isP1Turn);
            needToChange = true;
        }

        List<byte[,]> newBoards = new List<byte[,]>();
		for(int i=0; i<moves.Count; i++){
            if (needToChange)
                newBoards.Add(ComputeNewBoard(board, moves[i], !isP1Turn));
            else
                newBoards.Add(ComputeNewBoard(board, moves[i], isP1Turn));
        }

		List<byte[,]> newMoveBoards = new List<byte[,]>();
		for(int i=0; i<newBoards.Count; i++){
			newMoveBoards.Add(Constants.ComputeMovesBoard(newBoards[i]));
		}

        //Step 3: Initialize value to return
        double res;
        if (isNodeAlpha)
            res = double.MinValue;
        else
            res = double.MaxValue;

        //Step 4: Find the maximum or the minimum value recursively
        for(int i=0; i<newBoards.Count; i++)
        {
            double val;
            if(needToChange)
                val = MiniMaxTurn(newBoards[i], newMoveBoards[i], depth - 1, isP1Turn, turnCounter + 1, isNodeAlpha);
            else
                val = MiniMaxTurn(newBoards[i], newMoveBoards[i], depth - 1, !isP1Turn, turnCounter + 1, !isNodeAlpha);
            if (isNodeAlpha && val > res)
                res = val;
            if (!isNodeAlpha && val < res)
                res = val;
        }
		return res;
	}

    double AlphaBetaTurn(byte[,] board, byte[,] moveBoard, int depth, bool isP1Turn, int turnCounter, bool isNodeAlpha, double alpha, double beta)
    {
        //Step 1: If board state is terminal, then return evaluation function
        //Depth check
        if (depth == 0)
        {
            return eval.EvaluationFunction(board, moveBoard, GameManager.instance.isIAP1, isP1Turn, turnCounter);
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
            return eval.EvaluationFunction(board, moveBoard, GameManager.instance.isIAP1, isP1Turn, turnCounter);

        //Step 2: Expand node
        List<Constants.Square> moves = getPossibleMoves(moveBoard, isP1Turn);
        bool needToChange = false;

        //Step 2.5: If one player cannot move, the expansion of the node produces the same type of node as the original
        if (moves.Count == 0)
        {
            moves = getPossibleMoves(moveBoard, !isP1Turn);
            needToChange = true;
        }

        List<byte[,]> newBoards = new List<byte[,]>();
        for (int i = 0; i < moves.Count; i++)
        {
            if(needToChange)
                newBoards.Add(ComputeNewBoard(board, moves[i], !isP1Turn));
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
        for (int i = 0; i < newBoards.Count; i++)
        {
            double val;
            if(needToChange)
                val = AlphaBetaTurn(newBoards[i], newMoveBoards[i], depth - 1, isP1Turn, turnCounter + 1, isNodeAlpha, alpha, beta);
            else
                val = AlphaBetaTurn(newBoards[i], newMoveBoards[i], depth - 1, !isP1Turn, turnCounter + 1, !isNodeAlpha, alpha, beta);
            if (isNodeAlpha) {
                if (val > res)
                    res = val;
                if (res > alpha)
                    alpha = res;
            }
            else {
                if(val < res)
                    res = val;
                if (res < beta)
                    beta = res;
            }
            if (alpha >= beta)
                return res;
        }
        return res;
    }

	IEnumerator WaitTurn(){
        messageThinking.SetActive(true);
        if (GameManager.instance.difficulty == 2)
            yield return new WaitForSeconds(Constants.TIMER_WAIT_TURN_H);

        else
            yield return new WaitForSeconds(Constants.TIMER_WAIT_TURN_EM);
        stopWatch.Start();
        PerformTurn();
        stopWatch.Stop();
        messageThinking.SetActive(false);
        print("Time to perform turn (in ms): " + stopWatch.Elapsed.TotalMilliseconds);
        stopWatch.Reset();
    }

	List<Constants.Square> getPossibleMoves(byte[,] moveBoard, bool isP1turn){
		List<Constants.Square> moves = new List<Constants.Square>();
        int p;
        if (isP1turn)
            p = 1;
        else
            p = 2;
        //string result = "";
		for (byte i = 0; i < moveBoard.GetLength(0); i++) {
			for (byte j = 0; j < moveBoard.GetLength(1); j++) {
				if (moveBoard[i,j] == p || moveBoard[i, j] == 3) {
                    //result += "[" + i + "," + j + "] ";
					moves.Add (new Constants.Square(i,j));
				}
			}
		}
        //print(result);
		return moves;
	}
}