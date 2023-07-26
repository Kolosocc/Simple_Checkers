using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MyChess
{
    public partial class Form1 : Form
    {
        private const int BoardSize = 8; // Размер доски (8x8)
        private const int CellSize = 50; // Размер клетки (50 пикселей)

        private readonly Brush LightCellColor = Brushes.White; // Цвет светлой клетки
        private readonly Brush DarkCellColor = Brushes.Black; // Цвет темной клетки

        private readonly Brush LightPieceColor = Brushes.Red; // Цвет светлой шашки
        private readonly Brush DarkPieceColor = Brushes.Blue; // Цвет темной шашки
        private readonly Brush СrownColor = Brushes.Yellow; // Цвет короны

        private int firstClickX = -1;
        private int firstClickY = -1;
        private int motion = 2; // 1 - ход красного, 2 - ход синего

        private int[,] boardState = new int[BoardSize, BoardSize] {
                { 0,1,0,1,0,1,0,1 },
                { 1,0,1,0,1,0,1,0 },
                { 0,1,0,1,0,1,0,1 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 2,0,2,0,2,0,2,0 },
                { 0,2,0,2,0,2,0,2 },
                { 2,0,2,0,2,0,2,0 }
            }; // Состояние доски (1 - шашка красная 2 - синия, 0 - пусто)

        private List<int[]> arrList = new List<int[]>();
        public Form1()
        {
            InitializeComponent();
            Size = new Size(415, 480); // задаём размер поля
            textBox3.Text = "Ход Синих";
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем клетки и шашки
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    Brush cellColor = (row + col) % 2 == 0 ? LightCellColor : DarkCellColor;
                    g.FillRectangle(cellColor, col * CellSize, row * CellSize, CellSize, CellSize);

                    Brush pieceColor = null;
                    if (boardState[row, col] != 0)
                    {
                        switch (boardState[row, col])
                        {
                            case 1:
                                pieceColor = LightPieceColor;
                                break;
                            case 2:
                                pieceColor = DarkPieceColor;
                                break;
                            case 3:
                                pieceColor = LightPieceColor;
                                g.FillEllipse(pieceColor, col * CellSize + 5, row * CellSize + 5, CellSize - 10, CellSize - 10);
                                pieceColor = СrownColor;
                                g.FillEllipse(pieceColor, col * CellSize + 15, row * CellSize + 15, CellSize - 30, CellSize - 30);
                                break;
                            case 4:
                                pieceColor = DarkPieceColor;
                                g.FillEllipse(pieceColor, col * CellSize + 5, row * CellSize + 5, CellSize - 10, CellSize - 10);
                                pieceColor = СrownColor;
                                g.FillEllipse(pieceColor, col * CellSize + 15, row * CellSize + 15, CellSize - 30, CellSize - 30);
                                break;
                        }

                        if (boardState[row, col] == 1 || boardState[row, col] == 2)
                        {
                            g.FillEllipse(pieceColor, col * CellSize + 5, row * CellSize + 5, CellSize - 10, CellSize - 10);
                        }
                    }
                }
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            // определяем на какую клетку нажал игрок
            Point p = PointToClient(Cursor.Position);
            int clickedRow = p.Y / CellSize;
            int clickedCol = p.X / CellSize;
            textBox1.Text = clickedRow.ToString();
            textBox2.Text = clickedCol.ToString();

            if (!IsValidCell(clickedRow, clickedCol))
            {
                return;
            }

            if (boardState[clickedRow, clickedCol] == motion || boardState[clickedRow, clickedCol] == motion + 2)
            {
                // Шашка была кликнута
                if (arrList.Count > 0)
                {
                    foreach (int[] List in arrList)
                    {
                        if (List[0] == clickedRow && List[1] == clickedCol)
                        {
                            firstClickX = clickedRow;
                            firstClickY = clickedCol;
                            break;
                        }
                    }
                }
                else
                {
                    firstClickX = clickedRow;
                    firstClickY = clickedCol;
                }
            }
            else
            {
                // Шашка уже была выбрана
                if (firstClickX != -1 && firstClickY != -1 &&
                    PossibilityMove(firstClickX, firstClickY, clickedRow, clickedCol))
                {
                    if (arrList.Count > 0)
                    {
                        MovePiece(firstClickX, firstClickY, clickedRow, clickedCol);
                        CreateQueen(clickedRow, clickedCol);
                        boardState[firstClickX + (clickedRow - firstClickX) / 2, firstClickY + (clickedCol - firstClickY) / 2] = 0;
                        checkToWin();
                        arrList.Clear();
                        CanKill(clickedRow, clickedCol);
                        if (arrList.Count > 0)
                        {
                            firstClickX = clickedRow;
                            firstClickY = clickedCol;
                        }
                        else
                        {
                            motion = motion == 1 ? 2 : 1;
                            textBox3.Text = motion == 1 ? "Ход Красных" : "Ход Синих";
                            firstClickX = -1;
                            firstClickY = -1;
                            arrList.Clear();
                            CanKillFull();
                        }
                    }
                    else
                    {
                        MovePiece(firstClickX, firstClickY, clickedRow, clickedCol);
                        CreateQueen(clickedRow, clickedCol);
                        motion = motion == 1 ? 2 : 1;
                        textBox3.Text = motion == 1 ? "Ход Красных" : "Ход Синих";
                        firstClickX = -1;
                        firstClickY = -1;
                        arrList.Clear();
                        CanKillFull();
                    }

                    // Вывод значений массива DELL
                    foreach (int[] List in arrList)
                    {
                        foreach (int element in List)
                        {
                            Console.Write(element + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.ReadLine();
                }
                Console.ReadLine();
            }
            Refresh(); // Обновление доски
        }
    

        private bool IsValidCell(int row, int col)
        {
            return row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
        }

        private void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
        {
            if(boardState[fromRow, fromCol] == motion + 2)
            {
                boardState[toRow, toCol] = motion+2;
                boardState[fromRow, fromCol] = 0;
            }
            else
            {
                boardState[toRow, toCol] = motion;
                boardState[fromRow, fromCol] = 0;
            }
        }

        private bool PossibilityMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            // Проверяем, что выбранная клетка пустая
            if (boardState[toRow, toCol] == 0)
            {
                if(motion == 2)
                {
                    if (arrList.Count > 0)
                    {
                        for (int i = 0; i < arrList.Count; i++)
                        {
                            if (arrList[i][0] == fromRow && arrList[i][1] == fromCol &&
                                arrList[i][2] == toRow && arrList[i][3] == toCol)
                            {
                                return true;
                            }
                            if(i == arrList.Count - 1) { return false; }
                        }
                    }
                    else
                    {
                        if ((Math.Abs(fromCol - toCol) == 1) && (fromRow - toRow == 1 || boardState[fromRow, fromCol] == 4))
                        {
                            return true;
                        }
                        return false; 
                    }
                }
                else
                {
                    if (arrList.Count > 0)
                    {
                        for (int i = 0; i < arrList.Count; i++)
                        {
                            if (arrList[i][0] == fromRow && arrList[i][1] == fromCol &&
                                arrList[i][2] == toRow && arrList[i][3] == toCol)
                            {
                                return true;
                            }
                            if (i == arrList.Count - 1) { return false; }
                        }
                    }
                    else
                    {
                        if ((Math.Abs(fromCol - toCol) == 1) && (fromRow - toRow == -1 || boardState[fromRow, fromCol] == 3))
                        {
                            return true;
                        }
                        return false; 
                    }
                }
            }
            return false;
        }

        private void CanKillFull()
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    for(int i = -1; i < 2; i += 2)
                    {
                        for (int j = -1; j < 2; j += 2)
                        {
                            if (boardState[row, col] == motion || boardState[row, col] == motion+2) 
                            {
                                if ((row + i) >= 0 && (row + i) < BoardSize && (col + j) >= 0 && (col + j) < BoardSize)
                                {
                                    if (boardState[row + i, col + j] != motion && boardState[row + i, col + j] != 0 && boardState[row + i, col + j] != motion + 2)
                                    {
                                        // Проверяем, что есть свободное место за шашкой противника
                                        int newRow = row + i*2 ;
                                        int newColumn = col + j*2;

                                        if (newRow >= 0 && newRow < BoardSize && newColumn >= 0 && newColumn < BoardSize &&
                                            boardState[newRow, newColumn] == 0)
                                        {
                                            int[] ArrayOfInts = { row, col, newRow, newColumn };
                                            arrList.Add(ArrayOfInts);
                                        }
                                    }
                                }
                            }
                        }
                    }  
                }
            }
        }

        private void CanKill(int fromRow, int fromCol)
        {
            for (int i = -1; i < 2; i += 2)
            {
                for (int j = -1; j < 2; j += 2)
                {
                    if ((fromRow + i) >= 0 && (fromRow + i) < BoardSize && (fromCol + j) >= 0 && (fromCol + j) < BoardSize)
                    {
                        if (boardState[fromRow + i, fromCol + j] != motion && boardState[fromRow + i, fromCol + j] != 0 && boardState[fromRow + i, fromCol + j] != motion+2)
                        {
                            // Проверяем, что есть свободное место за шашкой противника
                            int newRow = fromRow + i * 2;
                            int newColumn = fromCol + j * 2;

                            if (newRow >= 0 && newRow < BoardSize && newColumn >= 0 && newColumn < BoardSize &&
                                boardState[newRow, newColumn] == 0)
                            {
                                int[] ArrayOfInts = { fromRow, fromCol, newRow, newColumn };
                                arrList.Add(ArrayOfInts);
                            }
                        }
                    }
                }
            }
        }

        private void CreateQueen(int fromRow, int fromCol)
        {
            if (motion == 2 && fromRow == 0)
            {
                boardState[fromRow, fromCol] = 4;
            }
            else if(motion == 1 && fromRow == BoardSize - 1)
            {
                boardState[fromRow, fromCol] = 3;
            }
        }

        private void checkToWin()
        {
            bool blueWins = true;
            bool redWins = true;

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (boardState[i, j] == 1 || boardState[i, j] == 3)
                    {
                        blueWins = false;
                    }

                    if (boardState[i, j] == 2 || boardState[i, j] == 4)
                    {
                        redWins = false;
                    }
                }
            }

            if (blueWins)
            {
                textBox4.Text = "Победа синих!";
            }
            else if (redWins)
            {
                textBox4.Text = "Победа красных!";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[,] values = new int[,]
            {
    { 0,1,0,1,0,1,0,1 },
    { 1,0,1,0,1,0,1,0 },
    { 0,1,0,1,0,1,0,1 },
    { 0,0,0,0,0,0,0,0 },
    { 0,0,0,0,0,0,0,0 },
    { 2,0,2,0,2,0,2,0 },
    { 0,2,0,2,0,2,0,2 },
    { 2,0,2,0,2,0,2,0 }
            };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    boardState[i, j] = values[i, j];
                }
            }
            
                motion= 2;
                textBox3.Text = "Ход Синих";
            
            textBox3.Text = "Ход Синих";
            int firstClickX = -1;
            int firstClickY = -1;
            arrList.Clear();
            Refresh(); // Обновление доски
        }
    }
}
