using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Range2LongestPrefix
{
    public class Program
    {
        public static string MATCH_BINARY_STRING = @"^[01]{32}$";
        public static string ERROR_MSG = @"Expecting two 32-bit binary strings as parameters where the first is smaller than the second.";

        public static List<string> result = new List<string>();

        static void Main(string[] args)
        {
            //Verify Input
            if (args.Length != 2)
            {
                Console.WriteLine("No parameters! " + ERROR_MSG);
                return;
            }
            var a = args[0].ToCharArray();
            var b = args[1].ToCharArray(); 
            if (!Regex.Match(a.toStr(), MATCH_BINARY_STRING).Success 
                || !Regex.Match(a.toStr(), MATCH_BINARY_STRING).Success || 
                Convert.ToInt32(a.toStr(), 2) > Convert.ToInt32(b.toStr(),2))
            {
                Console.WriteLine(ERROR_MSG);
                return;
            }
            Program p = new Program();
            p.GetPrefixes(a, b);
            //System.Diagnostics.Debug.WriteLine(result);
            foreach (var s in result)
            {
                if (!string.IsNullOrEmpty(s)) Console.WriteLine(s);
            }
        }

        public void GetPrefixes(char[] a, char[] b)
        {
            //0-based string will have 32-bit 0-31.
            int lastMatch = 31;
            //Have we reached the end of the prefix a & b that match?
            bool matchEnded = false;
            //Do the ends after the match end in 0s for a and and 1s for b?
            bool endingAllSame = true;
            //Check for the best case: endingAllSame = true after loop. 
            for (int i = 0; i < 32; i++)
            {
                if (!matchEnded)
                {
                    if (a[i] != b[i])
                    {
                        matchEnded = true;
                        lastMatch = i - 1;
                    }
                }
                else if (a[i] != '0' || b[i] != '1')
                {
                    endingAllSame = false;
                }
            }
            if (endingAllSame)
            {
                var response = string.Format("Prefix = {0} (/{1})", a.toStr().Substring(0, lastMatch + 1), lastMatch + 1) +
                    "\r\n" + a.toIpAddr() + (lastMatch + 1);
                result.Add(response);
                return; 
            }
            else
            {
                //split the prefix into two ranges and run this on both ranges:
                var a1 = a.copyStr();
                var b1 = a.fillOnes(lastMatch);
                var a2 = a.fillZeros(lastMatch);
                var b2 = b.copyStr();
                GetPrefixes(a1, b1);
                GetPrefixes(a2, b2);
            }
        }
    }

    #region Extension Methods to char[]

    /// <summary>
    /// Some extension methods to char[] that I found useful. 
    /// </summary>
    public static class Extensions
    {
        public static char[] fillOnes(this char[] str, int index)
        {
            char[] res = new char[str.Length];
            for (int i=0; i<=index; i++)
            {
                res[i] = str[i];
            }
            res[index + 1] = '0';
            for (int i=index+2; i<str.Length; i++)
            {
                res[i] = '1';
            }
            return res;
        }

        public static char[] fillZeros(this char[] str, int index)
        {
            char[] res = new char[str.Length];
            for (int i = 0; i <= index; i++)
            {
                res[i] = str[i];
            }
            res[index + 1] = '1';
            for (int i=index+2; i<str.Length; i++)
            {
                res[i] = '0';
            }
            return res;
        }

        public static string toStr(this char[] str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Because I wanted to see it as an IP address and a binary string. 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string toIpAddr(this char[] str)
        {
            StringBuilder sb = new StringBuilder();
            uint b1 = 0;
            for (int i=0; i< 8; i++)
            {
                b1 = AppendChar(b1, str, i);
            }
            uint b2 = 0;
            for (int i=8; i<16; i++)
            {
                b2 = AppendChar(b2, str, i);
            }
            uint b3 = 0;
            for (int i=16; i<24; i++)
            {
                b3 = AppendChar(b3, str, i);
            }
            uint b4 = 0; 
            for (int i=24; i<32; i++)
            {
                b4 = AppendChar(b4, str, i);
            }
            sb.Append(b1).Append(".").Append(b2).Append(".").Append(b3).Append(".").Append(b4).Append("/");
            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// Boolean logic + shift for creating IP Address
        /// </summary>
        /// <param name="b"></param>
        /// <param name="str"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static uint AppendChar(uint b, char[] str, int i)
        {
            b = b << 1;
            if (i >= str.Length || str[i] == '0')
                b = b | 0;
            else
            {
                b |= 1;
            }
            return b;
        }
        
        public static char[] copyStr(this char[] str)
        {
            char[] res = new char[str.Length];
            for (int i = 0; i < str.Length; i++) res[i] = str[i];
            return res;
        }
    }
}
