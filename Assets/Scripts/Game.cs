using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject Circle;
    public GameObject Cross;
    private int playerID = 1;
    public int[,] grid;
    public float offset;
    private bool playable = true;

    void Start()
    {
        grid = new int[3, 3];
    }
    void Update()
    {
        if (playable == true)
        {
            if (playerID == 1 && Input.GetMouseButtonDown(0))
            {
                P1Turn();
            }
            else if (playerID == 2)
            {
                P2Turn();
            }
        }
        if (playable == false)
        {
            if (Input.GetKeyDown("space"))
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    void SwitchPlayer()
    {
        if (playerID == 1)
        {
            playerID = 2;
        }
        else if (playerID == 2)
        {
            playerID = 1;
        }
    }

    void P1Turn()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        //Debug.Log("X: " + (int)worldPosition.x + "Y: " + (int)worldPosition.y);
        if (grid[(int)worldPosition.x, (int)worldPosition.y] == 0) //Si il n'y a rien
        {
            grid[(int)worldPosition.x, (int)worldPosition.y] = 1;
            Instantiate(Circle, new Vector3((int)worldPosition.x + offset, (int)worldPosition.y + offset, 0), Quaternion.identity);
            SwitchPlayer();
            if (checkWinner(1) == true)
            {
                Debug.Log("Player 1 Win");
                playable = false;
            }
            else if (draw() == true)
            {
                Debug.Log("Draw");
                playable = false;
            }
        }
    }

    void P2Turn()
    {
        {
            Move bestMove = findBestMove(grid);
            Debug.Log("ROW: " + bestMove.row + " COL: " + bestMove.col);
            grid[bestMove.row, bestMove.col] = 2;
            Instantiate(Cross, new Vector3(bestMove.row + offset, bestMove.col + offset, 0), Quaternion.identity);
            SwitchPlayer();
            if (checkWinner(2) == true)
            {
                Debug.Log("Player 2 Win");
                playable = false;
            }
            else if (draw() == true)
            {
                Debug.Log("Draw");
                playable = false;
            }
        }
    }

    private bool checkWinner(int player)
    {
        // check rows
        if (grid[0, 0] == player && grid[0, 1] == player && grid[0, 2] == player) { return true; }
        if (grid[1, 0] == player && grid[1, 1] == player && grid[1, 2] == player) { return true; }
        if (grid[2, 0] == player && grid[2, 1] == player && grid[2, 2] == player) { return true; }

        // check columns
        if (grid[0, 0] == player && grid[1, 0] == player && grid[2, 0] == player) { return true; }
        if (grid[0, 1] == player && grid[1, 1] == player && grid[2, 1] == player) { return true; }
        if (grid[0, 2] == player && grid[1, 2] == player && grid[2, 2] == player) { return true; }

        // check diags
        if (grid[0, 0] == player && grid[1, 1] == player && grid[2, 2] == player) { return true; }
        if (grid[0, 2] == player && grid[1, 1] == player && grid[2, 0] == player) { return true; }

        return false;
    }

    private bool draw()
    {
        int compteur = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid[i, j] == 0) { compteur++; }
            }
        }
        if (compteur == 0)
        {
            return true;
        }
        return false;
    }

    ////////////////////////////////////////////////AI////////////////////////////////////////////////
    class Move
    {
        public int row, col;
    };

    static int player = 2, opponent = 1;

    static bool isMovesLeft(int[,] board)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (board[i, j] == 0)
                    return true;
        return false;
    }

    static int evaluate(int[,] b)
    {
        // Checking for Rows for X or O victory.
        for (int row = 0; row < 3; row++)
        {
            if (b[row, 0] == b[row, 1] &&
                b[row, 1] == b[row, 2])
            {
                if (b[row, 0] == player)
                    return +10;
                else if (b[row, 0] == opponent)
                    return -10;
            }
        }

        // Checking for Columns for X or O victory.
        for (int col = 0; col < 3; col++)
        {
            if (b[0, col] == b[1, col] &&
                b[1, col] == b[2, col])
            {
                if (b[0, col] == player)
                    return +10;

                else if (b[0, col] == opponent)
                    return -10;
            }
        }

        // Checking for Diagonals for X or O victory.
        if (b[0, 0] == b[1, 1] && b[1, 1] == b[2, 2])
        {
            if (b[0, 0] == player)
                return +10;
            else if (b[0, 0] == opponent)
                return -10;
        }

        if (b[0, 2] == b[1, 1] && b[1, 1] == b[2, 0])
        {
            if (b[0, 2] == player)
                return +10;
            else if (b[0, 2] == opponent)
                return -10;
        }

        // Else if none of them have won then return 0
        return 0;
    }

    // This is the minimax function
    static int minimax(int[,] board, int depth, bool isMax)
    {
        int score = evaluate(board);

        if (score == 10)
            return score;

        if (score == -10)
            return score;

        if (isMovesLeft(board) == false)
            return 0;

        if (isMax)
        {
            int best = -1000;

            // Traverse all cells
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Check if cell is empty
                    if (board[i, j] == 0)
                    {
                        // Make the move
                        board[i, j] = player;

                        // the maximum value
                        best = Mathf.Max(best, minimax(board,
                                        depth + 1, !isMax));

                        // Undo the move
                        board[i, j] = 0;
                    }
                }
            }
            return best;
        }

        // If this minimizer's move
        else
        {
            int best = 1000;

            // Traverse all cells
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Check if cell is empty
                    if (board[i, j] == 0)
                    {
                        // Make the move
                        board[i, j] = opponent;

                        // the minimum value
                        best = Mathf.Min(best, minimax(board,
                                        depth + 1, !isMax));

                        // Undo the move
                        board[i, j] = 0;
                    }
                }
            }
            return best;
        }
    }

    // This will return the best possible
    static Move findBestMove(int[,] board)
    {
        int bestVal = -1000;
        Move bestMove = new Move();
        bestMove.row = -1;
        bestMove.col = -1;

        // optimal value.
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // Check if cell is empty
                if (board[i, j] == 0)
                {
                    // Make the move
                    board[i, j] = player;

                    int moveVal = minimax(board, 0, false);

                    // Undo the move
                    board[i, j] = 0;

                    // best
                    if (moveVal > bestVal)
                    {
                        bestMove.row = i;
                        bestMove.col = j;
                        bestVal = moveVal;
                    }
                }
            }
        }

        Debug.Log("The value of the best Move is " + bestVal);

        return bestMove;
    }
}
