using Microsoft.VisualBasic;
using System;

namespace DelimitedFileAnalyzer
{

    internal class Utilities
    {

        public static bool IsUpperCaseLetter(in char c) { return (c >= 'A' && c <= 'Z'); }
        public static bool IsLowerCaseLetter(in char c) { return (c >= 'a' && c <= 'z'); }
        public static bool IsLetter(in char c) { return IsUpperCaseLetter(c) || IsLowerCaseLetter(c); }
        public static bool IsDigit(in char c) { return (c >= '0' && c <= '9'); }
        public static bool IsAcceptableColumnCharacter(in char c) { return (IsLetter(c) || IsDigit(c)); }
        public static char ToUpperCaseLetter(in char c) { return IsLowerCaseLetter(c) ? (char)(c - ' ') : c; }
        public static char ToLowerCaseLetter(in char c) { return IsUpperCaseLetter(c) ? (char)(c + ' ') : c; }

        /******************************************************
         * CountSubStringInString
         * ----------------------------------------------------
         * Function to count the number of substrings in a 
         * string with the option of a qualifier
        ******************************************************/
        public static int CountSubstringInString(in string str, in string subStr, in string qualifier, bool inQualifiedField)
        {
            bool justActivatedQualifiedField;
            int i = 0;
            int shifter;
            int frequency = 0;
            string s1, s2;
            while (i < str.Length)
            {
                justActivatedQualifiedField = false;
                shifter = 1;
                /* Handle Qualifer stuff */
                if (qualifier != "")
                {
                    if ((i + qualifier.Length) <= str.Length)
                    {
                        s1 = str.Substring(i, qualifier.Length);
                    }
                    else
                    {
                        s1 = "";
                    }

                    if ((i + 2 * qualifier.Length) <= str.Length)
                    {
                        s2 = str.Substring(i + qualifier.Length, qualifier.Length);
                    }
                    else
                    {
                        s2 = "";
                    }

                    /* ....."data"... */
                    if (s1 == qualifier && !inQualifiedField)
                    {
                        inQualifiedField = true;
                        justActivatedQualifiedField = true;
                    }
                    /* ....." Hello ""Good Sir""..."...*/
                    else if (s1 == qualifier && s2 == qualifier && inQualifiedField)
                    {
                        /* Move on */
                        shifter = (qualifier.Length * 2);
                        s1 = "";
                    }
                    /* ....."data"... */
                    else if (s1 == qualifier && !(s2 == qualifier) && inQualifiedField)
                    {
                        /* Move on */
                        inQualifiedField = false;
                        justActivatedQualifiedField = true;
                    }
                }
                s1 = str.Substring(i, subStr.Length);

                /* Check for sub string */
                if (s1 != "" && !inQualifiedField && !justActivatedQualifiedField)
                {
                    if (s1 == subStr)
                    {
                        frequency += 1;
                    }
                }
                i += shifter;
            }
            return frequency;
        }

        /* function overload to call the function without providing confusing parameter */
        public static int CountSubstringInString(in string str, in string subStr, in string qualifier)
        {
            return CountSubstringInString(str, subStr, qualifier, false);
        }

        /******************************************************
         * SplitString
         * ----------------------------------------------------
         * Function to determine if a string represents a 
         * positive integer
        ******************************************************/
        public static string[] SplitString(in string str, in string delimiter, in string qualifier)
        {

            if(qualifier == "")
            {
                return str.Split(delimiter);
            }

            List<string> strings = new List<string>();
            if (delimiter == qualifier)
            {
                MessageBox.Show("Delimiter and qualifer are the same string. Data impossible to interpet.");
                return strings.ToArray();
            }

            bool inQualifiedField = false;
            bool foundQualifier;
            string currentString = "";
            string s1, s2;
            int i = 0;
            int shifter;
            while (i < str.Length)
            {
                foundQualifier = false;
                shifter = 1;
                /* Handle Qualifer stuff */
                if (qualifier != "")
                {
                    if ((i + qualifier.Length) <= str.Length)
                    {
                        s1 = str.Substring(i, qualifier.Length);
                    }
                    else
                    {
                        s1 = "";
                    }

                    if ((i + 2 * qualifier.Length) <= str.Length)
                    {
                        s2 = str.Substring(i + qualifier.Length, qualifier.Length);
                    }
                    else
                    {
                        s2 = "";
                    }

                    /* ....."data"... */
                    if ((s1 == qualifier) && !inQualifiedField)
                    {
                        inQualifiedField = true;
                        shifter = (qualifier.Length);
                        foundQualifier = true;
                    }
                    /* ....." Hello ""Good Sir""..."...*/
                    else if ((s1 == qualifier) && (s2 == qualifier) && inQualifiedField)
                    {
                        /* Move on */
                        shifter = (qualifier.Length * 2);
                        currentString += s1;
                        foundQualifier = true;
                    }
                    /* ....."data"... */
                    else if ((s1 == qualifier) && !(s2 == qualifier) && inQualifiedField)
                    {
                        /* Move on */
                        inQualifiedField = false;
                        shifter = (qualifier.Length);
                        foundQualifier = true;
                    }

                    if (foundQualifier)
                    {
                        i += shifter;
                        continue;
                    }
                }

                s1 = str.Substring(i, 1);

                currentString += s1;

                if (currentString.Length >= delimiter.Length)
                {
                    if ((currentString.Substring(currentString.Length - delimiter.Length, delimiter.Length) == delimiter) && (!inQualifiedField))
                    {
                        currentString = currentString.Substring(0, currentString.Length - delimiter.Length);
                        strings.Add(currentString);
                        currentString = "";
                    }
                }

                i += shifter;
            }
            strings.Add(currentString);
            return strings.ToArray();
        }

        /******************************************************
         * IsPositiveInt
         * ----------------------------------------------------
         * Function to determine if a string represents a 
         * positive integer
        ******************************************************/
        public static bool IsPositiveInt(in string s)
        {
            if (s.Length == 0) { return false; }
            for (int i = 0; i < s.Length; i++)
            {
                if (!IsDigit(s[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /******************************************************
         * IsPositiveDecimal
         * ----------------------------------------------------
         * Function to determine if a string represents a 
         * positive decimal
        ******************************************************/
        public static bool IsPositiveDecimal(in string s)
        {
            if (s.Length == 0) { return false; }
            bool foundDecimal = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (!IsDigit(s[i]))
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

        /******************************************************
         * ToLowerCaseString
         * ----------------------------------------------------
         * Function to convert a string to lower case
        ******************************************************/
        public static string ToLowerCaseString(in string s)
        {
            string newS = "";
            for (int i = 0; i < s.Length; i++)
            {
                newS += (IsUpperCaseLetter(s[i]) ? ToLowerCaseLetter(s[i]) : s[i]);
            }
            return newS;
        }

        /******************************************************
         * ContainsUnicodeChars
         * ----------------------------------------------------
         * Function to determine if characters outside of the 
         * ascii table exist in the string
        ******************************************************/
        public static bool ContainsUnicodeChars(in string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (((int)str[i]) > 127)
                {
                    return true;
                }
            }
            return false;
        }

        /******************************************************
         * ConvertStringToBool
         * ----------------------------------------------------
         * Function to convert a string value to a boolean
        ******************************************************/
        public static bool ConvertStringToBool(string b)
        {
            b = ToLowerCaseString(b);
            if (b == "t" || b == "true" || b == "1")
            {
                return true;
            }
            return false;
        }

        /******************************************************
         * ConvertStringToBool
         * ----------------------------------------------------
         * Function to convert a string value to a boolean
        ******************************************************/
        public static bool StringEndsWithinQualifiedField(in string str, in string qualifier)
        {
            return (CountSubstringInString(str, qualifier, "") % 2) == 1;
        }

        /******************************************************
         * ConvertStringToBool
         * ----------------------------------------------------
         * Function to convert a string value to a boolean
        ******************************************************/
        public static bool IsWhiteSpace(in char c)
        {
            return (c == ' ') || (c == '\r') || (c == '\n') || (c == '\t');
        }

        /******************************************************
         * ConvertStringToBool
         * ----------------------------------------------------
         * Function to convert a string value to a boolean
        ******************************************************/
        public static string TrimLeftWhiteSpace(in string s)
        {
            int startIndex = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (!IsWhiteSpace(s[i]))
                {
                    startIndex = i;
                    break;
                }
            }
            return s.Substring(startIndex, s.Length - startIndex);
        }

        /******************************************************
         * ConvertStringToBool
         * ----------------------------------------------------
         * Function to convert a string value to a boolean
        ******************************************************/
        public static string TrimRightWhiteSpace(in string s)
        {
            if (s.Length == 0)
            {
                return s;
            }

            int newLength = s.Length;
            for (int i = s.Length; i > 0; i--)
            {
                if (!IsWhiteSpace(s[i - 1]))
                {
                    newLength = i;
                    break;
                }
            }
            return s.Substring(0, newLength);
        }

        /******************************************************
         * ConvertStringToBool
         * ----------------------------------------------------
         * Function to convert a string value to a boolean
        ******************************************************/
        public static string TrimWhiteSpace(in string s)
        {
            return TrimLeftWhiteSpace(TrimRightWhiteSpace(s));
        }

        /******************************************************
         * ConvertBooleanValueToString
         * ----------------------------------------------------
         * Function to convert a bool value to a string
         * 
         * Used in the SQL generation because no bools
         * in T-SQL, just bits
        ******************************************************/
        public static string ConvertBooleanValueToString(string s)
        {
            s = Utilities.ToLowerCaseString(s);
            if (s == "1" || s == "true") { return "1"; }
            if (s == "0" || s == "false") { return "0"; }

            MessageBox.Show("Tried to represent a non-boolean value as boolean. Value: " + s);
            s = "";
            return s;
        }
    }
}