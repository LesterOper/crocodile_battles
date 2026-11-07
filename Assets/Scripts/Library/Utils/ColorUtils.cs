using System;
using UnityEngine;

namespace Library.Utils
{
    public static class ColorUtils
    {
        public static Color CreateColor(uint color)
        {
            var bytes = BitConverter.GetBytes(color);
            return new Color(bytes[3] / 255f, bytes[2] / 255f, bytes[1] / 255f, bytes[0] / 255f);
        }
    }
}