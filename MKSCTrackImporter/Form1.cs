using AdvancedLib;
using AdvancedLib.Serialize;
using AdvancedLib.Types;
using System.Diagnostics;
using System.Drawing.Imaging;
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
            manager = new Manager();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            labelRomOpened.Text = "ROM not loaded";
            labelRomOpened.ForeColor = Color.Maroon;

            openFileDialog.Filter = "GBA Files|*.gba|All Files|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    manager.Open(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.ToString()}", "Error opening file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelRomOpened.Text = "ROM not loaded";
                    labelRomOpened.ForeColor = Color.Maroon;
                    return;
                }
                labelRomOpened.Text = "Deserializing ROM";
                labelRomOpened.ForeColor = Color.Orange;
                try
                {
                    manager.Deserialize();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.ToString()}", "Error reading file",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    labelRomOpened.Text = "ROM not loaded";
                    labelRomOpened.ForeColor = Color.Maroon;
                    return;
                }
                labelRomOpened.Text = "Loaded Sucessfully";
                labelRomOpened.ForeColor = Color.DarkGreen;
                editorPanel.Enabled = true;
            }
        }

        private void tilemapExport_Click(object sender, EventArgs e)
        {
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var trackName = String.Join("", trackNames[trackSelector.SelectedIndex].Split(' '));
            var trackSize = selectedTrack.trackWidth * 128;
            string csvData = string.Join("\n", Enumerable.Range(0, trackSize)
                .Select(y => string.Join(",", Enumerable.Range(0, trackSize)
                    .Select(x => (selectedTrack.layout.indicies[x + y * trackSize] + 1).ToString())) + ","));

            // Comments for when I forget what goes wrong later:
            // nextlayerid may change if I add more layers (highest layer +1)
            // next objectid might do stuff (highest obj +1)
            XElement tilemapData = new XElement("map",
                new XAttribute("version", "1.10"), new XAttribute("tiledversion", "1.11.0"), new XAttribute("orientation", "orthogonal"), new XAttribute("renderorder", "right-down"),
                new XAttribute("width", $"{trackSize}"), new XAttribute("height", $"{trackSize}"),
                new XAttribute("tilewidth", "8"), new XAttribute("tileheight", "8"), new XAttribute("infinite", "0"),
                new XElement("tileset", new XAttribute("firstgid", "1"), new XAttribute("source", $"{trackName}.tsx")),
                new XElement("layer",
                    new XAttribute("id", "1"), new XAttribute("name", "Tilemap"),
                    new XAttribute("width", "256"), new XAttribute("height", "256"),

                    new XElement("data", new XAttribute("encoding", "csv"),
                        csvData
                    )
                )
            );
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
        }

        private void tilesetExport_Click(object sender, EventArgs e)
        {
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var trackName = String.Join("", trackNames[trackSelector.SelectedIndex].Split(' '));

            Bitmap image = new Bitmap(16 * 8, 16 * 8, PixelFormat.Format8bppIndexed);

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            IntPtr ptr = bmpData.Scan0;
            int stride = bmpData.Stride;
            int bytes = stride * image.Height;
            byte[] pixelData = new byte[bytes];

            Marshal.Copy(ptr, pixelData, 0, bytes);
            if (selectedTrack.tilesetLookback == 0)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                pixelData[(y * 8 + i) * stride + (x * 8 + j)] = selectedTrack.tileset.tiles[y * 16 + x].indicies[j, i];
                            }
                        }
                    }
                }
            } else
            {
                var lookbackTrack = manager.trackManager.tracks[trackSelector.SelectedIndex + selectedTrack.tilesetLookback];
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                pixelData[(y * 8 + i) * stride + (x * 8 + j)] = lookbackTrack.tileset.tiles[y * 16 + x].indicies[j, i];
                            }
                        }
                    }
                }
            }
            
            Marshal.Copy(pixelData, 0, ptr, bytes);
            image.UnlockBits(bmpData);
            var pal = image.Palette;
            for (int i = 0; i < selectedTrack.palette.paletteLength; i++)
            {
                pal.Entries[i] = selectedTrack.palette[i].ToColor();
            }
            for (int i = selectedTrack.palette.paletteLength; i < 256; i++)
            {
                pal.Entries[i] = Color.Black;
            }
            image.Palette = pal;

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
                                new XAttribute("value", selectedTrack.tileBehaviors[i])
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
        }

        private void tilemapImport_Click(object sender, EventArgs e)
        {
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            openFileDialog.Filter = "TMX Files|*.tmx|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                XDocument tilemap;
                try
                {
                    tilemap = XDocument.Load(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.ToString()}", "Error reading file",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                XElement? root = tilemap.Root;
#pragma warning disable CS8602
                try
                {
                    if ((root.Name == "map")
                        && (root.Attribute("orientation").Value == "orthogonal")
                        && (root.Attribute("tilewidth").Value == "8")
                        && (root.Attribute("tileheight").Value == "8")
                        && (root.Attribute("infinite").Value == "0")
                        )
                    {
                        XElement tilemapLayer =
                            (from layer in root.Descendants("layer")
                             where (layer.Attribute("name").Value == "Tilemap")
                             select layer).First();
                        XElement data =
                            (from layer in root.Descendants("data")
                             where (layer.Attribute("encoding").Value == "csv")
                             select layer).First();
                        var split = data.Value.Split(',');
                        byte[] trackData = split.Take(split.Length - 1).Select(s => (byte)(int.Parse(s) - 1)).ToArray(); // Convert CSV to byte array
                        selectedTrack.layout.indicies = trackData;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading track: {ex}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
#pragma warning restore CS8602

            }
        }
        private void tilesetImport_Click(object sender, EventArgs e)
        {
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            openFileDialog.Filter = "TSX Files|*.tsx;|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                XDocument tilesetDoc = XDocument.Load(openFileDialog.FileName);

                var tilesetPath = Path.Combine(Path.GetDirectoryName(openFileDialog.FileName), tilesetDoc.Root.Element("image").Attribute("source").Value);

                byte[] behaviors =
                    (from element in tilesetDoc.Root.Elements("tile")
                     select byte.Parse(element.Element("properties").Elements("property").Where(x => x.Attribute("name").Value == "behavior")
                     .First().Attribute("value").Value)).ToArray();
                selectedTrack.tileBehaviors = behaviors;

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
                selectedTrack.tileset.tiles = tiles;
                BgrColor[] pal = new BgrColor[64];
                for (int i = 0; i < 64; i++)
                {
                    var color = tileset.Palette.Entries[i];
                    pal[i] = new BgrColor(color.R, color.G, color.B);
                }

                selectedTrack.palette.palette = pal;
            }
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
            if (selectedTrack.tilesetLookback == 0)
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
                comboBox1.SelectedIndex = trackSelector.SelectedIndex + selectedTrack.tilesetLookback;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (trackSelector.SelectedIndex < 0)
            {
                MessageBox.Show("No Track Selected", "Please Select a track", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            selectedTrack.tilesetLookback = (sbyte)(comboBox1.SelectedIndex - trackSelector.SelectedIndex);
        }
    }
}
