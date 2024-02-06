using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DelimitedFileAnalyzer
{

    enum SqlType
    {
        TRANSACT_SQL
        , MY_SQL
    };

    internal class Column
    {
        static readonly int numTSqlStrings = 6;
        static readonly string[] tSqlStrings = { "transact-sql", "t-sql", "transact sql", "t sql", "transactsql", "tsql" };

        static readonly int numMySqlStrings = 3;
        static readonly string[] mySqlStrings = { "my-sql", "mysql", "my sql" };

        /******************************************************
         * ConvertSqlTypeStringToSqlType
         * ----------------------------------------------------
         * Function to convert a string to a Sql type
        ******************************************************/
        public static SqlType ConvertSqlTypeStringToSqlType(string sqlType)
        {
            sqlType = Utilities.ToLowerCaseString(sqlType);

            for (int i = 0; i < numTSqlStrings; i++)
            {
                if (sqlType == tSqlStrings[i])
                {
                    return SqlType.TRANSACT_SQL;
                }
            }

            for (int i = 0; i < numMySqlStrings; i++)
            {
                if (sqlType == mySqlStrings[i])
                {
                    return SqlType.MY_SQL;
                }
            }

            return SqlType.TRANSACT_SQL;
        }

        /******************************************************
         * Enums
        ******************************************************/
        public enum ColumnNameFormat
        {
            PROPER_CASE
            // , CAMEL_CASE
            // , UPPER_CASE
            // , LOWER_CASE
        };

        enum ColumnType
        {
            STRING
            , DECIMAL
            , INTEGER
            , CHARACTER
            , TEXT
            , DATE
            , DATETIME
            , BIT
            , BOOLEAN
            , UNDETERMINED
        };

        /******************************************************
         * FormatColumnName
         * ----------------------------------------------------
         * Function to standardize a column name
        ******************************************************/
        private static string FormatColumnName(in string oldColumnName, ColumnNameFormat columnNameFormat)
        {
            string newColumnName = "";
            bool setToUpperCase;
            /* proper case */
            if (columnNameFormat == ColumnNameFormat.PROPER_CASE)
            {
                setToUpperCase = true;
                for (int i = 0; i < oldColumnName.Length; i++)
                {
                    char c = oldColumnName[i];
                    if (Utilities.IsAcceptableColumnCharacter(c))
                    {
                        if (Utilities.IsLetter(c) && setToUpperCase)
                        {
                            newColumnName += Utilities.ToUpperCaseLetter(c);
                            setToUpperCase = false;
                        }
                        else
                        {
                            newColumnName += Utilities.ToLowerCaseLetter(c);
                        }
                    }

                    else
                    {
                        setToUpperCase = true;
                    }
                }
            }

            else
            {
                return oldColumnName;
            }

            return newColumnName;
        }

        static readonly string defaultColumnName = "C";

        /* Main column descriptors */
        string columnName = "";
        ColumnType columnType = ColumnType.UNDETERMINED;
        DateFormatAnalyzer dateFormatProcessor = new DateFormatAnalyzer();

        int columnLength = 0;
        int precision = 0;
        bool nullable = false;
        bool unicode = false;

        /* Members used to identify exact type */
        int instancesCounted = 0;       // Use this to track how many rows there are
        int nullsCounted = 0;           // Count how many instances were NULL or ""
        int columnLengthFrequency = 0;  // This is used to determine if string fields are constant lengths

        /******************************************************
         * ChangeColumnType
         * ----------------------------------------------------
         * Function to determine if the current column type
         * can convert into the new one
         * 
         * For example, an int can convert to a decimal type
         * 
         * a string cannot convert to an int
        ******************************************************/
        private bool ChangeColumnType(in ColumnType newColumnType)
        {
            if (newColumnType == columnType)
            {
                return true;
            }

            switch (columnType)
            {
                case (ColumnType.UNDETERMINED): { return true; }
                case (ColumnType.TEXT): { return false; }
                case (ColumnType.STRING):
                    {
                        if (newColumnType == ColumnType.TEXT)
                        {
                            return true;
                        }
                        return false;
                    }
                case (ColumnType.DECIMAL):
                case (ColumnType.CHARACTER):
                case (ColumnType.DATETIME):
                case (ColumnType.BOOLEAN):
                    {
                        if (newColumnType == ColumnType.STRING || newColumnType == ColumnType.TEXT)
                        {
                            return true;
                        }
                        return false;
                    }
                case (ColumnType.INTEGER):
                    {
                        if (newColumnType == ColumnType.STRING || newColumnType == ColumnType.TEXT || newColumnType == ColumnType.DECIMAL)
                        {
                            return true;
                        }
                        return false;
                    }
                case (ColumnType.DATE):
                    {
                        if (newColumnType == ColumnType.STRING || newColumnType == ColumnType.TEXT || newColumnType == ColumnType.DATETIME)
                        {
                            return true;
                        }
                        return false;
                    }
                case (ColumnType.BIT):
                    {
                        if (newColumnType == ColumnType.STRING || newColumnType == ColumnType.TEXT || newColumnType == ColumnType.INTEGER || newColumnType == ColumnType.DECIMAL)
                        {
                            return true;
                        }
                        return false;
                    }
            }
            return false;
        }

        /******************************************************
         * IsType
         * ----------------------------------------------------
         * Functions to determine column types
        ******************************************************/
        /* Check for NULL Value */
        private static bool IsBlank(string s)
        {
            s = Utilities.ToLowerCaseString(s);
            return (s == "" || s == "null");
        }

        /* Check for bit value */
        private static bool IsBit(in string s) { return (s == "1" || s == "0"); }

        /* Check for char */
        private static bool IsCharacter(in string s) { return (s.Length == 1); }

        /* Check for Boolean value */
        private static bool IsBoolean(in string s)
        {
            string t = Utilities.ToLowerCaseString(s);
            return (t == "true" || t == "false");
        }

        /* Check for int value */
        private static bool IsInt(in string s)
        {
            if (s.Length == 0) { return false; }
            int v = (s[0] == '-') ? 1 : 0;
            for (int i = v; i < s.Length; i++)
            {
                if (!Utilities.IsDigit(s[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /* Is Decimal */
        private static bool IsDecimal(in string s)
        {
            if (s.Length == 0) { return false; }
            bool foundDecimal = false;
            int v = (s[0] == '-') ? 1 : 0;
            for (int i = v; i < s.Length; i++)
            {
                if (!Utilities.IsDigit(s[i]))
                {
                    if (s[i] == '.' && !foundDecimal)
                    {
                        foundDecimal = true;
                    }

                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /* Is Text */
        private static bool IsText(in string s)
        {
            if (s.Length > 4000)
            {
                return true;
            }
            return false;
        }

        /******************************************************
         * Constructors
        ******************************************************/
        public Column() { }

        public Column(int columnIndex) { columnName = defaultColumnName + columnIndex.ToString(); }

        public Column(in string columnName, in bool formatColumnName, ColumnNameFormat columnNameFormat)
        {
            if (formatColumnName)
            {
                this.columnName = FormatColumnName(Utilities.TrimWhiteSpace(columnName), columnNameFormat).Replace("]", "").Replace("[", "");
            }
            else
            {

                this.columnName = columnName.Replace("]", "").Replace("[", "");
            }
        }

        /******************************************************
         * DetermineDataType
         * ----------------------------------------------------
         * Function to intake some string value and 
         * determine what its data type will be 
         * 
         * This function will be called for every row
         * in the data and this function may change the 
         * type of the column multiple times
        ******************************************************/
        public void DetermineDataType(in string s)
        {
            instancesCounted += 1;
            bool previousSuccess = false;

            if (IsBlank(s))
            {
                nullable = true;
                nullsCounted += 1;
            }
            else
            {

                int totalLen = s.Length;
                if (totalLen == columnLength)
                {
                    columnLengthFrequency += 1;
                }

                else
                {
                    columnLengthFrequency = 1;
                    if (totalLen > columnLength)
                    {
                        columnLength = totalLen;
                    }
                }

                if (ChangeColumnType(ColumnType.BIT) && IsBit(s))
                {
                    columnType = ColumnType.BIT;
                    previousSuccess = true;
                }
                else if (ChangeColumnType(ColumnType.DECIMAL) && IsDecimal(s) && (!IsInt(s) || columnType == ColumnType.DECIMAL))
                {
                    columnType = ColumnType.DECIMAL;
                    previousSuccess = true;
                    int decimalLen;

                    if (s.Contains('.'))
                    {
                        string[] decimalParts = Utilities.SplitString(s, ".", "");
                        // Account for negative here?
                        totalLen = decimalParts[1].Length + decimalParts[0].Length;
                        decimalLen = decimalParts[1].Length;
                    }
                    else
                    {
                        totalLen = s.Length;
                        decimalLen = 0;
                    }

                    if (totalLen > columnLength)
                    {
                        columnLength = totalLen;
                    }
                    if (decimalLen > precision)
                    {
                        precision = decimalLen;
                    }
                }
                else if (ChangeColumnType(ColumnType.INTEGER) && IsInt(s))
                {
                    columnType = ColumnType.INTEGER;
                    previousSuccess = true;
                }
                else if (ChangeColumnType(ColumnType.BOOLEAN) && IsBoolean(s))
                {
                    columnType = ColumnType.BOOLEAN;
                    previousSuccess = true;
                }
                else if (ChangeColumnType(ColumnType.DATE) || ChangeColumnType(ColumnType.DATETIME))
                {
                    dateFormatProcessor.ProcessDate(s);
                    if (dateFormatProcessor.IsPossibleDateTime())
                    {
                        columnType = ColumnType.DATETIME;
                        previousSuccess = true;
                    }
                    else if (dateFormatProcessor.IsPossibleDate())
                    {
                        columnType = ColumnType.DATE;
                        previousSuccess = true;
                    }
                }

                if (!previousSuccess && ChangeColumnType(ColumnType.CHARACTER) && IsCharacter(s))
                {
                    columnType = ColumnType.CHARACTER;
                }
                else if (!previousSuccess && ChangeColumnType(ColumnType.TEXT) && IsText(s))
                {
                    columnType = ColumnType.TEXT;
                    if (!unicode && Utilities.ContainsUnicodeChars(s))
                    {
                        unicode = true;
                    }
                }
                else if (!previousSuccess)
                {
                    columnType = ColumnType.STRING;
                    if (!unicode && Utilities.ContainsUnicodeChars(s))
                    {
                        unicode = true;
                    }
                }
            }
        }

        /******************************************************
         * ToString
         * ----------------------------------------------------
         * Function to represent a column (name + type)
         * as a string
        ******************************************************/
        public override string ToString()
        {
            bool constantLength = (instancesCounted == (nullsCounted + columnLengthFrequency));
            string s = columnName + " : [ ";
            switch (columnType)
            {
                case (ColumnType.STRING):
                    {
                        s += ((unicode) ? "LSTRING(" : "STRING(") + (columnLength).ToString();
                        s += (constantLength) ? "c)" : ")";
                        break;
                    }
                case (ColumnType.DECIMAL):
                    {
                        s += "DECIMAL(" + (columnLength).ToString() + "," + (precision).ToString() + ")";
                        break;
                    }
                case (ColumnType.INTEGER): { s += "INTEGER"; break; }
                case (ColumnType.CHARACTER): { s += "CHARACTER"; break; }
                case (ColumnType.TEXT): { s += ((unicode) ? "LTEXT" : "TEXT"); break; }
                case (ColumnType.DATE): { s += "DATE"; break; }
                case (ColumnType.DATETIME): { s += "DATETIME"; break; }
                case (ColumnType.BIT): { s += "BIT"; break; }
                case (ColumnType.BOOLEAN): { s += "BOOLEAN"; break; }
                case (ColumnType.UNDETERMINED): { s += "UNDETERMINED"; break; }
            }
            s += (nullable) ? " NULL" : " NOT NULL";
            s += " ]";
            return s;
        }

        /******************************************************
         * GetColumnName
        ******************************************************/
        public string GetColumnName() { return columnName; }

        /******************************************************
         * GetSqlTypeAsString
         * ----------------------------------------------------
         * Function to get the corresponding SQL script
         * to create a column of such type
        ******************************************************/
        public string GetSqlTypeAsString(SqlType sqlType)
        {
            string s = "";
            bool constantLength = (instancesCounted == (nullsCounted + columnLengthFrequency));
            if (sqlType == SqlType.TRANSACT_SQL)
            {
                switch (columnType)
                {
                    case (ColumnType.STRING):
                        {
                            if (constantLength)
                            {
                                s = (unicode ? "[NCHAR](" : "[CHAR](") + (columnLength).ToString() + ")";
                            }
                            else
                            {
                                s = (unicode ? "[NVARCHAR](" : "[VARCHAR](") + (columnLength).ToString() + ")";
                            }
                            break;
                        }
                    case (ColumnType.DECIMAL):
                        {
                            s = "[DECIMAL](" + (columnLength).ToString() + "," + (precision).ToString() + ")";
                            break;
                        }
                    case (ColumnType.CHARACTER): { s = (unicode ? "[NCHAR](1)" : "[CHAR](1)"); break; }
                    case (ColumnType.TEXT): { s = "[TEXT]"; break; }
                    case (ColumnType.DATE): { s = "[DATE]"; break; }
                    case (ColumnType.DATETIME):
                        {
                            if (columnLength < 20) { s = "[SMALLDATETIME]"; }
                            else if (columnLength < 24) { s = "[DATETIME]"; }
                            else { s = "[DATETIME2]"; }
                            break;
                        }
                    case (ColumnType.BIT): { s = "[BIT]"; break; }
                    case (ColumnType.BOOLEAN): { s = "[BIT]"; break; }
                    case (ColumnType.INTEGER):
                        {
                            if (columnLength < 3) { s = "[TINYINT]"; }
                            else if (columnLength < 5) { s = "[SMALLINT]"; }
                            else if (columnLength < 10) { s = "[INT]"; }
                            else { s = "[BIGINT]"; }
                            break;
                        }
                    case (ColumnType.UNDETERMINED): { s = "[???]"; break; }
                }

                s += (nullable) ? " NULL" : " NOT NULL";
            }
            return s;
        }

        /******************************************************
         * IsStringType
        ******************************************************/
        public bool IsStringType()
        {
            return (columnType == ColumnType.STRING || columnType == ColumnType.CHARACTER || columnType == ColumnType.TEXT || columnType == ColumnType.DATE || columnType == ColumnType.DATETIME);
        }

        /******************************************************
         * IsBoolean
        ******************************************************/
        public bool IsBoolean()
        {
            return (columnType == ColumnType.BOOLEAN);
        }

        /******************************************************
         * CombineColumns
         * ----------------------------------------------------
         * When parallelizing the analysis of data, a column
         * may be analyzing multiple values at once and 
         * a function will be needed to consolidate their 
         * determined types 
         * 
         * this was used in the C++ implementation - not
         * sure if I'll add parallelism here..
        ******************************************************/
        public static Column CombineColumns(ref Column c1, ref Column c2)
        {
            Column newColumn = c1;
            if (newColumn.ChangeColumnType(c2.columnType))
            {
                newColumn.columnType = c2.columnType;
            }
            newColumn.columnLength = (c1.columnLength > c2.columnLength) ? c1.columnLength : c2.columnLength;
            newColumn.precision = (c1.precision > c2.precision) ? c1.precision : c2.precision;
            newColumn.nullable = (c1.nullable || c2.nullable);
            newColumn.unicode = (c1.unicode || c2.unicode);
            newColumn.instancesCounted = c1.instancesCounted + c2.instancesCounted;
            newColumn.nullsCounted = c1.nullsCounted + c2.nullsCounted;
            newColumn.columnLengthFrequency = c1.columnLengthFrequency + c2.columnLengthFrequency;
            return newColumn;
        }
    }
}
