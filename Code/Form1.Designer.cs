namespace DelimitedFileAnalyzer
{
    partial class DelimitedDataAnalyzer
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Button_OpenFile = new Button();
            Textbox_FileName = new TextBox();
            Textbox_Delimiter = new TextBox();
            Textbox_RowSeperator = new TextBox();
            Textbox_Qualifier = new TextBox();
            Label_Delimiter = new Label();
            Label_Qualifier = new Label();
            Label_RowSeperator = new Label();
            Checkbox_StandardizeColumnNames = new CheckBox();
            Checkbox_HeadersIncluded = new CheckBox();
            Checkbox_IncompleteQualifierDetection = new CheckBox();
            Checkbox_TrimFieldWhitespace = new CheckBox();
            Textbox_OutputFolder = new TextBox();
            Button_SelectOutputFolder = new Button();
            Textbox_MessageWindow = new TextBox();
            Button_AnalyzeFile = new Button();
            Button_ClearMessageBox = new Button();
            Checkbox_WriteOutputFiles = new CheckBox();
            Dropdown_SqlSelector = new ComboBox();
            Label_NumberOfColumns = new Label();
            Textbox_NumberOfColumns = new TextBox();
            Label_BatchSize = new Label();
            Texbox_BatchSize = new TextBox();
            Checkbox_LineNumberMalformedRows = new CheckBox();
            Label_MalformedRows = new Label();
            Button_GenerateSqlTable = new Button();
            Button_GenerateSqlData = new Button();
            Label_SqlTableName = new Label();
            Textbox_SqlTableName = new TextBox();
            SuspendLayout();
            // 
            // Button_OpenFile
            // 
            Button_OpenFile.Location = new Point(23, 28);
            Button_OpenFile.Margin = new Padding(3, 4, 3, 4);
            Button_OpenFile.Name = "Button_OpenFile";
            Button_OpenFile.Size = new Size(219, 31);
            Button_OpenFile.TabIndex = 0;
            Button_OpenFile.Text = "Select Input File";
            Button_OpenFile.UseVisualStyleBackColor = true;
            Button_OpenFile.Click += Button_SelectInputFile_Click;
            // 
            // Textbox_FileName
            // 
            Textbox_FileName.BorderStyle = BorderStyle.FixedSingle;
            Textbox_FileName.Location = new Point(258, 28);
            Textbox_FileName.Margin = new Padding(3, 4, 3, 4);
            Textbox_FileName.Name = "Textbox_FileName";
            Textbox_FileName.Size = new Size(643, 27);
            Textbox_FileName.TabIndex = 1;
            // 
            // Textbox_Delimiter
            // 
            Textbox_Delimiter.BorderStyle = BorderStyle.FixedSingle;
            Textbox_Delimiter.Location = new Point(96, 105);
            Textbox_Delimiter.Margin = new Padding(3, 4, 3, 4);
            Textbox_Delimiter.Name = "Textbox_Delimiter";
            Textbox_Delimiter.Size = new Size(146, 27);
            Textbox_Delimiter.TabIndex = 2;
            // 
            // Textbox_RowSeperator
            // 
            Textbox_RowSeperator.BorderStyle = BorderStyle.FixedSingle;
            Textbox_RowSeperator.Location = new Point(128, 183);
            Textbox_RowSeperator.Margin = new Padding(3, 4, 3, 4);
            Textbox_RowSeperator.Name = "Textbox_RowSeperator";
            Textbox_RowSeperator.Size = new Size(114, 27);
            Textbox_RowSeperator.TabIndex = 3;
            Textbox_RowSeperator.Text = "\\r\\n";
            // 
            // Textbox_Qualifier
            // 
            Textbox_Qualifier.BorderStyle = BorderStyle.FixedSingle;
            Textbox_Qualifier.Location = new Point(96, 144);
            Textbox_Qualifier.Margin = new Padding(3, 4, 3, 4);
            Textbox_Qualifier.Name = "Textbox_Qualifier";
            Textbox_Qualifier.Size = new Size(146, 27);
            Textbox_Qualifier.TabIndex = 4;
            // 
            // Label_Delimiter
            // 
            Label_Delimiter.AutoSize = true;
            Label_Delimiter.Location = new Point(9, 105);
            Label_Delimiter.Name = "Label_Delimiter";
            Label_Delimiter.Size = new Size(74, 20);
            Label_Delimiter.TabIndex = 5;
            Label_Delimiter.Text = "Delimiter:";
            // 
            // Label_Qualifier
            // 
            Label_Qualifier.AutoSize = true;
            Label_Qualifier.Location = new Point(9, 144);
            Label_Qualifier.Name = "Label_Qualifier";
            Label_Qualifier.Size = new Size(69, 20);
            Label_Qualifier.TabIndex = 6;
            Label_Qualifier.Text = "Qualifier:";
            // 
            // Label_RowSeperator
            // 
            Label_RowSeperator.AutoSize = true;
            Label_RowSeperator.Location = new Point(9, 183);
            Label_RowSeperator.Name = "Label_RowSeperator";
            Label_RowSeperator.Size = new Size(110, 20);
            Label_RowSeperator.TabIndex = 7;
            Label_RowSeperator.Text = "Row Seperator:";
            // 
            // Checkbox_StandardizeColumnNames
            // 
            Checkbox_StandardizeColumnNames.AutoSize = true;
            Checkbox_StandardizeColumnNames.Checked = true;
            Checkbox_StandardizeColumnNames.CheckState = CheckState.Checked;
            Checkbox_StandardizeColumnNames.Location = new Point(14, 257);
            Checkbox_StandardizeColumnNames.Margin = new Padding(3, 4, 3, 4);
            Checkbox_StandardizeColumnNames.Name = "Checkbox_StandardizeColumnNames";
            Checkbox_StandardizeColumnNames.Size = new Size(215, 24);
            Checkbox_StandardizeColumnNames.TabIndex = 8;
            Checkbox_StandardizeColumnNames.Text = "Standardize Column Names";
            Checkbox_StandardizeColumnNames.UseVisualStyleBackColor = true;
            // 
            // Checkbox_HeadersIncluded
            // 
            Checkbox_HeadersIncluded.AutoSize = true;
            Checkbox_HeadersIncluded.Checked = true;
            Checkbox_HeadersIncluded.CheckState = CheckState.Checked;
            Checkbox_HeadersIncluded.Location = new Point(14, 356);
            Checkbox_HeadersIncluded.Margin = new Padding(3, 4, 3, 4);
            Checkbox_HeadersIncluded.Name = "Checkbox_HeadersIncluded";
            Checkbox_HeadersIncluded.Size = new Size(147, 24);
            Checkbox_HeadersIncluded.TabIndex = 9;
            Checkbox_HeadersIncluded.Text = "Headers Included";
            Checkbox_HeadersIncluded.UseVisualStyleBackColor = true;
            // 
            // Checkbox_IncompleteQualifierDetection
            // 
            Checkbox_IncompleteQualifierDetection.AutoSize = true;
            Checkbox_IncompleteQualifierDetection.Checked = true;
            Checkbox_IncompleteQualifierDetection.CheckState = CheckState.Checked;
            Checkbox_IncompleteQualifierDetection.Location = new Point(14, 324);
            Checkbox_IncompleteQualifierDetection.Margin = new Padding(3, 4, 3, 4);
            Checkbox_IncompleteQualifierDetection.Name = "Checkbox_IncompleteQualifierDetection";
            Checkbox_IncompleteQualifierDetection.Size = new Size(236, 24);
            Checkbox_IncompleteQualifierDetection.TabIndex = 10;
            Checkbox_IncompleteQualifierDetection.Text = "Incomplete Qualifier Detection";
            Checkbox_IncompleteQualifierDetection.UseVisualStyleBackColor = true;
            // 
            // Checkbox_TrimFieldWhitespace
            // 
            Checkbox_TrimFieldWhitespace.AutoSize = true;
            Checkbox_TrimFieldWhitespace.Checked = true;
            Checkbox_TrimFieldWhitespace.CheckState = CheckState.Checked;
            Checkbox_TrimFieldWhitespace.Location = new Point(14, 291);
            Checkbox_TrimFieldWhitespace.Margin = new Padding(3, 4, 3, 4);
            Checkbox_TrimFieldWhitespace.Name = "Checkbox_TrimFieldWhitespace";
            Checkbox_TrimFieldWhitespace.Size = new Size(177, 24);
            Checkbox_TrimFieldWhitespace.TabIndex = 11;
            Checkbox_TrimFieldWhitespace.Text = "Trim Field Whitespace";
            Checkbox_TrimFieldWhitespace.UseVisualStyleBackColor = true;
            // 
            // Textbox_OutputFolder
            // 
            Textbox_OutputFolder.BorderStyle = BorderStyle.FixedSingle;
            Textbox_OutputFolder.Location = new Point(258, 67);
            Textbox_OutputFolder.Margin = new Padding(3, 4, 3, 4);
            Textbox_OutputFolder.Name = "Textbox_OutputFolder";
            Textbox_OutputFolder.Size = new Size(643, 27);
            Textbox_OutputFolder.TabIndex = 13;
            // 
            // Button_SelectOutputFolder
            // 
            Button_SelectOutputFolder.Location = new Point(23, 67);
            Button_SelectOutputFolder.Margin = new Padding(3, 4, 3, 4);
            Button_SelectOutputFolder.Name = "Button_SelectOutputFolder";
            Button_SelectOutputFolder.Size = new Size(219, 31);
            Button_SelectOutputFolder.TabIndex = 12;
            Button_SelectOutputFolder.Text = "Select Output Folder";
            Button_SelectOutputFolder.UseVisualStyleBackColor = true;
            Button_SelectOutputFolder.Click += Button_SelectOutputFolder_Click;
            // 
            // Textbox_MessageWindow
            // 
            Textbox_MessageWindow.BackColor = SystemColors.Info;
            Textbox_MessageWindow.BorderStyle = BorderStyle.FixedSingle;
            Textbox_MessageWindow.Location = new Point(258, 224);
            Textbox_MessageWindow.Margin = new Padding(3, 4, 3, 4);
            Textbox_MessageWindow.Multiline = true;
            Textbox_MessageWindow.Name = "Textbox_MessageWindow";
            Textbox_MessageWindow.ReadOnly = true;
            Textbox_MessageWindow.ScrollBars = ScrollBars.Both;
            Textbox_MessageWindow.Size = new Size(643, 359);
            Textbox_MessageWindow.TabIndex = 14;
            Textbox_MessageWindow.WordWrap = false;
            // 
            // Button_AnalyzeFile
            // 
            Button_AnalyzeFile.Location = new Point(258, 105);
            Button_AnalyzeFile.Margin = new Padding(3, 4, 3, 4);
            Button_AnalyzeFile.Name = "Button_AnalyzeFile";
            Button_AnalyzeFile.Size = new Size(152, 69);
            Button_AnalyzeFile.TabIndex = 15;
            Button_AnalyzeFile.Text = "Analyze File";
            Button_AnalyzeFile.UseVisualStyleBackColor = true;
            Button_AnalyzeFile.Click += Button_AnalyzeFile_Click;
            // 
            // Button_ClearMessageBox
            // 
            Button_ClearMessageBox.Location = new Point(686, 183);
            Button_ClearMessageBox.Margin = new Padding(3, 4, 3, 4);
            Button_ClearMessageBox.Name = "Button_ClearMessageBox";
            Button_ClearMessageBox.Size = new Size(214, 33);
            Button_ClearMessageBox.TabIndex = 16;
            Button_ClearMessageBox.Text = "Clear Message Box";
            Button_ClearMessageBox.UseVisualStyleBackColor = true;
            Button_ClearMessageBox.Click += Button_ClearMessageBox_Click;
            // 
            // Checkbox_WriteOutputFiles
            // 
            Checkbox_WriteOutputFiles.AutoSize = true;
            Checkbox_WriteOutputFiles.Checked = true;
            Checkbox_WriteOutputFiles.CheckState = CheckState.Checked;
            Checkbox_WriteOutputFiles.Location = new Point(14, 224);
            Checkbox_WriteOutputFiles.Margin = new Padding(3, 4, 3, 4);
            Checkbox_WriteOutputFiles.Name = "Checkbox_WriteOutputFiles";
            Checkbox_WriteOutputFiles.Size = new Size(157, 24);
            Checkbox_WriteOutputFiles.TabIndex = 17;
            Checkbox_WriteOutputFiles.Text = "Create Output Files";
            Checkbox_WriteOutputFiles.UseVisualStyleBackColor = true;
            // 
            // Dropdown_SqlSelector
            // 
            Dropdown_SqlSelector.AllowDrop = true;
            Dropdown_SqlSelector.FormattingEnabled = true;
            Dropdown_SqlSelector.Location = new Point(732, 105);
            Dropdown_SqlSelector.Margin = new Padding(3, 4, 3, 4);
            Dropdown_SqlSelector.Name = "Dropdown_SqlSelector";
            Dropdown_SqlSelector.Size = new Size(168, 28);
            Dropdown_SqlSelector.TabIndex = 18;
            // 
            // Label_NumberOfColumns
            // 
            Label_NumberOfColumns.AutoSize = true;
            Label_NumberOfColumns.Location = new Point(9, 388);
            Label_NumberOfColumns.Name = "Label_NumberOfColumns";
            Label_NumberOfColumns.Size = new Size(145, 20);
            Label_NumberOfColumns.TabIndex = 20;
            Label_NumberOfColumns.Text = "Number of Columns:";
            // 
            // Textbox_NumberOfColumns
            // 
            Textbox_NumberOfColumns.BorderStyle = BorderStyle.FixedSingle;
            Textbox_NumberOfColumns.Location = new Point(160, 388);
            Textbox_NumberOfColumns.Margin = new Padding(3, 4, 3, 4);
            Textbox_NumberOfColumns.Name = "Textbox_NumberOfColumns";
            Textbox_NumberOfColumns.Size = new Size(82, 27);
            Textbox_NumberOfColumns.TabIndex = 19;
            // 
            // Label_BatchSize
            // 
            Label_BatchSize.AutoSize = true;
            Label_BatchSize.Location = new Point(9, 423);
            Label_BatchSize.Name = "Label_BatchSize";
            Label_BatchSize.Size = new Size(80, 20);
            Label_BatchSize.TabIndex = 22;
            Label_BatchSize.Text = "Batch Size:";
            // 
            // Texbox_BatchSize
            // 
            Texbox_BatchSize.BorderStyle = BorderStyle.FixedSingle;
            Texbox_BatchSize.Location = new Point(96, 423);
            Texbox_BatchSize.Margin = new Padding(3, 4, 3, 4);
            Texbox_BatchSize.Name = "Texbox_BatchSize";
            Texbox_BatchSize.Size = new Size(146, 27);
            Texbox_BatchSize.TabIndex = 21;
            Texbox_BatchSize.Text = "25";
            // 
            // Checkbox_LineNumberMalformedRows
            // 
            Checkbox_LineNumberMalformedRows.AutoSize = true;
            Checkbox_LineNumberMalformedRows.Checked = true;
            Checkbox_LineNumberMalformedRows.CheckState = CheckState.Checked;
            Checkbox_LineNumberMalformedRows.Location = new Point(14, 458);
            Checkbox_LineNumberMalformedRows.Margin = new Padding(3, 4, 3, 4);
            Checkbox_LineNumberMalformedRows.Name = "Checkbox_LineNumberMalformedRows";
            Checkbox_LineNumberMalformedRows.Size = new Size(174, 24);
            Checkbox_LineNumberMalformedRows.TabIndex = 23;
            Checkbox_LineNumberMalformedRows.Text = "Include Line Numbers";
            Checkbox_LineNumberMalformedRows.UseVisualStyleBackColor = true;
            // 
            // Label_MalformedRows
            // 
            Label_MalformedRows.AutoSize = true;
            Label_MalformedRows.Location = new Point(14, 486);
            Label_MalformedRows.Name = "Label_MalformedRows";
            Label_MalformedRows.Size = new Size(145, 20);
            Label_MalformedRows.TabIndex = 24;
            Label_MalformedRows.Text = "for Malformed Rows";
            // 
            // Button_GenerateSqlTable
            // 
            Button_GenerateSqlTable.Location = new Point(416, 105);
            Button_GenerateSqlTable.Margin = new Padding(3, 4, 3, 4);
            Button_GenerateSqlTable.Name = "Button_GenerateSqlTable";
            Button_GenerateSqlTable.Size = new Size(152, 69);
            Button_GenerateSqlTable.TabIndex = 25;
            Button_GenerateSqlTable.Text = "Get SQL Table";
            Button_GenerateSqlTable.UseVisualStyleBackColor = true;
            Button_GenerateSqlTable.Click += Button_GenerateSqlTable_Click;
            // 
            // Button_GenerateSqlData
            // 
            Button_GenerateSqlData.Location = new Point(574, 106);
            Button_GenerateSqlData.Margin = new Padding(3, 4, 3, 4);
            Button_GenerateSqlData.Name = "Button_GenerateSqlData";
            Button_GenerateSqlData.Size = new Size(152, 69);
            Button_GenerateSqlData.TabIndex = 26;
            Button_GenerateSqlData.Text = "Get SQL Data";
            Button_GenerateSqlData.UseVisualStyleBackColor = true;
            Button_GenerateSqlData.Click += Button_GenerateSqlData_Click;
            // 
            // Label_SqlTableName
            // 
            Label_SqlTableName.AutoSize = true;
            Label_SqlTableName.Location = new Point(9, 521);
            Label_SqlTableName.Name = "Label_SqlTableName";
            Label_SqlTableName.Size = new Size(121, 20);
            Label_SqlTableName.TabIndex = 28;
            Label_SqlTableName.Text = "SQL Table Name:";
            // 
            // Textbox_SqlTableName
            // 
            Textbox_SqlTableName.BorderStyle = BorderStyle.FixedSingle;
            Textbox_SqlTableName.Location = new Point(136, 521);
            Textbox_SqlTableName.Margin = new Padding(3, 4, 3, 4);
            Textbox_SqlTableName.Name = "Textbox_SqlTableName";
            Textbox_SqlTableName.Size = new Size(106, 27);
            Textbox_SqlTableName.TabIndex = 27;
            Textbox_SqlTableName.Text = "MyTable";
            // 
            // DelimitedDataAnalyzer
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(Label_SqlTableName);
            Controls.Add(Textbox_SqlTableName);
            Controls.Add(Button_GenerateSqlData);
            Controls.Add(Button_GenerateSqlTable);
            Controls.Add(Label_MalformedRows);
            Controls.Add(Checkbox_LineNumberMalformedRows);
            Controls.Add(Label_BatchSize);
            Controls.Add(Texbox_BatchSize);
            Controls.Add(Label_NumberOfColumns);
            Controls.Add(Textbox_NumberOfColumns);
            Controls.Add(Dropdown_SqlSelector);
            Controls.Add(Checkbox_WriteOutputFiles);
            Controls.Add(Button_ClearMessageBox);
            Controls.Add(Button_AnalyzeFile);
            Controls.Add(Textbox_MessageWindow);
            Controls.Add(Textbox_OutputFolder);
            Controls.Add(Button_SelectOutputFolder);
            Controls.Add(Checkbox_TrimFieldWhitespace);
            Controls.Add(Checkbox_IncompleteQualifierDetection);
            Controls.Add(Checkbox_HeadersIncluded);
            Controls.Add(Checkbox_StandardizeColumnNames);
            Controls.Add(Label_RowSeperator);
            Controls.Add(Label_Qualifier);
            Controls.Add(Label_Delimiter);
            Controls.Add(Textbox_Qualifier);
            Controls.Add(Textbox_RowSeperator);
            Controls.Add(Textbox_Delimiter);
            Controls.Add(Textbox_FileName);
            Controls.Add(Button_OpenFile);
            Margin = new Padding(3, 4, 3, 4);
            Name = "DelimitedDataAnalyzer";
            Text = "Delimited File Analyzer";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Button_OpenFile;
        private TextBox Textbox_FileName;
        private TextBox Textbox_Delimiter;
        private TextBox Textbox_RowSeperator;
        private TextBox Textbox_Qualifier;
        private Label Label_Delimiter;
        private Label Label_Qualifier;
        private Label Label_RowSeperator;
        private CheckBox Checkbox_StandardizeColumnNames;
        private CheckBox Checkbox_HeadersIncluded;
        private CheckBox Checkbox_IncompleteQualifierDetection;
        private CheckBox Checkbox_TrimFieldWhitespace;
        private TextBox Textbox_OutputFolder;
        private Button Button_SelectOutputFolder;
        private TextBox Textbox_MessageWindow;
        private Button Button_AnalyzeFile;
        private Button Button_ClearMessageBox;
        private CheckBox Checkbox_WriteOutputFiles;
        private ComboBox Dropdown_SqlSelector;
        private Label Label_NumberOfColumns;
        private TextBox Textbox_NumberOfColumns;
        private Label Label_BatchSize;
        private TextBox Texbox_BatchSize;
        private CheckBox Checkbox_LineNumberMalformedRows;
        private Label Label_MalformedRows;
        private Button Button_GenerateSqlTable;
        private Button Button_GenerateSqlData;
        private Label Label_SqlTableName;
        private TextBox Textbox_SqlTableName;
    }
}