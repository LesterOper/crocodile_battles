using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Library.Text
{
    public static class TextUtils
    {
        private static readonly List<string> NUMBER_LETTERS = new List<string>
            {"K", "M", "B", "T", "q", "Q", "s", "S", "O", "N"};

        public static double Round(double rawNumber)
        {
            if (rawNumber < 100) return rawNumber;

            var power10 = (int) Math.Floor(Math.Log(10, rawNumber));
            var roundBase = Math.Pow(10, power10 - 1);
            return (long) (Math.Round(rawNumber / roundBase) * roundBase);
        }

        public static string FormatShort(long rawNumber)
        {
            if (rawNumber < 1000) return rawNumber.ToString();

            var rawPower10 = Mathf.Log(rawNumber, 10);
            var power10 = (int) Mathf.Floor(rawPower10);
            var level = power10 / 3;

            if (power10 % 3 == 0)
            {
                var roundBase = Math.Pow(10, power10 - 1);
                var roundedPreparedNumber = (long) Math.Round(rawNumber / roundBase);
                if (roundedPreparedNumber % 10 == 0) return roundedPreparedNumber / 10 + NUMBER_LETTERS[level - 1];

                return string.Format("{0:0.#}", roundedPreparedNumber / 10f) + NUMBER_LETTERS[level - 1];
            }
            else
            {
                var roundBase = Math.Pow(10, level * 3);
                var roundedPreparedNumber = (long) Math.Round(rawNumber / roundBase);
                return roundedPreparedNumber + NUMBER_LETTERS[level - 1];
            }
        }

        public static string FormatLong(long rawNumber)
        {
            return rawNumber.ToString(NumberFormatInfo.CurrentInfo);
        }

        public static string FormatDouble(double rawNumber)
        {
            return rawNumber.ToString(NumberFormatInfo.CurrentInfo);
        }
    }
}