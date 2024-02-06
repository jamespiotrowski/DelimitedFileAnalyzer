using DelimitedFileAnalyzer;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DelimitedFileAnalyzer.DataAnalyzer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DelimitedFileAnalyzer
{
    internal class DataAnalyzer
    {
        public struct FileLine
        {
            public string line = "";
            public int lineNumber = 0;
            public FileLine() { }
            public override string ToString() { return ("Line " + lineNumber.ToString() + ": " + line); }
        }

        private string delimiter = "";
        private string fileName = "";
        private string qualifier = "";
        private string rowSeperator = "\r\n";
        private bool headerIncluded = false;
        private bool incompleteQualifierDetection = true;
        private bool trimLeadingAndTrailingWhiteSpace = true;
        private bool keepOriginalColumnNames = false;
        private int expectedColumns = 0;
        /* ifstream inFile; // to open the file, will use locally instead */
        /* locale loc; // For the encoding - not sure if needed in C# */

        private List<Column> columns = new List<Column>();
        private List<DateFormatAnalyzer> dateMetaData = new List<DateFormatAnalyzer>();
        private List<FileLine> malformedRows = new List<FileLine>();
        private int batchSize = 100;

        /******************************************************
         * OptionsToString
         * ----------------------------------------------------
         * Function to show the analyzers options
        ******************************************************/
        public string OptionsToString()
        {
            string s = "-------------------------------------\r\n| Delimited File Analyzer           |\r\n-------------------------------------\r\n";
            s += "| - File Name                          : " + fileName + "\r\n";
            s += "| - Delimiter                          : " + delimiter + "\r\n";
            s += "| - Qualifier                          : " + qualifier + "\r\n";
            s += "| - Headers Included                   : " + ((headerIncluded) ? "Yes" : "No") + "\r\n";
            s += "| - Number of Columns                  : " + ((expectedColumns == 0) ? "Auto-Detect" : (expectedColumns).ToString()) + "\r\n";
            s += "| - Incomplete Qualifier Detection     : " + ((incompleteQualifierDetection) ? "On" : "Off") + "\r\n";
            s += "| - Trim Leading & Trailing WhiteSpace : " + ((trimLeadingAndTrailingWhiteSpace) ? "Yes" : "No") + "\r\n";
            s += "| - Keep Original Column Names         : " + ((keepOriginalColumnNames) ? "Yes" : "No") + "\r\n";
            s += "-------------------------------------\r\n";
            return s;
        }

        /******************************************************
         * Constructor
        ******************************************************/
        public DataAnalyzer(
            string fileName
            , int expectedColumns = 0
            , string delimiter = ","
            , string qualifier = "\""
            , bool headerIncluded = true
            , string rowSeperator = "\r\n"
            , bool incompleteQualifierDetection = true
            , bool trimWhiteSpace = true
            , bool keepOriginalColumnNames = false /*
            , bool useThreads = false
            , size_t numThreads = 4 */
            , int ProcessingBatchSize = 25
        )
        {
            this.fileName = fileName;
            this.expectedColumns = expectedColumns;
            this.delimiter = delimiter;
            this.qualifier = qualifier;
            this.headerIncluded = headerIncluded;
            this.incompleteQualifierDetection = incompleteQualifierDetection;
            this.trimLeadingAndTrailingWhiteSpace = trimWhiteSpace;
            this.keepOriginalColumnNames = keepOriginalColumnNames;
            this.batchSize = ProcessingBatchSize;
            this.rowSeperator = rowSeperator;
        }

        /******************************************************
         * ReadUntil
         * ----------------------------------------------------
         * (Probably inefficient) function to read until
         * a specific series of characters is encountered. 
         * 
         * adding in getline stuff in case seperator is 
         * just an endline
        ******************************************************/
        private static void ReadUntil(ref string? s, ref StreamReader? streamReader, in string seperator)
        {
            /* check for null */
            s = "";
            if (streamReader is null) { s = null; return; }

            /* use probably more efficient function if seperator is just an endline */
            if (seperator == "\r\n" || seperator == "\n")
            {
                if (streamReader.Peek() >= 0)
                {
                    s = streamReader.ReadLine();
                }
                else
                {
                    s = null;
                }
            }
            /* else, read char by char! */
            else
            {
                bool ns_set = false;
                string ns = "";
                while (streamReader.Peek() >= 0) /* while chars to read */
                {
                    char c = (char)streamReader.Read(); /* read char */
                    ns += c; /* add char to string */
                    if (ns.Length >= seperator.Length)
                    {
                        if (ns.Substring(ns.Length - seperator.Length, seperator.Length) == seperator)
                        {
                            s = ns.Substring(0, ns.Length - seperator.Length);
                            ns_set = true;
                            break;
                        }
                    }
                }
                if (ns == "") { s = null; }
                else if(!ns_set){ s = ns; }
            }

            return;
        }

        /******************************************************
         * GetFileLine
         * ----------------------------------------------------
         * Function to read a line from a stream reader into 
         * a string
        ******************************************************/
        bool GetFileLine(ref string? s, ref StreamReader? streamReader)
        {
            if (streamReader is null) { return false; }
            ReadUntil(ref s, ref streamReader, rowSeperator);
            if (s is null) { return false; }

            return true;
        }

        /******************************************************
         * CompleteRowWithEndlines
         * ----------------------------------------------------
         * If a row has the premature row seperator in it,
         * this function will try to complete it, or it
         * will determine that the row is incomplete 
         * and add it to the malformed set
        ******************************************************/
        private void CompleteRowWithSeperator(ref FileLine fl, int delimiterCount, ref int totalLineCount, ref StreamReader? streamReader)
        {
            List<FileLine> strings = new List<FileLine>(); // Will be used to add to malformed strings if completed data row turns out malformed.
            string? s = ""; // Temp string to read into and do work
            string sLast;
            FileLine f = new FileLine();
            bool incompleteQualifierDetected = false;
            strings.Add(fl);  // Place current string into temp array
            int nextRowDelimiterCount;
            bool foundAdditionalLines = false;
            /* While the delimiter count is less than what we expect */
            while (delimiterCount < (expectedColumns - 1))
            {
                sLast = s;
                if (streamReader is null)
                {
                    break;
                }
                /* Get a line. If failed, close file and exit loop */
                if (!GetFileLine(ref s, ref streamReader))
                {
                    if (streamReader is not null)
                    {
                        streamReader.Close();
                        streamReader.Dispose();
                        streamReader = null;
                    }
                    s = sLast;
                    break;
                }
                else
                {
                    foundAdditionalLines = true;
                }


                totalLineCount += 1;

                if (s is null) { s = ""; }

                f.line = s;
                f.lineNumber = totalLineCount + ((headerIncluded) ? 1 : 0);

                if (incompleteQualifierDetection)
                {
                    nextRowDelimiterCount = Utilities.CountSubstringInString(s, delimiter, qualifier, false);
                    if (nextRowDelimiterCount == (expectedColumns - 1))
                    {
                        int lc = totalLineCount + ((headerIncluded) ? 0 : -1);
                        incompleteQualifierDetected = true;
                        //MessageBox.Show("Row with incomplete qualifier detected around line " + (lc).ToString() + ". Incomplete Qualifier Detection is currently on. If this is a mistake, rerun the program with this option set to off.");
                        break;
                    }
                }

                strings.Add(f); // Place current string into temp array
                delimiterCount += Utilities.CountSubstringInString(s, delimiter, qualifier, true); // With new line, count delims and add to current total
                /* If delimiter count is still less than, or equal to the desired ammount */
                if (delimiterCount <= (expectedColumns - 1))
                {
                    /*fl.line += "\n" + s;*/
                    fl.line += (rowSeperator + s); // Tack it onto the current string and continue.
                }
            }
            /* We broke out of loop above. If delimiter count is what we expect, good to go! */
            if (delimiterCount == (expectedColumns - 1) && !incompleteQualifierDetected)
            {
                return;
            }
            else
            {
                if (foundAdditionalLines)
                {
                    fl = f; // Set string equal to last string read. 
                    if (strings.Count > 0)
                    {
                        /* All strings in the array are guarenteed to be malformed, except the last one read */
                        int os = (incompleteQualifierDetected) ? 0 : 1;
                        for (int i = 0; i < (strings.Count - os); i++)
                        {
                            malformedRows.Add(strings[i]);  // Add row to malformed 
                        }
                    }
                }
            }
            /*
            At this point, the str contains the next row to be processed. 
            All malformed strings have been added to the log
            */
        }

        /******************************************************
         * InterpretRows
         * ----------------------------------------------------
         * This function takes in a set of rows from the data
         * and will send them through the set of columns 
         * to interpret their type
        ******************************************************/
        void InterpretRows(ref List<FileLine> rows)
        {
            List<string[]> data = new List<string[]>();
            for (int i = 0; i < rows.Count; i++)
            {
                data.Add(Utilities.SplitString(rows[i].line, delimiter, qualifier));
            }

            for (int i = 0; i < data.Count; i += 1)
            {
                for (int c = 0; c < expectedColumns; c += 1)
                {
                    columns[c].DetermineDataType(data[i][c]);
                }
            }
        }

        /******************************************************
        * InterpretRows
        * ----------------------------------------------------
        * This function takes in a set of rows from the data
        * and will send them through the set of columns 
        * to interpret their type
       ******************************************************/
        public void ProcessFile()
        {
            StreamReader? streamReader = new StreamReader(fileName);
            if (streamReader is not null)
            {
                List<FileLine> batch = new List<FileLine>();
                string? s1 = "";
                FileLine fl;
                int lineCount = (headerIncluded) ? 0 : 1;
                int totalLineCount = lineCount;

                // NOTE: If the file has no headers AND expected columns are not provided AND the first data row is malformed, this program probably will not work. 
                if (!headerIncluded && expectedColumns == 0)
                {
                    MessageBox.Show("Data file has no headers and expected columns is not provided. If the first row is malformed this program will NOT work.");
                }

                if (!GetFileLine(ref s1, ref streamReader))
                {
                    if (streamReader is not null)
                    {
                        streamReader.Close(); // If failed, close file
                        streamReader.Dispose();
                        streamReader = null;
                    }
                }

                /* Get expected columns if needed */
                if (expectedColumns == 0)
                {
                    s1 ??= "";
                    expectedColumns = Utilities.CountSubstringInString(s1, delimiter, qualifier) + 1;  // Grab the number of expected columns
                }

                /* If headers are not included */
                if (!headerIncluded)
                {
                    s1 ??= "";
                    fl.line = s1;
                    fl.lineNumber = totalLineCount + ((headerIncluded) ? 1 : 0);
                    batch.Add(fl); // This row contains data. Add this to the batch to be processed below
                    for (int i = 0; i < expectedColumns; i++)
                    {
                        columns.Add(new Column(i));
                    }
                }
                else
                {
                    s1 ??= "";
                    string[] columnNames = Utilities.SplitString(s1, delimiter, qualifier);
                    for (int i = 0; i < expectedColumns; i++)
                    {
                        columns.Add(new Column(columnNames[i], true, Column.ColumnNameFormat.PROPER_CASE));
                    }
                }

                /* Verify some valid amount of columns was determined */
                if (expectedColumns == 0)
                {
                    MessageBox.Show("Expected columns is 0. Will not process further.");
                    return;
                }

                /* Verify that batch either has data AND/OR file is still open*/
                if (batch.Count == 0 && streamReader is null)
                {
                    MessageBox.Show("Could not read from file. Will not process further.");
                    return;
                }

                /* We have something to process */
                int delimiterCount;
                while (true)
                {
                    /* Load Batch */
                    while (lineCount < batchSize && streamReader is not null)
                    {
                        if (!GetFileLine(ref s1, ref streamReader))
                        {
                            if (streamReader is not null)
                            {
                                streamReader.Close(); // If failed, close file
                                streamReader.Dispose();
                                streamReader = null;
                            }
                            break;
                        }

                        lineCount += 1;
                        totalLineCount += 1;

                        s1 ??= "";
                        fl.line = s1;
                        fl.lineNumber = totalLineCount + ((headerIncluded) ? 1 : 0);

                        /* Check the number of delimiters on the line */
                        delimiterCount = Utilities.CountSubstringInString(s1, delimiter, qualifier);
                        /* If less, than there is perhaps an endline within a qualifed row */
                        if (delimiterCount < (expectedColumns - 1))
                        {
                            /* Row has an unfinished qualifier */
                            if (Utilities.StringEndsWithinQualifiedField(s1, qualifier))
                            {
                                CompleteRowWithSeperator(ref fl, delimiterCount, ref totalLineCount, ref streamReader); // Call function to deal with this
                                delimiterCount = Utilities.CountSubstringInString(fl.line, delimiter, qualifier);  // With resulting string check again
                            }
                        }
                        /* If our delimiter count is greater, add to malformed. */
                        if (delimiterCount != (expectedColumns - 1))
                        {
                            //PrintMessage("DelimitedFileConverter::ProcessFile", WARNING, "Malformed Row. ");
                            malformedRows.Add(fl);
                            continue;
                        }

                        batch.Add(fl);

                    }

                    /* Interpret Rows */
                    if (batch.Count > 0)
                    {
                        InterpretRows(ref batch);
                    }

                    /* Check for continuation */
                    if (streamReader is null)
                    {
                        break;
                    }


                    /* Reset for next batch */
                    lineCount = 0;
                    batch.Clear();
                }
            }
            else
            {
                MessageBox.Show("File was unable to be opened. Cannot process file.");
            }

            if (malformedRows.Count > 0)
            {
                int i = 0;
                while (i < malformedRows.Count)
                {
                    if (malformedRows[i].line == "")
                    {
                        malformedRows.RemoveAt(i);
                        continue;
                    }
                    i++;
                }
            }
        }

        /******************************************************
        * GetMalformedRows
       ******************************************************/
        public List<FileLine> GetMalformedRows() { return malformedRows; }

        public FileLine GetMalformedRow(in int i) { return malformedRows[i]; }

        public int GetMalformedRowsCount() { return malformedRows.Count; }

        /******************************************************
        * GetTableStructureAsScript
        * ----------------------------------------------------
        * Function to generate the script to create 
        * a SQL table that matches the data
       ******************************************************/
        public string GetTableStructureAsScript(in SqlType sqlType, in string tableName)
        {
            string newTableName = tableName;
            string s = "";
            if (columns.Count > 0)
            {
                if (sqlType == SqlType.TRANSACT_SQL)
                {
                    s = "CREATE TABLE " + newTableName + " (\r\n";
                    /* First Column */
                    s += "\t[" + columns[0].GetColumnName() + "] "; /* Name */
                    s += columns[0].GetSqlTypeAsString(sqlType) + "\r\n"; /* SQL Type */
                    /* Remainder of columns */
                    for (int i = 1; i < columns.Count; i++)
                    {
                        s += "\t,[" + columns[i].GetColumnName() + "] "; /* Name */
                        s += columns[i].GetSqlTypeAsString(sqlType) + "\r\n"; /* SQL Type */
                    }
                    s += ");";
                }
            }
            else
            {
                MessageBox.Show("There are no columns. Cannot generate SQL Script.");
            }
            return s;
        }

        /******************************************************
        * NullIf
        * ----------------------------------------------------
        * Function to replace a string with NULL
       ******************************************************/
        string NullIf(in string s, in string n)
        {
            if (s == n)
            {
                return "NULL";
            }
            return s;
        }

        /******************************************************
        * GetSqlValueString
        * ----------------------------------------------------
        * Function to return a row in a SQL Values format
        * for an insert statement.
       ******************************************************/
        public string GetSqlValueString(in SqlType sqlType, in FileLine fileLine)
        {
            string[] values = Utilities.SplitString(fileLine.line, delimiter, qualifier);
            if (values.Length != columns.Count)
            {
                MessageBox.Show("Number of values in string does not equal number of columns.");
                return "";
            }

            if (sqlType == SqlType.TRANSACT_SQL)
            {
                string s = "(";
                for (int i = 0; i < columns.Count; i++)
                {
                    // If string, need quotes
                    if (columns[i].IsStringType())
                    {
                        // Need to qualify quotes with replace ( ' --> '' )
                        s += NullIf("'" + NullIf(values[i].Replace("'", "''"), "") + "'", "'NULL'");
                    }
                    // If bool, need to convert to BIT 
                    else if (columns[i].IsBoolean())
                    {
                        s += NullIf(Utilities.ConvertBooleanValueToString(values[i]), "");
                    }
                    // Should be fine from here
                    else
                    {
                        s += NullIf(values[i], "");
                    }

                    if (i != (columns.Count - 1))
                    {
                        s += ",";
                    }
                }
                s += ")";
                return s;
            }

            MessageBox.Show("Requested SQL Type is currently not implemented.");
            return "";
        }

        /******************************************************
        * GetDataAsSqlScript
        * ----------------------------------------------------
        * 
       ******************************************************/
        public void GetDataAsSqlScript(in SqlType sqlType, in string outFileName, in string tableName)
        {

            string newTableName = tableName;
            if (columns.Count == 0)
            {
                MessageBox.Show("There are no columns. Cannot generate SQL Script.");
                return;
            }

            using (StreamWriter sw = new StreamWriter(outFileName))
            {
                StreamReader? sr = new StreamReader(fileName);
                if (sr is not null)
                {
                    List<FileLine> batch = new List<FileLine>();
                    string? s1 = "";
                    FileLine fl;
                    int lineCount = (headerIncluded) ? 0 : 1;
                    int totalLineCount = lineCount;
                    if (!GetFileLine(ref s1, ref sr))
                    {
                        if (sr is not null)
                        {
                            sr.Close(); // If failed, close file
                            sr.Dispose();
                            sr = null;
                        }
                    }


                    /* If headers are not included */
                    if (!headerIncluded)
                    {
                        s1 ??= "";
                        fl.line = s1;
                        fl.lineNumber = totalLineCount + ((headerIncluded) ? 1 : 0);
                        batch.Add(fl);  // This row contains data. Add this to the batch to be processed below
                    }

                    /* Verify that batch either has data AND/OR file is still open*/
                    if (batch.Count == 0 && (sr is null))
                    {
                        MessageBox.Show("Could not read from file. Will not process further.");
                        return;
                    }

                    const int batchLimiter = 500;
                    int currentBatchCount = 0;

                    string columnList = "(";
                    for (int i = 0; i < columns.Count; i++)
                    {
                        columnList += "[" + columns[i].GetColumnName() + "]";
                        if (i != (columns.Count - 1))
                        {
                            columnList += ",";
                        }
                    }
                    columnList += ")";

                    /* We have something to process */
                    int delimiterCount = 0;
                    while (true)
                    {
                        /* Load Batch */
                        while (lineCount < batchSize && sr is not null)
                        {
                            if (!GetFileLine(ref s1, ref sr))
                            {
                                if (sr is not null)
                                {
                                    sr.Close(); // If failed, close file
                                    sr.Dispose();
                                    sr = null;
                                }
                                break;
                            }



                            lineCount += 1;
                            totalLineCount += 1;

                            s1 ??= "";
                            fl.line = s1;
                            fl.lineNumber = totalLineCount + ((headerIncluded) ? 1 : 0);

                            //wcout << fl.line << endl;

                            /* Check the number of delimiters on the line */
                            delimiterCount = Utilities.CountSubstringInString(s1, delimiter, qualifier);
                            /* If less, than there is perhaps an endline within a qualifed row */
                            if (delimiterCount < (expectedColumns - 1))
                            {
                                /* Row has an unfinished qualifier */
                                if (Utilities.StringEndsWithinQualifiedField(s1, qualifier))
                                {
                                    //PrintMessage("DelimitedFileConverter::ProcessFile", WARNING, "Found field contains qualified endlines.");
                                    CompleteRowWithSeperator(ref fl, delimiterCount, ref totalLineCount, ref sr); // Call function to deal with this
                                    delimiterCount = Utilities.CountSubstringInString(fl.line, delimiter, qualifier);  // With resulting string check again
                                }
                            }
                            /* If our delimiter count is greater, add to malformed. */
                            if (delimiterCount != (expectedColumns - 1))
                            {
                                //MessageBox.Show("Malformed row occured while generating insert script. Skipping. Line Number: " + (fl.lineNumber).ToString());
                                continue;
                            }

                            batch.Add(fl);
                        }

                        /* Interpret Rows */
                        if (batch.Count > 0)
                        {
                            string insertLine;
                            for (int i = 0; i < batch.Count; i++)
                            {
                                if (currentBatchCount == 0)
                                {
                                    insertLine = "\r\nINSERT INTO " + newTableName + " VALUES\r\n\t";
                                }
                                else
                                {
                                    insertLine = "\t,";
                                }
                                insertLine += GetSqlValueString(sqlType, batch[i]);
                                currentBatchCount += 1;
                                if (currentBatchCount >= batchLimiter)
                                {
                                    insertLine += ";";
                                    currentBatchCount = 0;
                                }
                                insertLine += "\r\n";
                                sw.Write(insertLine);
                            }
                        }

                        /* Check for continuation */
                        if (sr is null)
                        {
                            break;
                        }

                        /* Reset for next batch */
                        lineCount = 0;
                        batch.Clear();
                    }
                    if (currentBatchCount != 0)
                    {
                        sw.Write(";");
                    }
                }
                else
                {
                    MessageBox.Show("File unable to be opened.");
                }
            }
        }

        /******************************************************
        * GetColumnCount
       ******************************************************/
        public int GetColumnCount() { return columns.Count; }

        /******************************************************
        * At
       ******************************************************/
        public Column At(in int i) { return columns[i]; }
    }
}
