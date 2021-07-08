using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SudokuGrid s;
        private List<int[,]> solutions;
        private int solutionIndex; //index used when there are multiple solutions
        private bool solutionLoaded = false;

        public MainWindow()
        {
            InitializeComponent();

            bkButton.Visibility = fwButton.Visibility = resultsLabel.Visibility = Visibility.Hidden;
        }

        private TextBox GetSudokuCell(int x, int y)
        {
            if (x < 1 || x > 9 || y < 1 || y > 9)
            {
                return null;
            }

            return this.FindName($"t{x}{y}") as TextBox;
        }

        private void SetSudokuCell(int x, int y, int val)
        {
            if (x < 1 || x > 9 || y < 1 || y > 9)
            {
                return;
            }

            TextBox aux = this.FindName($"t{x}{y}") as TextBox;
            aux.Text = val.ToString();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate())
            {
                MessageBox.Show(this, "One or more cells are not containing a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CountGridNumbers() < 20)
            {
                if (MessageBox.Show(this, "There are too few numbers on the input. It could take a really long time to find the solutions. Do you want to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
            }

            int[,] grid = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (GetSudokuCell(i + 1, j + 1).Text.Trim() != "")
                        grid[i, j] = int.Parse(GetSudokuCell(i + 1, j + 1).Text);
                    else
                        grid[i, j] = 0; //0 indicates that the cell is empty
                }
            }
                

            s = new SudokuGrid();
            s.InputData(grid);

            int r = s.FindSolutions();

            MessageBox.Show($"{r} solutions found.");

            ShowSolutions();

            if (r > 0)
            {
                solveButton.IsEnabled = false;
                solutionLoaded = true;
            }
                
        }

        private void ShowSolutions()
        {
            int r = s.GetTotalSolutions();
            if (r < 1)
                return;
            solutions = new List<int[,]>();

            for (int i = 0; i < r; i++)
                solutions.Add(s.GetSolutionByIndex(i));

            solutionIndex = 0;

            bkButton.Visibility = fwButton.Visibility = resultsLabel.Visibility = Visibility.Visible;

            bkButton.IsEnabled = false;

            if (r == 1)
                fwButton.IsEnabled = false;
            else
                fwButton.IsEnabled = true;

            resultsLabel.Content = $"result {solutionIndex+1}/{r}";

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {

                    if (GetSudokuCell(i + 1, j + 1).Text.Trim() == "")
                    {
                        GetSudokuCell(i + 1, j + 1).Text = s.GetSolutionByIndex(solutionIndex)[i, j].ToString();
                        GetSudokuCell(i + 1, j + 1).Foreground = Brushes.Green;
                    }


                    GetSudokuCell(i + 1, j + 1).IsReadOnly = true;
                }
            }

        }

        private void UpdateGridSolution()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    GetSudokuCell(i + 1, j + 1).Text = s.GetSolutionByIndex(solutionIndex)[i, j].ToString();
                }
            }
        }

        private void ClearGrid()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    GetSudokuCell(i + 1, j + 1).Text = "";
                    GetSudokuCell(i + 1, j + 1).Foreground = Brushes.Black;
                    GetSudokuCell(i + 1, j + 1).IsReadOnly = false;
                }
            }

            bkButton.Visibility = fwButton.Visibility = resultsLabel.Visibility = Visibility.Hidden;
            solutionLoaded = false;
            solveButton.IsEnabled = true;
        }

        private void ClearGridSolution()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (GetSudokuCell(i + 1, j + 1).Foreground == Brushes.Green)
                    {
                        GetSudokuCell(i + 1, j + 1).Text = "";
                        GetSudokuCell(i + 1, j + 1).Foreground = Brushes.Black;
                    }
                    
                    
                    GetSudokuCell(i + 1, j + 1).IsReadOnly = false;
                }
            }

            bkButton.Visibility = fwButton.Visibility = resultsLabel.Visibility = Visibility.Hidden;
            solutionLoaded = false;
            solveButton.IsEnabled = true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid();
        }

        private void ClearOutputButton_Click(object sender, RoutedEventArgs e)
        {
            if (solutionLoaded)
            {
                ClearGridSolution();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Exit();
        }

        /// <summary>
        /// Checks all sudoku cells to see if there are only numbers.
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                    if (!IsValidNumber(GetSudokuCell(i, j).Text))
                    {
                        //System.Diagnostics.Debug.Write(getSudokuCell(i, j) + " no es valido");
                        return false;
                    }
                        

            return true;
        }

        /// <summary>
        /// Returns the total count of input numbers
        /// </summary>
        /// <returns></returns>
        private int CountGridNumbers()
        {
            int r = 0;
            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                    if (GetSudokuCell(i, j).Text.Trim() != "")
                    {
                        r++;
                    }


            return r;
        }

        private bool IsValidNumber(string n)
        {
            //counts empty space as valid cell
            if (n.Trim() == "")
                return true;

            try
            {
                int aux = int.Parse(n);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void Exit()
        {
            if (MessageBox.Show(this, "Are you sure you want to exit?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void FwButton_Click(object sender, RoutedEventArgs e)
        {
            if (solutionIndex == s.GetTotalSolutions() - 1)
                return;

            solutionIndex++;

            bkButton.IsEnabled = true;

            if (solutionIndex == s.GetTotalSolutions() - 1)
                fwButton.IsEnabled = false;

            resultsLabel.Content = $"result {solutionIndex + 1}/{s.GetTotalSolutions()}";

            UpdateGridSolution();
        }

        private void BkButton_Click(object sender, RoutedEventArgs e)
        {
            if (solutionIndex == 0)
                return;

            solutionIndex--;

            fwButton.IsEnabled = true;

            if (solutionIndex == 0)
                bkButton.IsEnabled = false;

            resultsLabel.Content = $"result {solutionIndex + 1}/{s.GetTotalSolutions()}";

            UpdateGridSolution();
        }
    }
}
