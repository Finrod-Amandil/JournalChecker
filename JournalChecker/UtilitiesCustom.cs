/*
 * UTILITIESCUSTOM.CS
 * JournalChecker module
 * 
 * Module description:
 * -------------------
 * UtilitiesCustom.cs contains the following functionalities:
 * - Multiple auxiliary methods
 * - Enum for User Interface
 * - Wagner-Fischer-Algorithm for calculating Levenshtein distance to determine typos
 * - Method to replace some special characters which may appear in proper name but not in filename
 * - Method to update config file
 * 
 */

using System;
using System.Xml;

namespace UtilitiesCustom
{
    public enum IFace
    {
        CMD,
        GUI,
        UNKNOWN
    }

    class Utilities
    {
        public static int LevenshteinDistance(string s, string t)
        {
            //Wagner-Fischer-Algorithm for calculcating Levenshtein distance
            //s, t: Strings to compare
            //n, m: Length of strings
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            //Special case: If one of the strings is empty, return length of other string
            if (n == 0) return m;
            if (m == 0) return n;

            //Initialise first row and column of matrix
            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            //Fill Matrix using algorithm
            for (int j = 1; j <= m; j++)
            {
                for (int i = 1; i <= n; i++)
                {
                    if (s[i - 1] == t[j - 1])
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        d[i, j] = Math.Min(Math.Min(
                            d[i - 1, j] + 1,
                            d[i, j - 1] + 1),
                            d[i - 1, j - 1] + 1);
                    }
                }
            }

            return d[n, m];
        }

        public static string ReplaceSpecialChars(string text)
        {
            string[,] hash = new string[,] {{ "é", "e" },{ "è", "e" },{ "ê", "e" },{ "ë", "e" },
                                            { "á", "a" },{ "à", "a" },{ "â", "a" },{ "ä", "ae" },{ "ã", "a" },{ "æ", "ae" },
                                            { "í", "i" },{ "ì", "i" },{ "î", "i" },{ "ï", "i" },
                                            { "ó", "o" },{ "ò", "o" },{ "ô", "o" },{ "ö", "oe" }, {"õ", "o" },{ "œ", "oe" },
                                            { "ú", "u" },{ "ù", "a" },{ "û", "u" },{ "ü", "ue" },

                                            { "É", "E" },{ "È", "E" },{ "Ê", "E" },{ "Ë", "E" },
                                            { "Á", "A" },{ "À", "A" },{ "Â", "A" },{ "Ä", "Ae" },{ "Ã", "A" },{ "Æ", "Ae" },
                                            { "Í", "I" },{ "Ì", "I" },{ "Î", "I" },{ "Ï", "I" },
                                            { "Ó", "O" },{ "Ò", "O" },{ "Ô", "O" },{ "Ö", "Oe" }, { "Õ",  "O" },{ "Œ", "Oe" },
                                            { "Ú", "U" },{ "Ù", "U" },{ "Û", "U" },{ "Ü", "Ue" },

                                            { "ç", "c" },{ "Ç", "C" },{ "ñ", "n" },{ "Ñ", "N" },{ "ß", "ss" }};
            for (int i = 0; i < text.Length; i++)
            {
                for (int j = 0; j < hash.GetLength(0); j++)
                {
                    if (text.Substring(i, 1).Equals(hash[j, 0]))
                    {
                        text = text.Replace(hash[j, 0], hash[j, 1]);
                        break;
                    }
                }
            }

            return text;
        }

        public static void UpdateAppSettings(string keyName, string keyValue)
        {
            XmlDocument doc = new XmlDocument();

            //Load config file
            doc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            //Iterate through elements
            foreach (XmlElement elem in doc.DocumentElement)
            {
                //Find appSettings element
                if (elem.Name == "appSettings")
                {
                    foreach (XmlNode node in elem.ChildNodes)
                    {
                        //Find attribute with key to be updated
                        if (node.Attributes[0].Value == keyName)
                        {
                            //Update value
                            node.Attributes[1].Value = keyValue;
                        }
                    }
                }
            }
            //Save
            doc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }
    }
}
