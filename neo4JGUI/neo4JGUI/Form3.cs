using Microsoft.VisualBasic.Devices;
using Neo4j.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Point = System.Drawing.Point;

namespace neo4JGUI
{
    public partial class Form3 : Form
    {
        private bool clicked = false;
        private int nrClicked = 0;
        private PictureBox clickedPictureBox = null;
        private Point firstPoint = Point.Empty;
        private Point secondPoint = Point.Empty;
        private List<Tuple<Point, Point>> lines = new List<Tuple<Point, Point>>();
        private List<Control> addedControls = new List<Control>();
        private List<Tuple<bool, int>> operationHistory = new List<Tuple<bool, int>>();
        private List<string> pictureBoxNames = new List<string>();

        public Form3()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            clicked = true;
            clickedPictureBox = rectangle;
            pictureBoxNames.Add(rectangle.Name);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            clicked = true;
            clickedPictureBox = circle;
            pictureBoxNames.Add(circle.Name);
        }

        /*private void pictureBox3_Click(object sender, EventArgs e)
        {
            clicked = true;
            clickedPictureBox = rhomb;
            pictureBoxNames.Add(rhomb.Name);
        }*/

        private void Form3_Click(object sender, EventArgs e)
        {
            if (clickedPictureBox == pictureBox4)
            {
                if (nrClicked == 0)
                {
                    firstPoint = PointToClient(MousePosition);
                    nrClicked = 1;
                }
                else if (nrClicked == 1)
                {
                    secondPoint = PointToClient(MousePosition);
                    lines.Add(Tuple.Create(firstPoint, secondPoint));
                    Invalidate();
                    nrClicked = 0;
                    operationHistory.Add(Tuple.Create(true, lines.Count - 1));
                }
            }
            else if (nrClicked != 2)
            {
                nrClicked++;
                if (nrClicked == 2)
                {
                    firstPoint = PointToClient(MousePosition);
                }
            }
            if (clicked)
            {
                clicked = false;
                if (clickedPictureBox == null)
                {
                    var textBox = new System.Windows.Forms.TextBox
                    {
                        Text = "Text",
                        Location = PointToClient(MousePosition),
                        Size = new Size(100, 30)
                    };
                    Controls.Add(textBox);
                    addedControls.Add(textBox);
                    operationHistory.Add(Tuple.Create(false, addedControls.Count - 1));
                    Controls.SetChildIndex(textBox, 0);
                }
                if (clickedPictureBox != null && clickedPictureBox != pictureBox4)
                {
                    var copiedPictureBox = new PictureBox
                    {
                        Image = clickedPictureBox.Image,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Location = PointToClient(MousePosition),
                        Size = clickedPictureBox.Size
                    };
                    Controls.Add(copiedPictureBox);
                    addedControls.Add(copiedPictureBox);
                    operationHistory.Add(Tuple.Create(false, addedControls.Count - 1));
                }
            }
        }

        private void Form3_Paint(object sender, PaintEventArgs e)
        {
            foreach (var line in lines)
            {
                Pen blackPen = new Pen(Color.Black, 4);
                e.Graphics.DrawLine(blackPen, line.Item1, line.Item2);

                DrawArrowhead(e.Graphics, blackPen, line.Item1, line.Item2, 15, 20);

                blackPen.Dispose();
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            clicked = true;
            clickedPictureBox = pictureBox4;
            nrClicked = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (operationHistory.Count > 0)
            {
                var lastOperation = operationHistory[operationHistory.Count - 1];
                if (lastOperation.Item1)
                {
                    if (lines.Count > 0)
                    {
                        lines.RemoveAt(lastOperation.Item2);
                    }
                }
                else
                {
                    if (lastOperation.Item2 < addedControls.Count)
                    {
                        Control controlToRemove = addedControls[lastOperation.Item2];
                        if(controlToRemove is PictureBox) pictureBoxNames.RemoveAt(pictureBoxNames.Count - 1);
                        Controls.Remove(controlToRemove);
                        addedControls.RemoveAt(lastOperation.Item2);
                        controlToRemove.Dispose();
                    }
                }

                operationHistory.RemoveAt(operationHistory.Count - 1);
                Invalidate();
            }
        }

        private void DrawArrowhead(Graphics g, Pen pen, Point lineStart, Point lineEnd, int headWidth, int headLength)
        {
            float angle = (float)Math.Atan2(lineEnd.Y - lineStart.Y, lineEnd.X - lineStart.X);
            float halfWidth = headWidth / 2;

            Point arrow1 = new Point(
                lineEnd.X - (int)(headLength * Math.Cos(angle - Math.PI / 6)),
                lineEnd.Y - (int)(headLength * Math.Sin(angle - Math.PI / 6))
            );

            Point arrow2 = new Point(
                lineEnd.X - (int)(headLength * Math.Cos(angle + Math.PI / 6)),
                lineEnd.Y - (int)(headLength * Math.Sin(angle + Math.PI / 6))
            );

            g.DrawLine(pen, lineEnd, arrow1);
            g.DrawLine(pen, lineEnd, arrow2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Control control in Controls.OfType<PictureBox>().ToList())
            {
                Controls.Remove(control);
                control.Dispose();
            }
            foreach (Control control in Controls.OfType<System.Windows.Forms.TextBox>().ToList())
            {
                Controls.Remove(control);
                control.Dispose();
            }
            lines.Clear();
            addedControls.Clear();
            operationHistory.Clear();
            pictureBoxNames.Clear();
            Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clicked = true;
            clickedPictureBox = null;
            nrClicked = 0;
        }

        private double CalculateDistance(Point point1, Point point2)
        {
            int dx = point2.X - point1.X;
            int dy = point2.Y - point1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private Point FindClosestPoint(List<Point> points, Point p, Dictionary<Point, bool> visited)
        {

            double minDistance = double.MaxValue;
            Point closestPoint = new Point(-1, -1);

            if (visited.ContainsKey(p))
                return closestPoint;

            foreach (var point in points)
            {
                double distance = CalculateDistance(point, p);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = point;
                }
            }

            return closestPoint;
        }

        private async void storeStructureInNeo4JToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Point> lineStarts = new List<Point>();
            List<Point> lineEnds = new List<Point>();
            List<Point> pictureBoxCenters = new List<Point>();
            List<Point> textBoxCenters = new List<Point>();
            List<string> textBoxValuesList = new List<string>();
            Dictionary<Point, string> textBoxValues = new Dictionary<Point, string>();
            //List<Point[]> pictureBoxCorners = new List<Point[]>();
            //List<Point[]> textBoxCorners = new List<Point[]>();
            foreach (var line in lines)
            {
                lineStarts.Add(new Point(line.Item1.X, line.Item1.Y));
                lineEnds.Add(new Point(line.Item2.X, line.Item2.Y));
                // lineCenters.Add(new Point((line.Item1.X + line.Item2.X) / 2, (line.Item1.Y + line.Item2.Y) / 2));
            }

            //int index = 0;
            foreach (var control in addedControls)
            {
                if (control is PictureBox pictureBox)
                {
                    Point pictureBoxCenter = new Point(pictureBox.Left + pictureBox.Width / 2, pictureBox.Top + pictureBox.Height / 2);
                    pictureBoxCenters.Add(pictureBoxCenter);

                    /*Point[] corners = {
                        new Point(pictureBox.Left, pictureBox.Top),
                        new Point(pictureBox.Right, pictureBox.Top),
                        new Point(pictureBox.Left, pictureBox.Bottom),
                        new Point(pictureBox.Right, pictureBox.Bottom)
                    };
                    pictureBoxCorners.Add(corners);

                    index++;*/
                }
                else if (control is System.Windows.Forms.TextBox textBox)
                {
                    Point textBoxCenter = new Point(textBox.Left + textBox.Width / 2, textBox.Top + textBox.Height / 2);
                    textBoxCenters.Add(textBoxCenter);
                    textBoxValuesList.Add(textBox.Text);
                    //Point[] corners = {
                    //new Point(textBox.Left, textBox.Top),
                    //new Point(textBox.Right, textBox.Top),
                    //new Point(textBox.Left, textBox.Bottom),
                    //new Point(textBox.Right, textBox.Bottom)
                    //};
                    // textBoxCorners.Add(corners);
                }
            }

            int i = 0;
            foreach (var textBoxCenter in textBoxCenters)
            {
                textBoxValues[textBoxCenter] = textBoxValuesList[i];
                i++;
            }

            //int indexInPictureBoxes = 0;
            bool done = false;
            //var firstPictureBox = pictureBoxCenters[0];
            var prevPictureBox = pictureBoxCenters[0];
            var previous = pictureBoxCenters[0];
            Dictionary<Point, bool> visited = new Dictionary<Point, bool>();
            Dictionary<Point, bool> visited_line_starts = new Dictionary<Point, bool>();
            Dictionary<Point, bool> visited_line_ends = new Dictionary<Point, bool>();
            Dictionary<Point, bool> visited_textboxes = new Dictionary<Point, bool>();
            List<string> neo4jValues = new List<string>();
            List<string> neo4jStructure = new List<string>();
            List<string> exclude = new List<string>() { "out", "filter", "has", "isSource", "isTarget", "return" };
            visited[prevPictureBox] = true;
            char[] alphabet = new char[26];
            for (int j = 0; j < 26; j++)
            {
                alphabet[j] = (char)('a' + j);
            }
            int indexAlphabet = 0;
            while (!done)
            {
                bool foundLineStart = false;
                foreach (Point p in lineStarts)
                {
                    // double distanceToCenter = CalculateDistance(p, previous);
                    Point closestPoint = FindClosestPoint(pictureBoxCenters, p, visited_line_starts);
                    if (closestPoint.X != -1 && closestPoint.Y != -1)
                    {
                        Point closestTextBox = FindClosestPoint(textBoxCenters, previous, visited_textboxes);
                        string txt = "";
                        if (closestTextBox.X != -1 && closestTextBox.Y != -1)
                        {
                            visited_textboxes[closestTextBox] = true;
                            // for in textBoxCenters to find index in textBoxValues or use dictonary
                            txt = textBoxValues[closestTextBox];
                            string result = neo4jStructure.FirstOrDefault(x => x == txt);
                            string result1 = exclude.FirstOrDefault(x => x == txt);
                            if (result == null && result1 == null)
                            {
                                neo4jStructure.Add(txt);
                                //MessageBox.Show("Found begin !" + txt);
                                string entity = txt[0] + ":";
                                if (entity != "R:" && entity != "P:" && entity != "C:")
                                {
                                    //string promptValue = Prompt.ShowDialog("Value: ", "Enter value for " + txt.Replace(entity, ""));
                                    //neo4jValues.Add(promptValue);
                                    neo4jValues.Add(alphabet[indexAlphabet++].ToString());
                                }
                                else
                                {
                                    neo4jValues.Add("");
                                }
                            }
                        }
                        visited_line_starts[p] = true;
                        foundLineStart = true;
                        break;
                    }
                }
                if (!foundLineStart)
                {
                    prevPictureBox = previous;
                }
                else
                {
                    foreach (Point p in lineEnds)
                    {
                        // double distanceToCenter = CalculateDistance(p, prevPictureBox);
                        Point closestPoint = FindClosestPoint(pictureBoxCenters, p, visited_line_ends);
                        if (closestPoint.X != -1 && closestPoint.Y != -1)
                        {
                            // MessageBox.Show("Found end !");
                            visited_line_ends[p] = true;
                            previous = prevPictureBox;
                            prevPictureBox = closestPoint;
                            visited[closestPoint] = true;

                            Point closestTextBox = FindClosestPoint(textBoxCenters, prevPictureBox, visited_textboxes);
                            string txt = "";
                            if (closestTextBox.X != -1 && closestTextBox.Y != -1)
                            {
                                visited_textboxes[closestTextBox] = true;
                                // for in textBoxCenters to find index in textBoxValues or use dictonary
                                txt = textBoxValues[closestTextBox];
                                string result = neo4jStructure.FirstOrDefault(x => x == txt);
                                string result1 = exclude.FirstOrDefault(x => x == txt);
                                if (result == null && result1 == null)
                                {
                                    neo4jStructure.Add(txt);
                                    //MessageBox.Show("Found begin !" + txt);
                                    string entity = txt[0] + ":";
                                    if (entity != "R:" && entity != "P:" && entity != "C:")
                                    {
                                        //string promptValue = Prompt.ShowDialog("Value: ", "Enter value for " + txt.Replace(entity, ""));
                                        //neo4jValues.Add(promptValue);
                                        neo4jValues.Add(alphabet[indexAlphabet++].ToString());
                                    }
                                    else
                                    {
                                        neo4jValues.Add("");
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
                done = true;
                foreach (var pictureBoxCenter in pictureBoxCenters)
                {
                    if (visited.ContainsKey(pictureBoxCenter) == false)
                    {
                        done = false;
                        break;
                    }
                }
            }
            for(int j = 0; j < neo4jStructure.Count; j++)
            {
                neo4jStructure[j] = ExcludeDigitsFromString(neo4jStructure[j]);
            }
            string prevEntity = ""; string last1 = neo4jStructure[neo4jStructure.Count - 1]; string last2 = neo4jValues[neo4jValues.Count - 1];
            neo4jStructure.RemoveAt(neo4jStructure.Count - 1); neo4jValues.RemoveAt(neo4jValues.Count - 1);
            for (int j = 0; j < neo4jStructure.Count; j++)
            {
                if (neo4jStructure[j].StartsWith("N:")) prevEntity = neo4jStructure[j];
                if (neo4jStructure[j].StartsWith("R:"))
                {
                    string nextEntity = neo4jStructure[j + 1];
                    string nextEntityValue = neo4jValues[j + 1];
                    var relations = neo4jStructure[j].Split(",");
                    neo4jStructure[j] = relations[0];
                    for(int z = 1; z < relations.Count(); z++) {
                        InsertAndShiftRight(neo4jStructure, j + z + 1, prevEntity);
                        //neo4jStructure.Add(prevEntity);
                        InsertAndShiftRight(neo4jValues, j + z + 1, alphabet[indexAlphabet++].ToString());
                        //neo4jValues.Add(alphabet[indexAlphabet++].ToString());
                        InsertAndShiftRight(neo4jStructure, j + z + 2, relations[z]);
                        //neo4jStructure.Add(relations[z]);
                        InsertAndShiftRight(neo4jValues, j + z + 2, "");
                        //neo4jValues.Add("");
                        InsertAndShiftRight(neo4jStructure, j + z + 3, nextEntity);
                        //neo4jStructure.Add(nextEntity);
                        InsertAndShiftRight(neo4jValues, j + z + 3, nextEntityValue);
                        //neo4jValues.Add(nextEntityValue);
                    }
                }
            }
            neo4jStructure.Add(last1);
            neo4jValues.Add(last2);
            for(int z = 0; z < neo4jStructure.Count; z++)
            {
                if (neo4jStructure[z].StartsWith("N:")) neo4jStructure[z] = neo4jStructure[z].Replace("N:", "");
                else if (neo4jStructure[z].StartsWith("P:")) neo4jStructure[z] = neo4jStructure[z].Replace("P:", "");
                else if (neo4jStructure[z].StartsWith("C:")) neo4jStructure[z] = neo4jStructure[z].Replace("C:=", "");
                else if (neo4jStructure[z].StartsWith("R:")) neo4jStructure[z] = neo4jStructure[z].Replace("R:","");
            }
            string matchQuery = "MATCH "; List<string> visitedAlphabetLetters = new List<string>(); bool wasRelation = false; int lastIndex = 0;
            string firstEntity = ""; string lastEntityValue = "";
            for (int z = 0; z < neo4jStructure.Count -1; z++)
            {
                if (neo4jValues[z] != "")
                {
                    if (firstEntity == "") firstEntity = neo4jStructure[z];
                    if (firstEntity == neo4jStructure[z]) lastEntityValue = neo4jValues[z];
                    if (matchQuery.Contains(neo4jStructure[z]) && visitedAlphabetLetters.Contains(neo4jValues[z]))
                    {
                        matchQuery += "(" + neo4jValues[z] + ")";
                    } else matchQuery += "(" + neo4jValues[z] + ":" + neo4jStructure[z] + ")";
                    if (wasRelation)
                    {
                        matchQuery += ",";
                        wasRelation = false;
                    }
                    visitedAlphabetLetters.Add(neo4jValues[z]);
                } else if(neo4jStructure[z].All(char.IsLower))
                {
                    matchQuery = matchQuery.Remove(matchQuery.Count()-1);
                    matchQuery += "{";
                    while (neo4jStructure[z].All(char.IsLower) && z < neo4jStructure.Count-2)
                    {
                        matchQuery += neo4jStructure[z] + ":" + neo4jStructure[z + 1];
                        z++;
                    }
                    matchQuery+= "})";
                } else if(neo4jStructure[z].Replace("_", "").All(char.IsUpper))
                {
                    matchQuery += "-[:" + neo4jStructure[z] + "]->";
                    wasRelation = true;
                }
                lastIndex = z;
            }
            matchQuery = matchQuery.Remove(matchQuery.Count() - 1);
            matchQuery += " RETURN " + lastEntityValue + "." + neo4jStructure[lastIndex+1];
            MessageBox.Show(matchQuery);

            //_ = connectToNeo4j(matchQuery, neo4jStructure[lastIndex + 1]);
            /*string combinedString1 = string.Join(",", neo4jStructure.ToArray());
            string combinedString2 = string.Join(",", neo4jValues.ToArray());
            MessageBox.Show(combinedString1);
            MessageBox.Show(combinedString2);
            MessageBox.Show(alphabet[indexAlphabet-1].ToString());*/

            var uri = "bolt://localhost:7687";
            var user = "neo4j";
            var password = "neo4jneo4jneo4j";
            IDriver _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));

            try
            {
                await ExecuteReadQueryAsync(matchQuery,_driver, lastEntityValue + "." + neo4jStructure[lastIndex + 1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                await _driver.CloseAsync();
            }
        }

        private static async Task ExecuteReadQueryAsync(string query,IDriver driver, string res)
        {
            var session = driver.AsyncSession();
            try
            {
                List<IRecord> result = await session.ReadTransactionAsync(async tx => { IResultCursor cursor = await tx.RunAsync(query); return await cursor.ToListAsync(); });

                foreach (var record in result)
                {
                    MessageBox.Show("Query result: "+ record[res].As<string>());
                }

                if (result.Count() == 0)
                {
                    MessageBox.Show("No records found.");
                }
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        private void InsertAndShiftRight(List<string> list, int position, string newElement)
        {
            list.Add("");
            for (int i = list.Count - 2; i >= position; i--)
            {
                list[i + 1] = list[i];
            }
            list[position] = newElement;
        }

        private string ExcludeDigitsFromString(string input)
        {
            var regexNonDigits = new Regex(@".*(?=\d+$)");
            var match = regexNonDigits.Match(input);
            return match.Success ? match.Value : input;
        }

        /* private void DisplayOperationHistory()
        {
            List<string> historyMessages = new List<string>();

            foreach (var line in lines)
            {
                string message = $"Line from ({line.Item1.X}, {line.Item1.Y}) to ({line.Item2.X}, {line.Item2.Y})";
                historyMessages.Add(message);
            }
            int index = 0;
            foreach (var control in addedControls)
            {
                if (control is PictureBox pictureBox)
                {
                    string message = $"{pictureBoxNames[index]} at ({pictureBox.Location.X}, {pictureBox.Location.Y})";
                    historyMessages.Add(message);
                    index++;
                }
                else if (control is System.Windows.Forms.TextBox textBox)
                {
                    string message = $"TextBox at ({textBox.Location.X}, {textBox.Location.Y})";
                    historyMessages.Add(message);
                }
            }

            if (historyMessages.Count == 0)
            {
                historyMessages.Add("No relationships recorded.");
            }

            string historyMessage = string.Join(Environment.NewLine, historyMessages);
            MessageBox.Show(historyMessage, "Operation History", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }*/

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = @"C:\";
            SaveFileDialog1.Title = "Browse Text Files";
            SaveFileDialog1.DefaultExt = "txt";
            List<string> historyMessages = new List<string>();

            foreach (var line in lines)
            {
                string message = $"Line {line.Item1.X}, {line.Item1.Y} to {line.Item2.X}, {line.Item2.Y}";
                historyMessages.Add(message);
            }
            int index = 0;
            foreach (var control in addedControls)
            {
                if (control is PictureBox pictureBox)
                {
                    string message = $"{pictureBoxNames[index]} {pictureBox.Location.X}, {pictureBox.Location.Y}";
                    historyMessages.Add(message);
                    index++;
                }
                else if (control is System.Windows.Forms.TextBox textBox)
                {
                    string message = $"TextBox {textBox.Location.X}, {textBox.Location.Y} {textBox.Text}";
                    historyMessages.Add(message);
                }
            }

            if (historyMessages.Count == 0)
            {
                historyMessages.Add("No relationships recorded.");
            }

            string historyMessage = string.Join(Environment.NewLine, historyMessages);
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog1.FileName))
                {
                    writer.Write(historyMessage);
                }
            }
        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "Browse Text Files";
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.ShowDialog();

            string filePath = openFileDialog1.FileName;
            if (File.Exists(filePath))
            {
                // Clear existing lines and controls
                this.lines.Clear();
                foreach (Control control in addedControls)
                {
                    control.Dispose();
                }
                addedControls.Clear();
                operationHistory.Clear();
                Invalidate();
                pictureBoxNames.Clear(); // Clear existing picture box names

                string[] fileLines = File.ReadAllLines(filePath);
                foreach (string line in fileLines)
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length >= 2)
                    {
                        string shapeType = parts[0];
                        int x1 = int.Parse(parts[1].Split(',')[0]);
                        int y1 = int.Parse(parts[2].Split(',')[0]);
                        int x2, y2; x2 = y2 = 0;
                        if (parts.Length > 3)
                        {
                            try
                            {
                                x2 = int.Parse(parts[4].Split(',')[0]);
                                y2 = int.Parse(parts[5].Split(',')[0]);
                            }
                            catch
                            {

                            }
                        }

                        if (shapeType.Equals("Line", StringComparison.OrdinalIgnoreCase))
                        {
                            this.lines.Add(Tuple.Create(new Point(x1, y1), new Point(x2, y2)));
                        }
                        else
                        {
                            if (shapeType != "TextBox")
                            {
                                PictureBox pictureBox = new PictureBox
                                {
                                    Image = Image.FromFile($".\\{shapeType}.png"),
                                    SizeMode = PictureBoxSizeMode.Zoom,
                                    Location = new Point(x1, y1),
                                    Size = new Size(100, 100)
                                };
                                Controls.Add(pictureBox);
                                addedControls.Add(pictureBox);
                                pictureBoxNames.Add(shapeType); // Add the picture box name
                            }
                            else
                            {
                                System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox
                                {
                                    Text = parts[3],
                                    Location = new Point(x1, y1),
                                    Size = new Size(100, 30)
                                };
                                // textBox.BringToFront();
                                Controls.Add(textBox);
                                addedControls.Add(textBox);
                                Controls.SetChildIndex(textBox, 0);
                            }
                        }
                    }
                }

                // Redraw lines
                Invalidate();
                //MessageBox.Show("Shapes and lines loaded from file successfully!", "Load Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}