namespace DelimitedFileAnalyzer
{
    public partial class DelimitedDataAnalyzer : Form
    {
        /**********************************************************************************************
        ###############################################################################################
        ##### Class Vars
        ###############################################################################################
        **********************************************************************************************/
        private const string dataAnalysisFile = "DataAnalysis.txt";
        private const string malformedRowsFile = "MalformedRows.txt";
        private const string sqlTableScript = "SqlTableScript.txt";
        private const string sqlDataScript = "SqlDataScript.txt";
        private const string initialDirectory = "c:\\";
        private const string openFileExtensions = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|All files (*.*)|*.*";

        private string? dataAnalysisFileWithPath;
        private string? malformedRowsFileWithPath;
        private string? sqlTableScriptWithPath;
        private string? sqlDataScriptWithPath;

        //static readonly int sqlTypes = 1;
        static readonly string[] sqlStrings = { "T-SQL" }; /*, "MySQL" };*/

        private DataAnalyzer? dataAnalyzer;

        private string qualifier = "\"", delimiter = ",", seperator = "\r\n", inputFileName = "", outputFolderName = "";
        private bool headersIncluded = true, standardizeColumnNames = true, trimFieldWhitespace = true, iqd = true;
        private bool createOutputFiles = true;
        private bool includeLineNumbersForMalformedRows = true;
        private SqlType sqlType = SqlType.TRANSACT_SQL;
        private int numberOfColumns = 0, batchSize = 25;
        private string tableName = "MyTable";

        /**********************************************************************************************
        ###############################################################################################
        ##### Form Initializer
        ###############################################################################################
        **********************************************************************************************/
        public DelimitedDataAnalyzer()
        {
            InitializeComponent();
            Dropdown_SqlSelector.Items.AddRange(sqlStrings);
            Dropdown_SqlSelector.SelectedIndex = 0;
            Button_GenerateSqlData.Enabled = false;
            Button_GenerateSqlTable.Enabled = false;
        }

        /**********************************************************************************************
        ###############################################################################################
        ##### Helper Functions 
        ###############################################################################################
        **********************************************************************************************/
        public void LogError(in string error)
        {
            LogMessage("ERROR: " + error);
        }

        private void LogMessage(in string message)
        {
            Textbox_MessageWindow.AppendText(message + Environment.NewLine);
        }

        private void ClearMessageBox()
        {
            Textbox_MessageWindow.Clear();
        }

        private void GetFormVariables()
        {
            qualifier = Textbox_Qualifier.Text;
            delimiter = Textbox_Delimiter.Text; ;
            seperator = Textbox_RowSeperator.Text;
            inputFileName = Textbox_FileName.Text;
            outputFolderName = Textbox_OutputFolder.Text;
            tableName = Textbox_SqlTableName.Text;
            headersIncluded = Checkbox_HeadersIncluded.Checked;
            standardizeColumnNames = Checkbox_StandardizeColumnNames.Checked;
            trimFieldWhitespace = Checkbox_TrimFieldWhitespace.Checked;
            iqd = Checkbox_IncompleteQualifierDetection.Checked;
            createOutputFiles = Checkbox_WriteOutputFiles.Checked;
            includeLineNumbersForMalformedRows = Checkbox_LineNumberMalformedRows.Checked;

            int selectedItem = Dropdown_SqlSelector.SelectedIndex;
            string? selectedSql = Dropdown_SqlSelector.Items[selectedItem].ToString();
            selectedSql ??= "T-SQL";
            sqlType = Column.ConvertSqlTypeStringToSqlType(selectedSql);

            if (Textbox_NumberOfColumns.Text != "")
            {
                if (Utilities.IsPositiveInt(Textbox_NumberOfColumns.Text))
                {
                    numberOfColumns = Int32.Parse(Textbox_NumberOfColumns.Text);
                }
                else
                {
                    numberOfColumns = 0;
                }
            }
            else
            {
                numberOfColumns = 0;
            }

            if (Texbox_BatchSize.Text != "")
            {
                if (Utilities.IsPositiveInt(Texbox_BatchSize.Text))
                {
                    batchSize = Int32.Parse(Texbox_BatchSize.Text);
                }
                else
                {
                    batchSize = 25;
                }
            }
            else
            {
                batchSize = 25;
            }

            if (delimiter == "\\t") { delimiter = "\t"; }

            if (seperator == "\\r\\n") { seperator = "\r\n"; }
            else if (seperator == "\\n") { seperator = "\n"; }
        }


        /**********************************************************************************************
        ###############################################################################################
        ##### Buttons 
        ###############################################################################################
        **********************************************************************************************/

        /*************************
         * SELECT INPUT FILE
        *************************/
        private void Button_SelectInputFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = initialDirectory;
                openFileDialog.Filter = openFileExtensions;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Textbox_FileName.Text = openFileDialog.FileName;
                }
            }
        }

        /*************************
         * SELECT OUTPUT FOLDER
        *************************/
        private void Button_SelectOutputFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.InitialDirectory = initialDirectory;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    Textbox_OutputFolder.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        /*************************
         * Analyze File
        *************************/
        private void Button_AnalyzeFile_Click(object sender, EventArgs e)
        {

            GetFormVariables();

            bool readyToAnalyze = true;

            if (delimiter == "") { readyToAnalyze = false; LogMessage("ERROR: Delimiter field must be populated."); }
            if (seperator == "") { readyToAnalyze = false; LogMessage("ERROR: Row Seperator field must be populated."); }
            if (inputFileName == "") { readyToAnalyze = false; LogMessage("ERROR: Input file field must be populated."); }
            if (outputFolderName == "" && createOutputFiles) { readyToAnalyze = false; LogMessage("ERROR: Output folder field must be populated or uncheck \"Create Output Files\"."); }
            if (numberOfColumns == 0 && !headersIncluded) { LogMessage("WARNING: Headers are not included and the number of expected columns is not provided."); }

            if (!readyToAnalyze)
            {
                LogMessage("Cannot Analyze File. Please check messages above this one for context.");
                return;
            }

            LogMessage("Beginning file analysis: " + inputFileName);
            if (createOutputFiles)
            {
                /* relatively reliable check for linux file paths... */
                string slash = "\\";
                if (Utilities.CountSubstringInString(outputFolderName, "/", "") > 0)
                {
                    slash = "/";
                }

                if (outputFolderName.Substring(outputFolderName.Length - 1, 1) != slash)
                {
                    outputFolderName += slash;
                }
                dataAnalysisFileWithPath = outputFolderName + dataAnalysisFile;
                malformedRowsFileWithPath = outputFolderName + malformedRowsFile;
                LogMessage("Analysis file will write to: " + dataAnalysisFileWithPath);
                LogMessage("Malformed rows file will write to: " + malformedRowsFileWithPath);
            }

            /* Create a new data analyzer */
            Button_GenerateSqlData.Enabled = false;
            Button_GenerateSqlTable.Enabled = false;
            dataAnalyzer = new DataAnalyzer(inputFileName, numberOfColumns, delimiter, qualifier, headersIncluded, seperator, iqd, trimFieldWhitespace, !standardizeColumnNames, batchSize);

            /* Print Options */
            //LogMessage(dataAnalyzer.OptionsToString());

            /* Process file - might take a while - add progress? */
            dataAnalyzer.ProcessFile();

            /* Get Column Count */
            numberOfColumns = dataAnalyzer.GetColumnCount();
            Textbox_NumberOfColumns.Text = numberOfColumns.ToString();

            /* Process Malformed Rows */
            int numMalformedRows = dataAnalyzer.GetMalformedRowsCount();
            if (numMalformedRows > 0)
            {
                string m_mr = "-----------------------------\r\n| Malformed Rows Found: "
                    + numMalformedRows.ToString()
                    + "\r\n-----------------------------\r\nRows:";
                LogMessage(m_mr);

                for (int i = 0; i < numMalformedRows; i++)
                {
                    LogMessage(" - " + dataAnalyzer.GetMalformedRow(i).ToString());
                }

                if (createOutputFiles && malformedRowsFileWithPath is not null)
                {
                    using (StreamWriter sw = new StreamWriter(malformedRowsFileWithPath))
                    {
                        if (includeLineNumbersForMalformedRows)
                        {
                            for (int i = 0; i < numMalformedRows; i++)
                            {
                                sw.WriteLine(dataAnalyzer.GetMalformedRow(i).ToString());
                            }
                        }
                        else
                        {
                            for (int i = 0; i < numMalformedRows; i++)
                            {
                                sw.WriteLine(dataAnalyzer.GetMalformedRow(i).line);
                            }
                        }
                    }
                }
            }

            /* Process Data Analysis */
            string m_da = "-----------------------------\r\n| Column Analysis: "
                   + "\r\n-----------------------------\r\nColumns:";
            LogMessage(m_da);

            for (int i = 0; i < numberOfColumns; i++)
            {
                LogMessage(" - " + dataAnalyzer.At(i).ToString());
            }

            if (createOutputFiles && dataAnalysisFileWithPath is not null)
            {
                using (StreamWriter sw = new StreamWriter(dataAnalysisFileWithPath))
                {
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        sw.WriteLine(dataAnalyzer.At(i).ToString());
                    }
                }
            }

            Button_GenerateSqlData.Enabled = true;
            Button_GenerateSqlTable.Enabled = true;
        }

        /*************************
         * Clear Message Box
        *************************/
        private void Button_ClearMessageBox_Click(object sender, EventArgs e)
        {
            ClearMessageBox();
        }

        /*************************
         * Generate SQL Table
        *************************/
        private void Button_GenerateSqlTable_Click(object sender, EventArgs e)
        {
            GetFormVariables();

            if(tableName == "")
            {
                tableName = "MyTable";
            }

            if (dataAnalyzer is not null)
            {
                string tbl = dataAnalyzer.GetTableStructureAsScript(sqlType, tableName);
                LogMessage(tbl);
                if (createOutputFiles)
                {
                    /* relatively reliable check for linux file paths... */
                    string slash = "\\";
                    if (Utilities.CountSubstringInString(outputFolderName, "/", "") > 0)
                    {
                        slash = "/";
                    }

                    if (outputFolderName.Substring(outputFolderName.Length - 1, 1) != slash)
                    {
                        outputFolderName += slash;
                    }

                    sqlTableScriptWithPath = outputFolderName + sqlTableScript;

                    using (StreamWriter sw = new StreamWriter(sqlTableScriptWithPath))
                    {
                        sw.Write(tbl);
                    }
                }
            }
        }

        /*************************
         * Generate SQL Data
        *************************/
        private void Button_GenerateSqlData_Click(object sender, EventArgs e)
        {
            GetFormVariables();

            if (tableName == "")
            {
                tableName = "MyTable";
            }

            if (dataAnalyzer is not null)
            {
                /* relatively reliable check for linux file paths... */
                string slash = "\\";
                if (Utilities.CountSubstringInString(outputFolderName, "/", "") > 0)
                {
                    slash = "/";
                }

                if (outputFolderName.Substring(outputFolderName.Length - 1, 1) != slash)
                {
                    outputFolderName += slash;
                }

                sqlDataScriptWithPath = outputFolderName + sqlDataScript;
                LogMessage("Writing SQL Script to: " + sqlDataScriptWithPath);
                dataAnalyzer.GetDataAsSqlScript(sqlType, sqlDataScriptWithPath, tableName);
                LogMessage("SQL Data Script Finished.");
            }
        }
    }
}