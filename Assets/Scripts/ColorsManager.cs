using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Colors
{
    Green,
    Red,
}

public static class ColorsManager
{
    public static string Blob;
    public static Color Black { get; private set; } = new Color32(21, 21, 21, 0xff);
    public static Color Blue { get; private set; } = new Color32(76, 96, 135, 0xff);
    public static Color Green { get; private set; } = new Color32(65, 139, 87, 0xff);
    public static Color Cyan { get; private set; } = new Color32(68, 140, 143, 0xff);
    public static Color Red { get; private set; } = new Color32(174, 75, 103, 0xff);
    public static Color Magenta { get; private set; } = new Color32(108, 91, 123, 0xff);
    public static Color Brown { get; private set; } = new Color32(128, 104, 98, 0xff);
    public static Color LightGray { get; private set; } = new Color32(83, 81, 79, 0xff);
    public static Color DarkGray { get; private set; } = new Color32(62, 60, 66, 0xff);
    public static Color LightBlue { get; private set; } = new Color32(129, 148, 184, 0xff);
    public static Color LightGreen { get; private set; } = new Color32(134, 194, 139, 0xff);
    public static Color LightCyan { get; private set; } = new Color32(130, 214, 213, 0xff);
    public static Color LightRed { get; private set; } = new Color32(213, 130, 147, 0xff);
    public static Color LightMagenta { get; private set; } = new Color32(159, 144, 173, 0xff);
    public static Color Yellow { get; private set; } = new Color32(240, 180, 158, 0xff);
    public static Color White { get; private set; } = new Color32(255, 250, 232, 0xff);

    public static Color GetColor(Colors color)
    {
        if (color == Colors.Green)
        {
            return Green;
        }

        if (color == Colors.Red)
        {
            return Red;
        }

        return White;
    }
}

public static class ColorExtensions
{
    public static Color WithAlpha(this Color color, float a)
    {
        return new Color(color.r, color.g, color.b, a);
    }

    public static Color Transparent(this Color color)
    {
        return color.WithAlpha(0);
    }

    public static Color Opaque(this Color color)
    {
        return color.WithAlpha(1);
    }
}