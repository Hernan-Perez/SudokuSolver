using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver
{
    public class SudokuGrid
    {

        private int[,] grid;
        List<int[,]> solutions;

        public SudokuGrid()
        {
            grid = new int[9, 9];
            solutions = new List<int[,]>();
        }

        public void InputData(int[,] g)
        {
            if (g.GetLength(0) == 9 && g.GetLength(1) == 9)
            {
                grid = g;
            }
            else
            {
                grid = null;
            }
        }

        /// <summary>
        /// Returns the number of solutions found
        /// </summary>
        /// <returns></returns>
        public int FindSolutions()
        {
            if (grid == null)
                return 0;
            solutions.Clear();

            //checks to see if the initial numbers are valid
            if (!isGridValid(grid))
            {
                //debugMSG("invalid initial grid");
                return 0;
            }

            solve(grid);

            return solutions.Count;
        }

        /// <summary>
        /// Each time this func is called its going to try to guess only the first empty cell it encounters and then call itself recursively 
        /// adding the guess number to the grid.
        /// </summary>
        /// <param name="g"></param>
        private void solve(int[,] g)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (g[i, j] == 0)
                    {

                        //if its 0 it means the cell is empty. Then it tries the different posibilities
                        for (int k = 1; k < 10; k++)
                        {
                            g[i, j] = k;


                            if (isGridValid(g))
                            {
                                solve(g);
                            }
                        }

                        g[i, j] = 0;
                        return;
                    }
                }
            }

            //if the code reachs here it means it does not contains empty cells and the solution is valid, so it gets added to the solution list
            solutions.Add(g.Clone() as int[,]);
        }

        /// <summary>
        /// check if the numbers on the grid are valid, and ignores the empty cells
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        private bool isGridValid(int[,] g)
        {
            int i, j;
            int[] l = new int[9];

            //check if rows are valid
            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                    l[j] = g[i, j];

                if (hasDuplicates(l))
                    return false;
            }

            //check if columns are valid
            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                    l[j] = g[j, i];

                if (hasDuplicates(l))
                    return false;
            }

            //check if 3x3 squares are valid
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    l[0] = g[0 + i * 3, 0 + j * 3];
                    l[1] = g[1 + i * 3, 0 + j * 3];
                    l[2] = g[2 + i * 3, 0 + j * 3];
                    l[3] = g[0 + i * 3, 1 + j * 3];
                    l[4] = g[1 + i * 3, 1 + j * 3];
                    l[5] = g[2 + i * 3, 1 + j * 3];
                    l[6] = g[0 + i * 3, 2 + j * 3];
                    l[7] = g[1 + i * 3, 2 + j * 3];
                    l[8] = g[2 + i * 3, 2 + j * 3];

                    if (hasDuplicates(l))
                        return false;
                }
            }


            return true;
        }

        private bool hasDuplicates(int[] g)
        {
            HashSet<int> h = new HashSet<int>();
            for (int i = 0; i < 9; i++)
            {
                if (g[i] == 0)
                    continue;

                if (!h.Add(g[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public int GetTotalSolutions()
        {
            if (solutions == null)
                return 0;
            return solutions.Count;
        }

        public int[,] GetSolutionByIndex(int n)
        {
            if (solutions == null || n >= solutions.Count || n < 0)
                return null;

            return solutions[n];
        }

        private void debugMSG(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

        private void debugGridShow(int[,] g)
        {
            string aux = "";
            for (int i = 0; i < 9; i++)
            {
                aux = "";
                for (int j = 0; j < 9; j++)
                {
                    aux += g[i, j].ToString();
                }
                debugMSG(aux);
            }

            debugMSG(" ");
        }


    }
}
