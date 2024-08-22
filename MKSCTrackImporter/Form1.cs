using AdvancedLib;
using AdvancedLib.Serialize;
using AdvancedLib.Types;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace MKSCTrackImporter
{
    public partial class Form1 : Form
    {
        private Manager manager;
        private Track selectedTrack;

        public Form1()
        {
            InitializeComponent();
            manager = new Manager(Program.loggingEnabled);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            labelRomOpened.Text = "ROM not loaded";
            labelRomOpened.ForeColor = Color.Maroon;

            openFileDialog.Filter = "GBA Files|*.gba|All Files|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
#if !DEBUG
                try
                {
#endif
                    manager.Open(openFileDialog.FileName);
#if !DEBUG
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.ToString()}", "Error opening file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelRomOpened.Text = "ROM not loaded";
                    labelRomOpened.ForeColor = Color.Maroon;

                    return;
                }
#endif
                labelRomOpened.Text = "Deserializing ROM";
                labelRomOpened.ForeColor = Color.Orange;
#if !DEBUG
                try
                {
#endif
                    manager.Deserialize();
#if !DEBUG
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.ToString()}", "Error reading file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelRomOpened.Text = "ROM not loaded";
                    labelRomOpened.ForeColor = Color.Maroon;

                    return;
                }
#endif
                labelRomOpened.Text = "Loaded Sucessfully";
                labelRomOpened.ForeColor = Color.DarkGreen;
                editorPanel.Enabled = true;
            }
        }
        private void tilemapExport_Click(object sender, EventArgs e)
        {
            #region Track selected check
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion
            #region Extract track data
            var trackName = String.Join("", trackNames[trackSelector.SelectedIndex].Split(' '));
            var trackWidth = selectedTrack.TrackWidth * 128;
            var trackHeight = selectedTrack.TrackHeight * 128;
            string csvData = string.Join("\n", Enumerable.Range(0, trackWidth)
                .Select(y => string.Join(",", Enumerable.Range(0, trackHeight)
                    .Select(x => (selectedTrack.Layout.indicies[x + y * trackWidth] + 1).ToString())) + ","));
            #endregion
            #region XML Layout
            int objID = 1;
            XElement tilemapData = new XElement("map",
                new XAttribute("version", "1.10"), new XAttribute("tiledversion", "1.11.0"), new XAttribute("orientation", "orthogonal"), new XAttribute("renderorder", "right-down"),
                new XAttribute("width", trackWidth), new XAttribute("height", trackHeight),
                new XAttribute("tilewidth", "8"), new XAttribute("tileheight", "8"), new XAttribute("infinite", "0"),
                new XElement("tileset", new XAttribute("firstgid", "1"), new XAttribute("source", $"{trackName}.tsx")),
                new XElement("layer",
                    new XAttribute("id", "1"), new XAttribute("name", "Tilemap"),
                    new XAttribute("width", trackWidth), new XAttribute("height", trackHeight),

                    new XElement("data", new XAttribute("encoding", "csv"),
                        csvData
                    )
                ),
                new XElement("objectgroup", new XAttribute("id", "1"), new XAttribute("name", "Start Line")

                ),
                new XElement("objectgroup", new XAttribute("id", "2"), new XAttribute("name", "AI Zones")

                ),
                new XElement("objectgroup", new XAttribute("id", "3"), new XAttribute("name", "AI Targets set 1")

                ),
                new XElement("objectgroup", new XAttribute("id", "4"), new XAttribute("name", "AI Targets set 2")

                ),
                new XElement("objectgroup", new XAttribute("id", "5"), new XAttribute("name", "AI Targets set 3")

                )
            );
            #endregion
            #region Start line
            var startLineGroup = tilemapData.Elements("objectgroup").First(o => o.Attribute("name").Value == "Start Line");

            string[] names = {
                "1st start position",
                "2nd start position",
                "3rd start position",
                "4th start position",
                "5th start position",
                "6th start position",
                "7th start position",
                "8th start position",
                "Multi-pak 1st start position",
                "Multi-pak 2nd start position",
                "Finish line top left",
            };

            foreach (GameObject o in selectedTrack.FinishLine.GameObjects)
            {
                startLineGroup.Add(
                    new XElement("object", new XAttribute("id", objID++),
                        new XAttribute("x", o.X * 8),
                        new XAttribute("y", o.Y * 8),
                        new XElement("properties",
                            new XElement("property",
                                new XAttribute("name", "type"),
                                new XAttribute("type", "int"),
                                new XAttribute("value", o.Id)
                            ),
                            new XElement("property",
                                new XAttribute("name", "zone"),
                                new XAttribute("type", "int"),
                                new XAttribute("value", o.Zone)
                            ),
                            new XElement("property",
                                new XAttribute("name", "name"),
                                new XAttribute("type", "string"),
                                new XAttribute("value", names[Math.Clamp(o.Id - 0x81,0, 20)])
                            )
                        ),
                        new XElement("point")
                    )
                );
            }
            #endregion
            #region AI
            var zonesGroup = tilemapData.Elements("objectgroup").First(o => o.Attribute("name").Value == "AI Zones");
            var targetsGroup1 = tilemapData.Elements("objectgroup").First(o => o.Attribute("name").Value == "AI Targets set 1");
            var targetsGroup2 = tilemapData.Elements("objectgroup").First(o => o.Attribute("name").Value == "AI Targets set 2");
            var targetsGroup3 = tilemapData.Elements("objectgroup").First(o => o.Attribute("name").Value == "AI Targets set 3");
            int targetCount = 512;

            for (int i = 0; i < selectedTrack.TrackAI.Targets.Length / 3; i++)
            {
                AiTarget o = selectedTrack.TrackAI.Targets[i];
                targetsGroup1.Add(
                    new XElement("object", new XAttribute("id", targetCount++),
                        new XAttribute("type", "Target"),
                        new XAttribute("x", o.X * 8),
                        new XAttribute("y", o.Y * 8),
                        new XElement("properties",
                            new XElement("property",
                                new XAttribute("name", "intersection"),
                                new XAttribute("type", "bool"),
                                new XAttribute("value", (o.Intersection == 0) ? "false" : "true")
                            ),
                            new XElement("property",
                                new XAttribute("name", "speed"),
                                new XAttribute("type", "int"),
                                new XAttribute("value", o.Speed)
                            )
                        ),
                        new XElement("point")
                    )
                );
            }
            for (int i = selectedTrack.TrackAI.Targets.Length / 3; i < 2 * (selectedTrack.TrackAI.Targets.Length / 3); i++)
            {
                AiTarget o = selectedTrack.TrackAI.Targets[i];
                targetsGroup2.Add(
                    new XElement("object", new XAttribute("id", targetCount++),
                        new XAttribute("type", "Target"),
                        new XAttribute("x", o.X * 8),
                        new XAttribute("y", o.Y * 8),
                        new XElement("properties",
                            new XElement("property",
                                new XAttribute("name", "intersection"),
                                new XAttribute("type", "bool"),
                                new XAttribute("value", (o.Intersection == 0) ? "true" : "false")
                            ),
                            new XElement("property",
                                new XAttribute("name", "speed"),
                                new XAttribute("type", "int"),
                                new XAttribute("value", o.Speed)
                            )
                        ),
                        new XElement("point")
                    )
                );
            }
            for (int i = 2 * (selectedTrack.TrackAI.Targets.Length / 3); i < selectedTrack.TrackAI.Targets.Length; i++)
            {
                AiTarget o = selectedTrack.TrackAI.Targets[i];
                targetsGroup3.Add(
                    new XElement("object", new XAttribute("id", targetCount++),
                        new XAttribute("type", "Target"),
                        new XAttribute("x", o.X * 8),
                        new XAttribute("y", o.Y * 8),
                        new XElement("properties",
                            new XElement("property",
                                new XAttribute("name", "intersection"),
                                new XAttribute("type", "bool"),
                                new XAttribute("value", (o.Intersection == 0) ? "true" : "false")
                            ),
                            new XElement("property",
                                new XAttribute("name", "speed"),
                                new XAttribute("type", "int"),
                                new XAttribute("value", o.Speed)
                            )
                        ),
                        new XElement("point")
                    )
                );
            }

            int zoneCount = 1024;
            foreach (AiZone o in selectedTrack.TrackAI.Zones)
            {
                string points = "";
                switch (o.Shape)
                {
                    case 1:
                        points = $"0,0 {o.Width * 8},0 0,{o.Width * 8}";
                        break;
                    case 2:
                        points = $"0,0 {-o.Width * 8},0 0,{o.Width * 8}";
                        break;
                    case 4:
                        points = $"0,0 {o.Width * 8},0 0,{-o.Width * 8}";
                        break;
                    case 3:
                        points = $"0,0 {-o.Width * 8},0 0,{-o.Width * 8}";
                        break;
                    default: break;

                }
                if (o.Shape == 0)
                {
                    zonesGroup.Add(
                        new XElement("object", new XAttribute("id", zoneCount++),
                            new XAttribute("type", "Zone"),
                            new XAttribute("x", o.X * 8),
                            new XAttribute("y", o.Y * 8),
                            new XAttribute("width", o.Width * 8),
                            new XAttribute("height", o.Height * 8)
                        /* This is confusing because its just for looks,
                        new XElement("property",
                            new XAttribute("name", "DISPLAY ONLY TARGET"),
                            new XAttribute("type", "object"),
                            new XAttribute("value", zoneCount-512)
                        )*/

                        )
                    );
                } else
                {
                    zonesGroup.Add(
                        new XElement("object", new XAttribute("id", zoneCount++),
                            new XAttribute("type", "Zone"),
                            new XAttribute("x", o.X * 8),
                            new XAttribute("y", o.Y * 8),
                            new XElement("polygon",
                                new XAttribute("points", points))
                        /*,
                            new XElement("property",
                                new XAttribute("name", "DISPLAY ONLY TARGET"),
                                new XAttribute("type", "object"),
                                new XAttribute("value", zoneCount - 512)
                            )*/

                        )
                    );
                }

            }
        #endregion
        #region Saving
        SaveFile:
            saveFileDialog.DefaultExt = "tmx";
            saveFileDialog.Filter = "TMX Files|*.tmx|All Files|*.*";
            saveFileDialog.FileName = trackName + ".tmx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    tilemapData.Save(saveFileDialog.OpenFile());
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show($"Error exporting tilemap: {ex}", "Export Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        goto SaveFile;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                if (MessageBox.Show("Please select a save location", "Export Aborted", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                {
                    goto SaveFile;
                }
                else
                {
                    return;
                }
            }
            MessageBox.Show("Sucessfully exported tilemap", "Sucess", MessageBoxButtons.OK, MessageBoxIcon.Information);
            #endregion
        }
        private void tilesetExport_Click(object sender, EventArgs e)
        {
            #region Track selected check
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion
            #region Tilset image generation
            var trackName = String.Join("", trackNames[trackSelector.SelectedIndex].Split(' '));

            Bitmap image = new Bitmap(16 * 8, 16 * 8, PixelFormat.Format8bppIndexed);

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            IntPtr ptr = bmpData.Scan0;
            int stride = bmpData.Stride;
            int bytes = stride * image.Height;
            byte[] pixelData = new byte[bytes];

            Marshal.Copy(ptr, pixelData, 0, bytes);
            if (selectedTrack.TilesetLookback == 0)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                pixelData[(y * 8 + i) * stride + (x * 8 + j)] = selectedTrack.Tileset.tiles[y * 16 + x].indicies[j, i];
                            }
                        }
                    }
                }
            }
            else
            {
                var lookbackTrack = manager.trackManager.tracks[trackSelector.SelectedIndex + selectedTrack.TilesetLookback];
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                pixelData[(y * 8 + i) * stride + (x * 8 + j)] = lookbackTrack.Tileset.tiles[y * 16 + x].indicies[j, i];
                            }
                        }
                    }
                }
            }

            Marshal.Copy(pixelData, 0, ptr, bytes);
            image.UnlockBits(bmpData);
            var pal = image.Palette;
            for (int i = 0; i < selectedTrack.Palette.paletteLength; i++)
            {
                pal.Entries[i] = selectedTrack.Palette[i].ToColor();
            }
            for (int i = selectedTrack.Palette.paletteLength; i < 256; i++)
            {
                pal.Entries[i] = Color.Black;
            }
            image.Palette = pal;
        #endregion
        #region Save files
        SavePNG:
            saveFileDialog.DefaultExt = "png";
            saveFileDialog.Filter = "PNG Files|*.png|All Files|*.*";

            saveFileDialog.FileName = trackName + ".png";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show($"Error saving tilemap {ex}", "Export Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        goto SavePNG;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                if (MessageBox.Show("Please select a save location", "Export Aborted", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                {
                    goto SavePNG;
                }
                else
                {
                    return;
                }
            }
            MessageBox.Show("Sucessfully exported tileset image", "Sucess", MessageBoxButtons.OK, MessageBoxIcon.Information);
        SaveTSX:
            saveFileDialog.DefaultExt = "tsx";
            saveFileDialog.Filter = "TSX Files|*.tsx|All Files|*.*";
            saveFileDialog.FileName = trackName + ".tsx";

            XElement tilesetData = new XElement("tileset",
                new XAttribute("version", "1.10"), new XAttribute("tiledversion", "1.11.0"), new XAttribute("name", trackName),
                new XAttribute("tilewidth", "8"), new XAttribute("tileheight", "8"), new XAttribute("tilecount", "256"), new XAttribute("columns", "16"),
                new XElement("image",
                    new XAttribute("source", trackName + ".png"), new XAttribute("width", "128"), new XAttribute("height", "128")
                )
            );
            for (int i = 0; i < 256; i++)
            {
                tilesetData.Add(
                    new XElement("tile", new XAttribute("id", i.ToString()),
                        new XElement("properties",
                            new XElement("property",
                                new XAttribute("name", "behavior"),
                                new XAttribute("type", "int"),
                                new XAttribute("value", selectedTrack.TileBehaviors[i])
                            )
                        )
                    )
                );
            }
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    tilesetData.Save(saveFileDialog.OpenFile());
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show($"Error saving tilemap {ex}", "Export Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        goto SaveTSX;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                if (MessageBox.Show("Please select a save location", "Export Aborted", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                {
                    goto SaveTSX;
                }
                else
                {
                    return;
                }
            }
            MessageBox.Show("Sucessfully exported tileset", "Sucess", MessageBoxButtons.OK, MessageBoxIcon.Information);
            #endregion
        }
        private void tilemapImport_Click(object sender, EventArgs e)
        {
            #region Track selected check
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion
            #region Import file
            openFileDialog.Filter = "TMX Files|*.tmx|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                XDocument tilemap;

#if !DEBUG
                try
                {
#endif

                    tilemap = XDocument.Load(openFileDialog.FileName);

#if !DEBUG
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.ToString()}", "Error reading file",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
#endif

                XElement? root = tilemap.Root;

#pragma warning disable CS8602
                if ((root.Name == "map")
                    && (root.Attribute("orientation").Value == "orthogonal")
                    && (root.Attribute("tilewidth").Value == "8")
                    && (root.Attribute("tileheight").Value == "8")
                    && (root.Attribute("infinite").Value == "0")
                    )
                {
                    XElement tilemapLayer = root.Descendants("layer").First(o => o.Attribute("name").Value == "Tilemap");
                    var trackWidth = Int32.Parse(tilemapLayer.Attribute("width").Value) / 128;
                    var trackHeight = Int32.Parse(tilemapLayer.Attribute("height").Value) / 128;
                    selectedTrack.TrackHeight = (byte)trackHeight;
                    selectedTrack.TrackWidth = (byte)trackWidth;
                    XElement data = root.Descendants("data").First(o => o.Attribute("encoding").Value == "csv");
                    XElement startPositions = root.Descendants("objectgroup").First(o => o.Attribute("name").Value == "Start Line");
                    var split = data.Value.Split(',');
                    byte[] trackData = split.Take(split.Length - 1).Select(s => (byte)(int.Parse(s) - 1)).ToArray(); // Convert CSV to byte array
                    selectedTrack.Layout.indicies = trackData;
                
                    #region Finish Line
                        GameObject[] finishGameObjectData = (
                            from o in startPositions.Elements("object").Select(o => o.Element("properties"))
                            select new GameObject(
                                Byte.Parse(o.Elements("property").First(p => p.Attribute("name").Value == "type").Attribute("value").Value),
                                (byte)(Int32.Parse(o.Parent.Attribute("x").Value) / 8),
                                (byte)(Int32.Parse(o.Parent.Attribute("y").Value) / 8),
                                Byte.Parse(o.Elements("property").First(p => p.Attribute("name").Value == "zone").Attribute("value").Value)
                            )).ToArray();
                        selectedTrack.FinishLine.GameObjects = finishGameObjectData;
                        #endregion
                
                    #region Zones
                    XElement zonesElement = root.Descendants("objectgroup").First(o => o.Attribute("name").Value == "AI Zones");
                    int oldLen = selectedTrack.TrackAI.Zones.Length;
                    if (selectedTrack.TrackAI.Zones.Length != zonesElement.Elements("object").Count())
                    {
                        Array.Resize(ref selectedTrack.TrackAI.Zones, zonesElement.Elements("object").Count());
                        if (oldLen < selectedTrack.TrackAI.Zones.Length)
                        {
                            for (int j = oldLen; j < selectedTrack.TrackAI.Zones.Length; j++)
                            {
                                selectedTrack.TrackAI.Zones[j] = new AiZone();
                            }
                        }
                        if (selectedTrack.TrackAI.Zones.Length > 255)
                        {
                            MessageBox.Show($"Tracks cannot have more than 255 zones. Exiting.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        selectedTrack.TrackAI.zonesCount = (byte)selectedTrack.TrackAI.Zones.Length;
                    }
                    int i = 0;
                    foreach (var o in zonesElement.Elements("object"))
                    {
                
                        if (o.Element("polygon") != null)
                        {
                            Point[] trianglePoints = (from p in o.Element("polygon").Attribute("points").Value.Split(' ')
                                                      select new Point(Int32.Parse(p.Split(",")[0]), Int32.Parse(p.Split(",")[1]))).ToArray();
                
                            int? cornerIndex = FindCorner(trianglePoints);
                            if (cornerIndex == null)
                                MessageBox.Show($"Error reading track: Invalid triangle zone in AI zones", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                            Point cornerPos = trianglePoints[cornerIndex.Value];
                            trianglePoints = trianglePoints.Select(o => new Point(o.X + cornerPos.X, o.Y + cornerPos.Y)).ToArray();
                
                            trianglePoints[cornerIndex.Value] = trianglePoints[0];
                            trianglePoints[0] = cornerPos;
                
                            int triSize;
                            byte triangleType = TriangleType(trianglePoints, out triSize);
                
                            int adjustedX = (Int32.Parse(o.Attribute("x").Value) / 8) - cornerPos.X;
                            int adjustedY = (Int32.Parse(o.Attribute("y").Value) / 8) - cornerPos.Y;
                
                            selectedTrack.TrackAI.Zones[i].Shape = triangleType;
                            selectedTrack.TrackAI.Zones[i].X = adjustedX;
                            selectedTrack.TrackAI.Zones[i].Y = adjustedY;
                            selectedTrack.TrackAI.Zones[i].Width = triSize / 8;
                            selectedTrack.TrackAI.Zones[i++].Height = 0;
                        } else {
                            selectedTrack.TrackAI.Zones[i].Shape = 0;
                            selectedTrack.TrackAI.Zones[i].X = Int32.Parse(o.Attribute("x").Value) / 8;
                            selectedTrack.TrackAI.Zones[i].Y = Int32.Parse(o.Attribute("y").Value) / 8;
                            selectedTrack.TrackAI.Zones[i].Width = Int32.Parse(o.Attribute("width").Value) / 8;
                            selectedTrack.TrackAI.Zones[i++].Height = Int32.Parse(o.Attribute("height").Value) / 8;
                        }
                    }
                    #endregion
                
                    #region Targets
                    XElement targets1element = root.Descendants("objectgroup").First(o => o.Attribute("name").Value == "AI Targets set 1");
                    XElement targets2element = root.Descendants("objectgroup").First(o => o.Attribute("name").Value == "AI Targets set 2");
                    XElement targets3element = root.Descendants("objectgroup").First(o => o.Attribute("name").Value == "AI Targets set 3");
                    
                    if ((targets1element.Elements("object").Count() != selectedTrack.TrackAI.Zones.Length) ||
                        (targets2element.Elements("object").Count() != selectedTrack.TrackAI.Zones.Length) ||
                        (targets3element.Elements("object").Count() != selectedTrack.TrackAI.Zones.Length)
                        )
                    {
                        MessageBox.Show($"Each target group must have the same number of targets as there are zones. Exiting.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                
                    var allTargets = targets1element.Elements("object").Concat(targets2element.Elements("object")).Concat(targets3element.Elements("object"));
                    oldLen = selectedTrack.TrackAI.Targets.Length;
                    var newLen = allTargets.Count();
                    if (selectedTrack.TrackAI.Targets.Length != newLen)
                    {
                        Array.Resize(ref selectedTrack.TrackAI.Targets, newLen);
                        if (oldLen < selectedTrack.TrackAI.Targets.Length)
                        {
                            for (int j = oldLen; j < selectedTrack.TrackAI.Targets.Length; j++)
                            {
                                selectedTrack.TrackAI.Targets[j] = new AiTarget();
                            }
                        }
                        selectedTrack.TrackAI.RecalculateSize(); // update pointers and crash sooner so I can debug
                    }
                
                    i = 0;
                    foreach (var o in allTargets)
                    {
                        var props = o.Element("properties").Elements("property");
                        selectedTrack.TrackAI.Targets[i].X = (ushort)(int.Parse(o.Attribute("x").Value)/8);
                        selectedTrack.TrackAI.Targets[i].Y = (ushort)(int.Parse(o.Attribute("y").Value)/8);
                        selectedTrack.TrackAI.Targets[i].Speed = ushort.Parse(props.First(p => p.Attribute("name").Value == "speed").Attribute("value").Value);
                        selectedTrack.TrackAI.Targets[i++].Intersection = (ushort)((props.First(p => p.Attribute("name").Value == "intersection").Attribute("value").Value == "false")?0:8);
                    }
                    #endregion
                
                    MessageBox.Show("Sucessfully imported tilemap", "Sucess", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
#pragma warning restore CS8602
            }
            #endregion
        }
        private static int? FindCorner(Point[] points)
        {
            // This code is so bad but I am too tired for good code and also its probably faster anyways
            if (
                (points[0].X == points[1].X && points[0].Y == points[2].Y) || (points[0].X == points[2].X && points[0].Y == points[1].Y)
                )
                return 0;
            if (
                (points[1].X == points[0].X && points[1].Y == points[2].Y) || (points[1].X == points[2].X && points[1].Y == points[0].Y)
                )
                return 1;
            if (
                (points[2].X == points[1].X && points[2].Y == points[0].Y) || (points[2].X == points[0].X && points[2].Y == points[1].Y)
                )
                return 2;
            return null;
        }
        private static byte TriangleType(Point[] points, out int width)
        {
            // first coord must be 0,0
            if ((points[0].X | points[0].Y) != 0)
                throw new ArgumentException("Whoops I goofed");
            bool top;
            bool left;
            if (points[1].X == 0)
            {
                width = Math.Abs(points[1].Y);
                top = (points[1].Y > 0);
                left = (points[2].X > 0);
            }
            else
            {
                width = Math.Abs(points[1].X);
                left = (points[1].X > 0);
                top = (points[2].Y > 0);
            }
            if (top)
                if (left)
                    return 1;
                else
                    return 2;
            else
                if (left)
                    return 4;
                else 
                    return 3;
        }
        private void tilesetImport_Click(object sender, EventArgs e)
        {
            #region Track selected check
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion
            #region Import file
            openFileDialog.Filter = "TSX Files|*.tsx;|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                XDocument tilesetDoc = XDocument.Load(openFileDialog.FileName);

                var tilesetPath = Path.Combine(Path.GetDirectoryName(openFileDialog.FileName), tilesetDoc.Root.Element("image").Attribute("source").Value);

                byte[] behaviors =
                    (from element in tilesetDoc.Root.Elements("tile")
                     select byte.Parse(element.Element("properties").Elements("property").Where(x => x.Attribute("name").Value == "behavior")
                     .First().Attribute("value").Value)).ToArray();
                selectedTrack.TileBehaviors = behaviors;

                Bitmap tileset = (Bitmap)Image.FromFile(tilesetPath);

                var width = tileset.Width;
                var height = tileset.Height;
                int PixelCount = width * height;
                Rectangle rect = new Rectangle(0, 0, width, height);
                if (tileset.PixelFormat != PixelFormat.Format8bppIndexed)
                {
                    MessageBox.Show("Only 8 bpp paletted images are supported. Check the FAQ for more info", "Image type error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var bitmapData = tileset.LockBits(rect, ImageLockMode.ReadOnly,
                                         tileset.PixelFormat);
                var pixelData = new byte[PixelCount];
                var ptr = bitmapData.Scan0;

                Marshal.Copy(ptr, pixelData, 0, pixelData.Length);

                Tile[] tiles = new Tile[256];
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        byte[,] indicies = new byte[8, 8];
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                indicies[i, j] = Math.Clamp(pixelData[(y * 8 + j) * 128 + (x * 8 + i)], (byte)0, (byte)63);
                            }
                        }
                        tiles[y * 16 + x] = new Tile(indicies);
                    }
                }
                selectedTrack.Tileset.tiles = tiles;
                BgrColor[] pal = new BgrColor[64];
                for (int i = 0; i < 64; i++)
                {
                    var color = tileset.Palette.Entries[i];
                    pal[i] = new BgrColor(color.R, color.G, color.B);
                }

                selectedTrack.Palette.palette = pal;
            }
            #endregion
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                manager.Save(null);
                MessageBox.Show($"Game saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#if DEBUG
                throw;
#endif
                return;
            }
        }
        private void trackSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (trackSelector.SelectedIndex == 20 ||
                trackSelector.SelectedIndex == 21 ||
                trackSelector.SelectedIndex == 22 ||
                trackSelector.SelectedIndex == 23 ||
                trackSelector.SelectedIndex == -1
                )
            {
                trackSelector.SelectedIndex = -1;
                return;
            }
            selectedTrack = manager.trackManager.tracks[trackSelector.SelectedIndex];

            checkBox1.Enabled = false;
            if (selectedTrack.TilesetLookback == 0)
            {
                tilesetImport.Enabled = true;
                checkBox1.Checked = false;
                comboBox1.Enabled = checkBox1.Checked;
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                tilesetImport.Enabled = false;
                checkBox1.Checked = true;
                comboBox1.Enabled = checkBox1.Checked;
                comboBox1.SelectedIndex = trackSelector.SelectedIndex + selectedTrack.TilesetLookback;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            selectedTrack.TilesetLookback = (sbyte)(comboBox1.SelectedIndex - trackSelector.SelectedIndex);
        }
        private void button3_Click(object sender, EventArgs e)
        {
        SaveProject:
            saveFileDialog.DefaultExt = "tiled-project";
            saveFileDialog.Filter = "Tiled project files|*.tiled-project|All Files|*.*";

            saveFileDialog.FileName = "mksc.tiled-project";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy("template.tiled-project", saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show($"Error exporting project {ex}", "Export Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        goto SaveProject;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                if (MessageBox.Show("Please select a save location", "Export Aborted", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Retry)
                {
                    goto SaveProject;
                }
                else
                {
                    return;
                }
            }
        }
    }
}
