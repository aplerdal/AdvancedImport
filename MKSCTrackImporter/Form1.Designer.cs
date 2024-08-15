namespace MKSCTrackImporter
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public static string[] trackNames = {
        "SNES Mario Circuit 1",
        "SNES Donut Plains 1",
        "SNES Ghost Valley 1",
        "SNES Bowser Castle 1",
        "SNES Mario Circuit 2",
        "SNES Choco Island 1",
        "SNES Ghost Valley 2",
        "SNES Donut Plains 2",
        "SNES Bowser Castle 2",
        "SNES Mario Circuit 3",
        "SNES Koopa Beach 1",
        "SNES Choco Island 2",
        "SNES Vanilla Lake 1",
        "SNES Bowser Castle 3",
        "SNES Mario Circuit 4",
        "SNES Donut Plains 3",
        "SNES Koopa Beach 2",
        "SNES Ghost Valley 3",
        "SNES Vanilla Lake 2",
        "SNES Rainbow Road",
        "SNES Battle Course 1",
        "SNES Battle Course 2",
        "SNES Battle Course 3",
        "SNES Battle Course 4",
        "Peach Circuit",
        "Shy Guy Beach",
        "Sunset Wilds",
        "Bowser Castle 1",
        "Luigi Circuit",
        "Riverside Park",
        "Yoshi Desert",
        "Bowser Castle 2",
        "Mario Circuit",
        "Cheep-Cheep Island",
        "Ribbon Road",
        "Bowser Castle 3",
        "Snow Land",
        "Boo Lake",
        "Cheese Land",
        "Rainbow Road",
        "Sky Garden",
        "Broken Pier",
        "Bowser Castle 4",
        "Lakeside Park",
        "Battle Course???",
        "Battle Course???",
        "Battle Course???",
        "Battle Course???",
        "Test Track???",
    };

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            button1 = new Button();
            labelRomOpened = new Label();
            editorPanel = new Panel();
            comboBox1 = new ComboBox();
            checkBox1 = new CheckBox();
            tilesetImport = new Button();
            button2 = new Button();
            label3 = new Label();
            tilemapImport = new Button();
            tilemapExport = new Button();
            label2 = new Label();
            label1 = new Label();
            trackSelector = new ComboBox();
            saveButton = new Button();
            toolTip1 = new ToolTip(components);
            editorPanel.SuspendLayout();
            SuspendLayout();
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "mksc.gba";
            // 
            // saveFileDialog
            // 
            saveFileDialog.DefaultExt = "tmx";
            saveFileDialog.Filter = "TMX Files|*.tmx|All Files|*.*";
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Load Game";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // labelRomOpened
            // 
            labelRomOpened.AutoSize = true;
            labelRomOpened.ForeColor = Color.Maroon;
            labelRomOpened.Location = new Point(93, 16);
            labelRomOpened.Name = "labelRomOpened";
            labelRomOpened.Size = new Size(94, 15);
            labelRomOpened.TabIndex = 1;
            labelRomOpened.Text = "ROM not loaded";
            // 
            // editorPanel
            // 
            editorPanel.BorderStyle = BorderStyle.FixedSingle;
            editorPanel.Controls.Add(comboBox1);
            editorPanel.Controls.Add(checkBox1);
            editorPanel.Controls.Add(tilesetImport);
            editorPanel.Controls.Add(button2);
            editorPanel.Controls.Add(label3);
            editorPanel.Controls.Add(tilemapImport);
            editorPanel.Controls.Add(tilemapExport);
            editorPanel.Controls.Add(label2);
            editorPanel.Controls.Add(label1);
            editorPanel.Controls.Add(trackSelector);
            editorPanel.Enabled = false;
            editorPanel.Location = new Point(12, 41);
            editorPanel.Name = "editorPanel";
            editorPanel.Size = new Size(776, 397);
            editorPanel.TabIndex = 2;
            // 
            // comboBox1
            // 
            comboBox1.DropDownWidth = 178;
            comboBox1.Enabled = false;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(325, 65);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(165, 23);
            comboBox1.TabIndex = 7;
            comboBox1.DataSource = trackNames;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Enabled = false;
            checkBox1.Location = new Point(180, 68);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(139, 19);
            checkBox1.TabIndex = 6;
            checkBox1.Text = "Reuse previous tileset";
            toolTip1.SetToolTip(checkBox1, "Feature currently disabled!");
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // tilesetImport
            // 
            tilesetImport.Location = new Point(121, 65);
            tilesetImport.Name = "tilesetImport";
            tilesetImport.Size = new Size(53, 23);
            tilesetImport.TabIndex = 5;
            tilesetImport.Text = "Import";
            tilesetImport.UseVisualStyleBackColor = true;
            tilesetImport.Click += tilesetImport_Click;
            // 
            // button2
            // 
            button2.Location = new Point(64, 65);
            button2.Name = "button2";
            button2.Size = new Size(53, 23);
            button2.TabIndex = 4;
            button2.Text = "Export";
            button2.UseVisualStyleBackColor = true;
            button2.Click += tilesetExport_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(9, 69);
            label3.Name = "label3";
            label3.Size = new Size(40, 15);
            label3.TabIndex = 3;
            label3.Text = "Tileset";
            // 
            // tilemapImport
            // 
            tilemapImport.Location = new Point(121, 31);
            tilemapImport.Name = "tilemapImport";
            tilemapImport.Size = new Size(53, 23);
            tilemapImport.TabIndex = 2;
            tilemapImport.Text = "Import";
            tilemapImport.UseVisualStyleBackColor = true;
            tilemapImport.Click += tilemapImport_Click;
            // 
            // tilemapExport
            // 
            tilemapExport.Location = new Point(64, 31);
            tilemapExport.Name = "tilemapExport";
            tilemapExport.Size = new Size(53, 23);
            tilemapExport.TabIndex = 0;
            tilemapExport.Text = "Export";
            tilemapExport.UseVisualStyleBackColor = true;
            tilemapExport.Click += tilemapExport_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(9, 35);
            label2.Name = "label2";
            label2.Size = new Size(49, 15);
            label2.TabIndex = 1;
            label2.Text = "Tilemap";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 6);
            label1.Name = "label1";
            label1.Size = new Size(84, 15);
            label1.TabIndex = 1;
            label1.Text = "Selected Track:";
            // 
            // trackSelector
            // 
            trackSelector.FormattingEnabled = true;
            trackSelector.Items.AddRange(new object[] { "SNES Mario Circuit 1", "SNES Donut Plains 1", "SNES Ghost Valley 1", "SNES Bowser Castle 1", "SNES Mario Circuit 2", "SNES Choco Island 1", "SNES Ghost Valley 2", "SNES Donut Plains 2", "SNES Bowser Castle 2", "SNES Mario Circuit 3", "SNES Koopa Beach 1", "SNES Choco Island 2", "SNES Vanilla Lake 1", "SNES Bowser Castle 3", "SNES Mario Circuit 4", "SNES Donut Plains 3", "SNES Koopa Beach 2", "SNES Ghost Valley 3", "SNES Vanilla Lake 2", "SNES Rainbow Road", "SNES Battle Course 1", "SNES Battle Course 2", "SNES Battle Course 3", "SNES Battle Course 4", "Peach Circuit", "Shy Guy Beach", "Sunset Wilds", "Bowser Castle 1", "Luigi Circuit ", "Riverside Park", "Yoshi Desert", "Bowser Castle 2", "Mario Circuit", "Cheep-Cheep Island", "Ribbon Road", "Bowser Castle 3", "Snow Land", "Boo Lake", "Cheese Land", "Rainbow Road", "Sky Garden", "Broken Pier", "Bowser Castle 4", "Lakeside Park", "Battle Course 1", "Battle Course 2", "Battle Course 3", "Battle Course 4", "Ceramony" });
            trackSelector.Location = new Point(99, 3);
            trackSelector.Name = "trackSelector";
            trackSelector.Size = new Size(178, 23);
            trackSelector.TabIndex = 0;
            trackSelector.SelectedIndexChanged += trackSelector_SelectedIndexChanged;
            // 
            // saveButton
            // 
            saveButton.Location = new Point(713, 16);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(75, 23);
            saveButton.TabIndex = 3;
            saveButton.Text = "Save Game";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(saveButton);
            Controls.Add(editorPanel);
            Controls.Add(labelRomOpened);
            Controls.Add(button1);
            Name = "Form1";
            Text = "AdvancedImport";
            editorPanel.ResumeLayout(false);
            editorPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private Button button1;
        private Label labelRomOpened;
        private Panel editorPanel;
        private ComboBox trackSelector;
        private Label label1;
        private Label label2;
        private Button tilemapExport;
        private Button tilemapImport;
        private Button tilesetImport;
        private Button button2;
        private Label label3;
        private Button saveButton;
        private CheckBox checkBox1;
        private ComboBox comboBox1;
        private ToolTip toolTip1;
    }
}
