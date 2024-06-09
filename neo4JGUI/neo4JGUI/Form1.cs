using Neo4j.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Point = System.Drawing.Point;

namespace neo4JGUI
{
    public partial class Form1 : Form
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

        public Form1()
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

        private void Form1_Click(object sender, EventArgs e)
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
                if(clickedPictureBox == null)
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

        private void Form1_Paint(object sender, PaintEventArgs e)
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
                        if(controlToRemove is PictureBox)
                        {
                            pictureBoxNames.RemoveAt(pictureBoxNames.Count - 1);
                        }
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

        private Point FindClosestPoint(List<Point> points, Point p, Dictionary<Point,bool> visited)
        {

            double minDistance = double.MaxValue;
            Point closestPoint = new Point(-1,-1);

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
            Dictionary<Point,string> textBoxValues = new Dictionary<Point,string>();
            foreach (var line in lines)
            {
                lineStarts.Add(new Point(line.Item1.X,line.Item1.Y));
                lineEnds.Add(new Point(line.Item2.X, line.Item2.Y));
            }

            foreach (var control in addedControls)
            {
                if (control is PictureBox pictureBox)
                {
                    Point pictureBoxCenter = new Point(pictureBox.Left + pictureBox.Width / 2, pictureBox.Top + pictureBox.Height / 2);
                    pictureBoxCenters.Add(pictureBoxCenter);
                }
                else if (control is System.Windows.Forms.TextBox textBox)
                {
                    Point textBoxCenter = new Point(textBox.Left + textBox.Width / 2, textBox.Top + textBox.Height / 2);
                    textBoxCenters.Add(textBoxCenter);
                    textBoxValuesList.Add(textBox.Text);
                }
            }

            int i = 0;
            foreach(var textBoxCenter in textBoxCenters)
            {
                textBoxValues[textBoxCenter] = textBoxValuesList[i];
                i++;
            }

            bool done = false;
            var prevPictureBox = pictureBoxCenters[0];
            var previous = pictureBoxCenters[0];
            Dictionary<Point, bool> visited = new Dictionary<Point, bool>();
            Dictionary<Point, bool> visited_line_starts = new Dictionary<Point, bool>();
            Dictionary<Point, bool> visited_line_ends = new Dictionary<Point, bool>();
            Dictionary<Point, bool> visited_textboxes = new Dictionary<Point, bool>();
            List<string> neo4jValues = new List<string>();
            List<string> neo4jStructure = new List<string>();
            List<string> exclude = new List<string>() { "isOptional", "hasOne", "hasMore", "isSource", "isTarget" };
            visited[prevPictureBox] = true;
            while(!done)
            {
                bool foundLineStart = false;
                foreach (Point p in lineStarts)
                {   
                    Point closestPoint = FindClosestPoint(pictureBoxCenters, p, visited_line_starts);
                    if(closestPoint.X != -1 && closestPoint.Y != -1)
                    {
                        Point closestTextBox = FindClosestPoint(textBoxCenters, previous, visited_textboxes);
                        string txt = "";
                        if (closestTextBox.X != -1 && closestTextBox.Y != -1)
                        {
                            visited_textboxes[closestTextBox] = true;
                            txt = textBoxValues[closestTextBox];
                            string result = neo4jStructure.FirstOrDefault(x => x == txt);
                            string result1 = exclude.FirstOrDefault(x => x == txt);
                            if (result == null && result1 == null)
                            {
                                neo4jStructure.Add(txt);
                                string entity = txt[0]+":";
                                if (entity != "R:")
                                {
                                    string promptValue = Prompt.ShowDialog("Value: ", "Enter value for " + txt.Replace(entity, ""));
                                    neo4jValues.Add(promptValue);
                                } else
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
                } else
                {
                    foreach (Point p in lineEnds)
                    {
                        Point closestPoint = FindClosestPoint(pictureBoxCenters, p, visited_line_ends);
                        if (closestPoint.X != -1 && closestPoint.Y != -1)
                        {
                            visited_line_ends[p] = true;
                            previous = prevPictureBox;
                            prevPictureBox = closestPoint;
                            visited[closestPoint] = true;

                            Point closestTextBox = FindClosestPoint(textBoxCenters, prevPictureBox, visited_textboxes);
                            string txt = "";
                            if (closestTextBox.X != -1 && closestTextBox.Y != -1)
                            {
                                visited_textboxes[closestTextBox] = true;
                                txt = textBoxValues[closestTextBox];
                                string result = neo4jStructure.FirstOrDefault(x => x == txt);
                                string result1 = exclude.FirstOrDefault(x => x == txt);
                                if (result == null && result1 == null)
                                {
                                    neo4jStructure.Add(txt);
                                    string entity = txt[0] + ":";
                                    if (entity != "R:") {
                                        string promptValue = Prompt.ShowDialog("Value: ", "Enter value for " + txt.Replace(entity, ""));
                                        neo4jValues.Add(promptValue);
                                    } else
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
                foreach(var pictureBoxCenter in pictureBoxCenters)
                {
                    if(visited.ContainsKey(pictureBoxCenter) == false)
                    {
                        done = false;
                        break;
                    }
                }
            }

            string cypherQuery = ""; bool isRelation = false;
            for (int index = 0; index < neo4jStructure.Count; index++)
            {
                if (neo4jStructure[index].StartsWith("N:"))
                {   
                    if(index != 0)
                    {
                        cypherQuery = cypherQuery.Remove(cypherQuery.Length - 2);
                        cypherQuery += "});";
                    }
                    cypherQuery += "CREATE(";
                    cypherQuery += neo4jValues[index] + ":" + neo4jStructure[index].Replace("N:", "") + " { ";
                    isRelation = false;
                } else
                {
                    if (neo4jValues[index] != "")
                    {
                        if (isRelation == false) cypherQuery += neo4jStructure[index].Replace("P:", "") + ": '" + neo4jValues[index] + "', ";
                    } else
                    {
                        isRelation = true;
                    }
                }
            }
            cypherQuery = cypherQuery.Remove(cypherQuery.Length - 2);
            cypherQuery += "});";
            List<string> entities = new List<string>();
            List<string> relations = new List<string>();
            List<string> properties = new List<string>();
            List<int> relationBeginEntityIndex = new List<int>();
            int indexEntity = -1;
            for (int index = 0; index < neo4jStructure.Count; index++)
            {   
                if (neo4jStructure[index].StartsWith("N:"))
                {
                    entities.Add(neo4jValues[index]);
                    indexEntity++;
                }
                if (neo4jStructure[index].StartsWith("R:"))
                {   
                    while(index < neo4jStructure.Count && neo4jStructure[index].StartsWith("R:"))
                    {
                        relations.Add(neo4jStructure[index].Replace("R:", ""));
                        relationBeginEntityIndex.Add(indexEntity);
                        index++;
                    }
                    while(index < neo4jStructure.Count && neo4jStructure[index].StartsWith("P:"))
                    {
                        properties.Add(neo4jStructure[index].Replace("P:","")+":['"+neo4jValues[index]+"']");
                        index++;
                    }
                    index--;
                }
            }
            
            char[] alphabet = new char[26];
            for (int j = 0; j < 26; j++)
            {
                alphabet[j] = (char)('a' + j);
            }
            int indexAlphabet = 0;
            if (relations.Count > 0)
            {
                string matchStatement = cypherQuery.Replace("CREATE", "MATCH").Replace(";", " ");
                cypherQuery = cypherQuery.Replace(";", " ");
                cypherQuery += ";";
                cypherQuery += matchStatement;

                for (int index = 0; index < relations.Count; index++)
                {
                    int indexBeginEntity = relationBeginEntityIndex[index];
                    string entity1 = entities[indexBeginEntity];
                    string entity2 = entities[indexBeginEntity + 1];
                    string property = "";
                    try
                    {
                        property = properties[index];
                    }
                    catch {}
                    if (property != "")
                    {
                        cypherQuery += "MERGE(" + entity1 + ")-[" + alphabet[indexAlphabet++] + ":" + relations[index] + " {" + property + "}]->(" + entity2 + ") ";
                    }
                    else
                    {
                        cypherQuery += "MERGE(" + entity1 + ")-[" + alphabet[indexAlphabet++] + ":" + relations[index] + "]->(" + entity2 + ") ";
                    }
                }
                cypherQuery = cypherQuery.Remove(cypherQuery.Length - 1);
            }
            cypherQuery += ";";
            MessageBox.Show(cypherQuery);
            var cypherQueries = cypherQuery.Split(";");
            for(int z = 0; z < cypherQueries.Count() - 1; z++)
            {
                _ = connectToNeo4j(cypherQueries[z]);
            }
        }

        private async Task connectToNeo4j(string cypherQuery)
        {
            var uri = "neo4j://localhost:7687";
            var user = "neo4j";
            var password = "neo4jneo4jneo4j";

            var driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            try
            {
                await InsertDataAsync(driver, cypherQuery);
            }
            finally
            {
                await driver.CloseAsync();
            }
        }

        private async Task InsertDataAsync(IDriver driver, string query)
        {
            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

            try
            {
                await session.WriteTransactionAsync(async tx => { await tx.RunAsync(query); });
                MessageBox.Show("Data inserted successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally
            {
                await session.CloseAsync();
            }
        }

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
                            } catch
                            {

                            }
                        }

                        if (shapeType.Equals("Line", StringComparison.OrdinalIgnoreCase))
                        {
                            this.lines.Add(Tuple.Create(new Point(x1, y1), new Point(x2, y2)));
                        }
                        else
                        {
                            if (shapeType != "TextBox"){
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
                            } else
                            {
                                System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox
                                {
                                    Text = parts[3],
                                    Location = new Point(x1, y1),
                                    Size = new Size(100, 30)
                                };
                                Controls.Add(textBox);
                                addedControls.Add(textBox);
                                Controls.SetChildIndex(textBox, 0);
                            }
                        }
                    }
                }

                // Redraw lines
                Invalidate();
            }
            else
            {
                MessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}