using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelimitedFileAnalyzer
{
    enum DateFormat
    {
        YYYYMMDD
        , YYYYDDMM
        , MMYYYYDD
        , MMDDYYYY
        , DDMMYYYY
        , DDYYYYMM
        , UNSURE
        , INVALID
    };

    /**********************************************************************************************
    ###############################################################################################
    ##### Date Format Analyzer 
    ###############################################################################################
    **********************************************************************************************/
    internal class DateFormatAnalyzer
    {
        /* Members */
        private DateFormat[] possibleDateFormats = Array.Empty<DateFormat>();
        private DateFormat dateFormat = DateFormat.UNSURE;
        private bool hasTime = false;
        private string delimiter = "";

        /******************************************************
         * Is Date Type
         * ----------------------------------------------------
         * Basic functions to help identify if an 
         * object of this class is some type of date
         * 
         * An object of this class will be instantiated 
         * when we need to determine if a string is a
         * date or not. If it is a date, then we will
         * do further analysis on it's possible formats 
         * and store them in the analyzer object
        ******************************************************/
        public bool IsNotDate() { return (dateFormat == DateFormat.INVALID); }
        public bool IsDate() { return (dateFormat != DateFormat.INVALID && dateFormat != DateFormat.UNSURE && !hasTime); }
        public bool IsDateTime() { return (dateFormat != DateFormat.INVALID && dateFormat != DateFormat.UNSURE && hasTime); }
        public bool IsPossibleDate() { return (dateFormat != DateFormat.INVALID && (!hasTime)); }
        public bool IsPossibleDateTime() { return (dateFormat != DateFormat.INVALID && hasTime); }

        /******************************************************
         * IsPossible
         * ----------------------------------------------------
         * Basic functions to help identify possible 
         * date values
        ******************************************************/
        private static bool IsPossibleDay(in int d) { return (d > 0 && d < 32); }
        private static bool IsPossibleMonth(in int m) { return (m > 0 && m < 13); }
        private static bool IsPossibleYear(in int y) { return (y >= 0); }

        /******************************************************
         * Is time helpers
         * ----------------------------------------------------
         * Basic functions to help identify possible 
         * time values
        ******************************************************/
        private static bool IsHour(int h) { return (h >= 0 && h <= 23); }
        private static bool IsMinute(int m) { return (m >= 0 && m < 60); }
        private static bool IsSecond(double s) { return (s >= 0.0 && s < 60.0); }

        /******************************************************
         * Exists
         * ----------------------------------------------------
         * Function to check for existence of item in array
        ******************************************************/
        public static bool Exists(in DateFormat[] arr, in DateFormat item)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (item == arr[i])
                {
                    return true;
                }
            }
            return false;
        }

        /******************************************************
         * IsDate
         * ----------------------------------------------------
         * Function to determine if a series of integers
         * represents a valid date
        ******************************************************/
        private static bool IsDate(in int y, in int m, in int d)
        {
            /* quick filtering for obvious non-dates */
            if (!IsPossibleYear(y)) { return false; }
            if (!IsPossibleMonth(m)) { return false; }
            if (!IsPossibleDay(d)) { return false; }

            /* next, check each month */
            switch (m)
            {
                /* these months have 31 days */
                case (1):
                case (3):
                case (5):
                case (7):
                case (8):
                case (10):
                case (12):
                    {
                        return ((d >= 1) && (d <= 31));
                    }
                /* feburary including leap years */
                case (2):
                    {
                        return ((d >= 1) && (d <= 28)) || ((d == 29) && ((y % 4) == 0));
                    }
                /* remainder of months only have 30 days */
                default:
                    {
                        return ((d >= 1) && (d <= 30));
                    }
            }
        }

        /******************************************************
         * IsTime
         * ----------------------------------------------------
         * Function to determine if a string represents a time
         * Acceptable formats:
         *  HH:MM:SS.dddd...ddd PM/AM
         *  HH:MM:SS.dddd...ddd
        ******************************************************/
        private static bool IsTime(in string t)
        {
            int numSpaces = Utilities.CountSubstringInString(t, " ", "");
            bool am_pm_included = false;
            bool pm = false;
            string timeString;
            if (numSpaces == 1)
            {
                string[] tp = Utilities.SplitString(t, " ", "");
                string amPm = Utilities.ToLowerCaseString(tp[1]);
                if (amPm == "pm")
                {
                    am_pm_included = true;
                    pm = true;
                }
                else if (amPm == "am")
                {
                    am_pm_included = true;
                }
                else
                {
                    return false;
                }
                timeString = tp[0];
            }
            else if (numSpaces > 1)
            {
                return false;
            }
            else
            {
                timeString = t;
            }

            string[] timeParts = Utilities.SplitString(timeString, ":", "");
            if (timeParts.Length != 3)
            {
                return false;
            }

            int h = (Utilities.IsPositiveInt(timeParts[0])) ? Int32.Parse(timeParts[0]) : -1;
            int m = (Utilities.IsPositiveInt(timeParts[1])) ? Int32.Parse(timeParts[1]) : -1;
            double s = (Utilities.IsPositiveDecimal(timeParts[2])) ? double.Parse(timeParts[2]) : -1.0;

            if (h == -1 || m == -1 || s == -1.0)
            {
                return false;
            }

            if (am_pm_included && pm)
            {
                h += 12;
            }

            return (IsHour(h) && IsMinute(m) && IsSecond(s));
        }


        /******************************************************
         * GetPossibleDateFormats
         * ----------------------------------------------------
         * Function to find all possible formats that a 
         * string that looks like a date could be 
         * 
         * For example, 01-01-01 could be like 3 different 
         * dates 
        ******************************************************/
        private static DateFormat[] GetPossibleDateFormats(in string d, ref string delimiter)
        {
            List<DateFormat> df = new List<DateFormat>();

            // Count number of delimiters
            bool identifiedDelimiter = false;
            int nonDigitChars = 0;
            int digitChars = 0;
            // For each char in the string
            for (int i = 0; i < d.Length; i += 1)
            {
                if (Utilities.IsDigit(d[i])) // If this is a digit
                {
                    digitChars += 1; // Count and continue
                }
                else // Else, not a digit
                {
                    if (!identifiedDelimiter) // If we have not found a delimiter yet
                    {
                        identifiedDelimiter = true; // Indicate that we have a delimiter
                        delimiter = d[i].ToString(); // Save it
                        nonDigitChars += 1; // Count and continue
                    }
                    else // We already found a delimiter earlier
                    {
                        if (d[i].ToString() != delimiter) // if they are not the same
                        {
                            return df.ToArray(); // Return false..?                     
                        }
                        else // They are the same
                        {
                            nonDigitChars += 1; // Count and continue
                        }
                    }
                }
            }
            // Need at least 4 to 8 digits (YYMD or YYYYMMDD) AND Need exactly 2 delimiters
            if (digitChars < 4 || digitChars > 8 || nonDigitChars != 2 || delimiter.Length != 1)
            {
                return df.ToArray();
            }

            // Now get date parts
            string[] dateParts = Utilities.SplitString(d, delimiter, "");
            int datePart1 = (Utilities.IsPositiveInt(dateParts[0])) ? Int32.Parse(dateParts[0]) : -1;
            int datePart2 = (Utilities.IsPositiveInt(dateParts[1])) ? Int32.Parse(dateParts[1]) : -1;
            int datePart3 = (Utilities.IsPositiveInt(dateParts[2])) ? Int32.Parse(dateParts[2]) : -1;

            // If failed return
            if (datePart1 == -1 || datePart2 == -1 || datePart3 == -1)
            {
                return df.ToArray();
            }
            // Get possible date formats
            if (IsDate(datePart1, datePart2, datePart3))
            { // YYYY MM DD
                df.Add(DateFormat.YYYYMMDD);
            }
            if (IsDate(datePart1, datePart3, datePart2))
            { // YYYY DD MM
                df.Add(DateFormat.YYYYDDMM);
            }
            if (IsDate(datePart2, datePart1, datePart3))
            { // MM YYYY DD
                df.Add(DateFormat.MMYYYYDD);
            }
            if (IsDate(datePart3, datePart1, datePart2))
            { // MM DD YYYY
                df.Add(DateFormat.MMDDYYYY);
            }
            if (IsDate(datePart3, datePart2, datePart1))
            { // DD MM YYYY
                df.Add(DateFormat.DDMMYYYY);
            }
            if (IsDate(datePart2, datePart3, datePart1))
            { // DD YYYY MM
                df.Add(DateFormat.DDYYYYMM);
            }
            return df.ToArray();
        }

        /******************************************************
         * SplitDateParts
         * ----------------------------------------------------
         * Function to split a date into integer parts
         * given a format (and assuming that string follows 
         * a date format already)
        ******************************************************/
        void SplitDateParts(in string s, ref int y, ref int m, ref int d, DateFormat df)
        {

            string[] dateParts = Utilities.SplitString(s, delimiter, "");

            y = (df == DateFormat.YYYYDDMM || df == DateFormat.YYYYMMDD) ? Int32.Parse(dateParts[0]) :

                ((df == DateFormat.DDMMYYYY || df == DateFormat.MMDDYYYY) ? Int32.Parse(dateParts[2]) : Int32.Parse(dateParts[1]));

            m = (df == DateFormat.MMDDYYYY || df == DateFormat.MMYYYYDD) ? Int32.Parse(dateParts[0]) :

                ((df == DateFormat.DDYYYYMM || df == DateFormat.YYYYDDMM) ? Int32.Parse(dateParts[2]) : Int32.Parse(dateParts[1]));

            d = (df == DateFormat.DDMMYYYY || df == DateFormat.DDYYYYMM) ? Int32.Parse(dateParts[0]) :

                ((df == DateFormat.YYYYMMDD || df == DateFormat.MMYYYYDD) ? Int32.Parse(dateParts[2]) : Int32.Parse(dateParts[1]));
        }

        /******************************************************
         * ProcessDate
         * ----------------------------------------------------
         * Function to intake a possible date string,
         * determine if it is a date and/or time, and if so
         * what format it follows
        ******************************************************/
        public void ProcessDate(in string d)
        {
            // If already invalid, dont process
            if (dateFormat == DateFormat.INVALID) { return; }
            // If string does not meet minimum length, dont process.
            if (d.Length < 6)
            {
                dateFormat = DateFormat.INVALID;
                return;
            }
            // Check for time
            string dateString = "";
            string timeString = "";
            int numSpaces = Utilities.CountSubstringInString(d, " ", "");
            if (numSpaces == 1 || numSpaces == 2)
            { // If there might be a time here
                string[] dateParts = Utilities.SplitString(d, " ", ""); // split
                dateString = dateParts[0]; // grab the date part
                timeString = dateParts[1]; // grab the time part
                if (numSpaces == 2)
                {
                    timeString += " " + dateParts[2];
                }
                hasTime = true;
            }
            else
            {
                dateString = d;
            }
            // If time identified and it is not valid, dont process
            if (hasTime && !IsTime(timeString))
            {
                hasTime = false;
                dateFormat = DateFormat.INVALID;
                return;
            }
            // Grab all possible formats for the date
            DateFormat[] df = GetPossibleDateFormats(dateString, ref delimiter);

            if (df.Length > 0)
            {
                /* If date format already determined */
                if (dateFormat != DateFormat.UNSURE)
                {
                    if (!Exists(df, dateFormat))
                    { // if this format is not in the possible formats
                        dateFormat = DateFormat.INVALID; // done, dont process
                        return;
                    }
                    else
                    {
                        return; // We are done, no changes
                    }
                }
                else
                { // dateFormat is still unsure
                    if (possibleDateFormats.Length > 0)
                    { // If there are multiple possible formats
                        for (int i = 0; i < df.Length; i++)
                        { // for each new format
                            if (!Exists(possibleDateFormats, df[i]))
                            { // if the new format is not found in the old formats
                                dateFormat = DateFormat.INVALID; // then this is not a possible date
                                return;
                            }
                        }
                        // if we have reached this point, then all new formats were found in the old formats
                        possibleDateFormats = df;
                    }
                    else
                    {
                        possibleDateFormats = df;
                    }
                }

                // If narrowed down to 1, we have the only possible date format.
                if (possibleDateFormats.Length == 1)
                {
                    dateFormat = possibleDateFormats[0];
                }
            }
            else
            {
                dateFormat = DateFormat.INVALID;
                return;
            }
        }

    }
}
