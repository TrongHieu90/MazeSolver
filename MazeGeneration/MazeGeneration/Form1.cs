using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeGeneration
{
    public partial class Form1 : Form
    {
        protected int c_width;
        protected static int cols;
        protected static int rows;
        protected Spot[,] grid;
        protected Stack<Spot> stack = new Stack<Spot>();

        protected Spot currentSpot;
        protected Spot next;

        //for pathfinding 
        protected List<Spot> openSet; //= new List<Spot>();
        protected List<Spot> closedSet = new List<Spot>();
        protected List<Spot> path;

        protected Spot startPoint;
        protected Spot endPoint;
        protected Spot currentPoint;

        Graphics g;
        Pen blackPen;
        Pen purplePen;
        SolidBrush blackBrush;
        SolidBrush redBrush;
        SolidBrush greenBrush;
        SolidBrush blueBrush;

        //windows form variable
        protected int inputInt; //input to set cols and rows
        protected bool canFindPath; 
        protected bool showProgression;//a checkbox to allow redraw the pathfinding along the maze
        protected bool canDrawBackground = true;
        protected bool canClick = true; //allow a button to be clicked or not

        public Form1()
        {
            InitializeComponent();

            g = Canvas.CreateGraphics();
            blackPen = new Pen(Color.Black, 2);
            purplePen = new Pen(Color.Violet, 5);
            blackBrush = new SolidBrush(Color.Black);
            greenBrush = new SolidBrush(Color.Green);
            blueBrush = new SolidBrush(Color.Blue);
            redBrush = new SolidBrush(Color.DarkRed);
        }

        public class Spot : Form1
        {
            public int i;
            public int j;
            public bool[] walls = { true, true, true, true }; //going 'top botom right left' as element order
            public List<Spot> neighbors;
            public bool visited = false;

            //for pathfinding
            public int f = 0;
            public int h = 0;
            public int g = 0;
            public Spot previous = null;
            public List<Spot> neighborsPF = new List<Spot>();

            public Spot(int i, int j)
            {
                this.i = i;
                this.j = j;
            }
            public Spot CheckNeighbors(Spot[,] grid)
            {
                int i = this.i;
                int j = this.j;

                neighbors = new List<Spot>();

                Spot right, up, down, left;
                if (i < cols - 1)
                {
                    //right adjacent spot
                    right = grid[i + 1, j];
                    if (right != null && !right.visited)
                    {
                        neighbors.Add(right);
                    }
                }
                if (i > 0)
                {
                    left = grid[i - 1, j];
                    //left adjacent spot
                    if (left != null && !left.visited)
                    {
                        neighbors.Add(left);
                    }
                }
                if (j < rows - 1)
                {
                    down = grid[i, j + 1];

                    //down adjacent spot
                    if (down != null && !down.visited)
                    {
                        neighbors.Add(down);
                    }
                }
                if (j > 0)
                {
                    up = grid[i, j - 1];
                    //up adjacent spot
                    if (up != null && !up.visited)
                    {
                        neighbors.Add(up);
                    }
                }
                
                if(neighbors.Count > 0)
                {
                    RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();
                    var r = RandomInteger(Rand, 0, neighbors.Count);
                    return neighbors[r];
                }
                else
                {
                    return null;
                }               
            }

            public void AddNeighborsPF(Spot[,] grid) //adding different kinds of neighbors that ignore wall on our maze
            {
                int i = this.i;
                int j = this.j;

                Spot rightPF, upPF, downPF, leftPF;

                //right adjacent spot
                if (i < cols - 1)
                {
                    rightPF = grid[i + 1, j];
                   
                    if (!rightPF.walls[3]) //left wall on our spot must be false to add right neighbor
                    {
                        neighborsPF.Add(rightPF);
                    }
                }
               

                //left adjacent spot
                if (i > 0)
                {
                    leftPF = grid[i - 1, j];
                    if(!leftPF.walls[1])
                    { 
                        neighborsPF.Add(leftPF);
                    }
                    
                }

                //down adjacent spot
                if (j < rows - 1)
                {
                    downPF = grid[i, j + 1];
                    if(!downPF.walls[0])
                    {
                        neighborsPF.Add(downPF);
                    }
                    
                }

                //up adjacent spot
                if (j > 0)
                {
                    upPF = grid[i, j - 1];
                    if(!upPF.walls[2])
                    {
                        neighborsPF.Add(upPF);
                    }
                }

                ////Diagonal Movement
                //if (i > 0 && j > 0)
                //{
                //    neighbors.Add(grid[i - 1,j - 1]);
                //}
                //if (i < cols - 1 && j > 0)
                //{
                //    neighbors.Add(grid[i + 1,j - 1]);
                //}
                //if (i > 0 && j < rows - 1)
                //{
                //    neighbors.Add(grid[i - 1,j + 1]);
                //}
                //if (i < cols - 1 && j < rows - 1)
                //{
                //    neighbors.Add(grid[i + 1,j + 1]);
                //}
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            showProgression = checkBox1.Checked;
        }


        #region Maze generator
        private void button1_Click(object sender, EventArgs e)
        {
            //MAZE GENERATOR IS IMPLEMENTED HERE
            if (int.TryParse(textBox1.Text, out inputInt))
            {
                if (inputInt == 0)
                {
                    string message = "Cannot use 0 as integer. Please Enter another Integer number.";
                    string caption = "Error Detected in Input";

                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    result = MessageBox.Show(message, caption, buttons);

                    //if (result == DialogResult.OK)
                    //{

                    //}
                }
                else
                {
                    if(canClick)
                    {
                        cols = inputInt;
                        rows = inputInt;

                        c_width = Canvas.Width / cols;

                        grid = new Spot[cols, rows];

                        //building grid
                        for (int i = 0; i < cols; i++)
                        {
                            for (int j = 0; j < rows; j++)
                            {
                                grid[i, j] = new Spot(i, j);
                            }
                        }

                        currentSpot = grid[0, 0];

                        while (true)
                        {
                            currentSpot.visited = true;

                            //Using recursive backtracker algorithm
                            //https://en.wikipedia.org/wiki/Maze_generation_algorithm#Recursive_backtracker

                            //step 1
                            next = currentSpot.CheckNeighbors(grid);

                            if (next != null)
                            {
                                next.visited = true;

                                //step 2
                                stack.Push(currentSpot);
                                //Console.WriteLine($"stack count is {stack.Count}");

                                //step 3
                                RemoveWalls(currentSpot, next);

                                //step 4
                                currentSpot = next;
                                //Console.WriteLine($"currentSpot is {currentSpot.i} and {currentSpot.j}");
                            }
                            else if (stack.Count > 0)
                            {
                                currentSpot = stack.Pop();
                                //Console.WriteLine($"pop a stack and count is {stack.Count}");
                                //Console.WriteLine($"currentSpot after pop is {currentSpot.i} and {currentSpot.j}");

                            }
                            else if (next == null)
                            {
                                break;
                            }

                            //Call DrawGrid here (inside while loop) when we want to see how it works step by step. Really slow down the program
                            //because we used 2d array and how each element is accessed in 2d array(?) and how WF implements draw graphics.
                            //DrawGrid();
                        }

                        //Call DrawGrid outside while loop when the maze has already been generated. Really fast visualization
                        DrawGrid();
                        canDrawBackground = false;
                        canFindPath = true; //allow path to be found after finish drawing the maze
                    }
                    
                }             
            }

            else
            {
                string message = "Please Enter an Integer number.";
                string caption = "Error Detected in Input";

                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
            }
        }
        #endregion

        #region Pathfinding
        private void button2_Click(object sender, EventArgs e)
        {
            //PATHFINDING IS IMPLEMENTED HERE
            if(canFindPath && canClick)
            {
                for (int i = 0; i < cols; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        grid[i, j].AddNeighborsPF(grid);
                    }
                }

                startPoint = grid[0, 0];
                endPoint = grid[cols - 1, rows - 1];

                //Adding the predefined start point to the openSet
                openSet = new List<Spot>();
                openSet.Add(startPoint);


                while (true)
                {
                    if (openSet.Count > 0)
                    {
                        int winner = 0;
                        for (int i = 0; i < openSet.Count; i++)
                        {
                            if (openSet[i].f < openSet[winner].f)
                            {
                                winner = i;
                            }
                        }

                        currentPoint = openSet[winner];

                        if (currentPoint == endPoint)
                        {
                            //Console.WriteLine("Pathfinding done");
                            string message = "Pathfinding complete!";
                            string caption = "Success!";

                            DrawGrid();

                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            DialogResult result;

                            result = MessageBox.Show(message, caption, buttons);
                            break;
                        }

                        RemoveElement(openSet, currentPoint);
                        //openSet.RemoveAt(winner);

                        //closedSet = new List<Spot>();
                        closedSet.Add(currentPoint);

                        var neighbors = currentPoint.neighborsPF;


                        //Console.WriteLine($"current point pos is: {currentPoint.i},{currentPoint.j} and the neighbors.count is: {currentPoint.neighbors.Count}");
                        //checking each neighbor Spot
                        for (int i = 0; i < neighbors.Count; i++)
                        {
                            //Console.WriteLine("checking neighbors");
                            var neighbor = neighbors[i];
                            if (!closedSet.Contains(neighbor))
                            {
                                var tempG = currentPoint.g + 1;
                                var newPath = false;

                                if (openSet.Contains(neighbor))
                                {
                                    if (tempG < neighbor.g)
                                    {
                                        neighbor.g = tempG;
                                        newPath = true;
                                    }
                                }
                                else
                                {
                                    neighbor.g = tempG;
                                    newPath = true;
                                    openSet.Add(neighbor);
                                }
                                if (newPath)
                                {
                                    neighbor.h = Heuristic(neighbor, endPoint);
                                    neighbor.f = neighbor.g + neighbor.h;
                                    neighbor.previous = currentPoint;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine("No pathfinding solution");
                        string message = "Cannot find path to destination!";
                        string caption = "Failure!";

                        DrawGrid();

                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        DialogResult result;

                        result = MessageBox.Show(message, caption, buttons);
                        break;

                    }

                    path = new List<Spot>();
                    var temp = currentPoint;
                    path.Add(temp);
                    while (temp.previous != null)
                    {
                        path.Add(temp.previous);
                        temp = temp.previous;
                    }

                    if (showProgression)
                    {
                        DrawGrid();
                    }

                    //Visualizing open set
                    for (int i = 0; i < openSet.Count; i++)
                    {
                        Rectangle rectOpen = new Rectangle(openSet[i].i * c_width, openSet[i].j * c_width, c_width, c_width);
                        g.FillRectangle(greenBrush, rectOpen);
                    }

                    //Visualizing closed set
                    for (int i = 0; i < closedSet.Count; i++)
                    {
                        Rectangle rectClose = new Rectangle(closedSet[i].i * c_width, closedSet[i].j * c_width, c_width, c_width);
                        g.FillRectangle(redBrush, rectClose);
                    }

                    //visualizing our path
                    for (var i = 0; i < path.Count; i++)
                    {
                        //Graphics r = Canvas.CreateGraphics();
                        Rectangle pathRect = new Rectangle(path[i].i * c_width, path[i].j * c_width, c_width, c_width);
                        g.FillRectangle(blueBrush, pathRect);
                    }
                }

                canClick = false;
            }
            else if(canClick)
            {               
                string message = "Cannot perform Pathfinding yet! Must generate maze first.";
                string caption = "Failed to find path";
                
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
            }
                   
        }
        #endregion
        private void RemoveWalls(Spot a, Spot b)
        {
            var x = a.i - b.i;
            if (x == 1)
            {
                a.walls[3] = false;
                b.walls[1] = false;
            }

            if (x == -1)
            {
                a.walls[1] = false;
                b.walls[3] = false;
            }
            var y = a.j - b.j;
            if (y == 1)
            {
                a.walls[0] = false;
                b.walls[2] = false;
            }

            if (y == -1)
            {
                a.walls[2] = false;
                b.walls[0] = false;
            }
        }

        private int RandomInteger(RNGCryptoServiceProvider Rand, int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                Rand.GetBytes(four_bytes);

                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (int)(min + (max - min) *
                (scale / (double)uint.MaxValue));
        }

        private void DrawGrid()
        {
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    int x = i * c_width;
                    int y = j * c_width;

                    if (grid[i, j].walls[0])
                    {
                        g.DrawLine(purplePen, x, y, x + c_width, y);
                    }
                    if (grid[i, j].walls[1])
                    {

                        g.DrawLine(purplePen, x + c_width, y, x + c_width, y + c_width);
                    }
                    if (grid[i, j].walls[2])
                    {

                        g.DrawLine(purplePen, x + c_width, y + c_width, x, y + c_width);
                    }
                    if (grid[i, j].walls[3])
                    {

                        g.DrawLine(purplePen, x, y + c_width, x, y);
                    }
                    if (canDrawBackground)
                    {
                        if (grid[i, j].visited)
                        {
                            Rectangle Rect = new Rectangle(x, y, c_width, c_width);

                            g.FillRectangle(blackBrush, Rect);
                        }
                    }
                }
            }
        }

        private void RemoveElement(List<Spot> list, Spot element)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == element)
                {
                    list.RemoveAt(i);
                }
            }
        }

        private int Heuristic(Spot node, Spot goal)
        {
            //for implementing different Heuristic functions see here: http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html#S7

            ////Euclidean square distance function
            //var dist = Math.Sqrt(((node.i - goal.i) * (node.i - goal.i) + (node.j - goal.j) * (node.j - goal.j)));
            //return (int)dist;

            //Diagonal distance
            //var D = 1;
            //var D2 = 1;
            //var dx = Math.Abs(node.i - goal.i);
            //var dy = Math.Abs(node.j - goal.j);
            //var dist = D * Math.Max(dx, dy) + (D2 - D) * Math.Min(dx, dy);
            //return dist;

            //Manhattan distance
            var dx = Math.Abs(node.i - goal.i);
            var dy = Math.Abs(node.j - goal.j);
            return (int)(dx + dy);
        }

        #region extra
        //suppressing the alt button causing the form to be redrawn
        protected override void WndProc(ref Message m)
        {
            // Suppress the WM_UPDATEUISTATE message
            if (m.Msg == 0x128) return;
            base.WndProc(ref m);
        }
        #endregion
    }
}
