namespace FastEnumGenerator;
public static class Colors
{
    extension(string color)
    {
        public string ToWebColor
        {
            get
            {
                if (color == cs1.Transparent)
                {
                    return "none"; //this is how svg shows as transparent
                }
                if (color.Length == 0)
                {
                    throw new CustomBasicException("Had no color");
                }
                if (color.Length != 9)
                {
                    throw new CustomBasicException("Color In Wrong Format");
                }
                if (color.StartsWith("#FF") == false)
                {
                    throw new CustomBasicException("Colors must start with FF so no transparency");
                }
                string output = $"#{color.Substring(3, 6)}";
                return output;
            }
        }
    }
    extension(string thisStr)
    {
        internal bool HasColor
        {
            get
            {
                string thisColor = thisStr.ToColor(false);
                return thisColor != "";
            }   
        }
            
        public string ToColor(bool showError = true)
        {
            switch (thisStr)
            {
                case "Boy":
                    return cs1.Blue;
                case "Girl":
                    return cs1.DeepPink; //just in case you are playing the game of life and it needs to be converted to colors.
                case "AliceBlue":
                    return cs1.AliceBlue;
                case "AntiqueWhite":
                    return cs1.AntiqueWhite;
                case "Aqua":
                    return cs1.Aqua;
                case "Aquamarine":
                    return cs1.Aquamarine;
                case "Azure":
                    return cs1.Azure;
                case "Beige":
                    return cs1.Beige;
                case "Bisque":
                    return cs1.Bisque;
                case "Black":
                    return cs1.Black;
                case "BlanchedAlmond":
                    return cs1.BlanchedAlmond;
                case "Blue":
                    return cs1.Blue;
                case "BlueViolet":
                    return cs1.BlueViolet;
                case "Brown":
                    return cs1.Brown;
                case "BurlyWood":
                    return cs1.BurlyWood;
                case "CadetBlue":
                    return cs1.CadetBlue;
                case "Chartreuse":
                    return cs1.Chartreuse;
                case "Chocolate":
                    return cs1.Chocolate;
                case "Coral":
                    return cs1.Coral;
                case "CornflowerBlue":
                    return cs1.CornflowerBlue;
                case "Cornsilk":
                    return cs1.Cornsilk;
                case "Crimson":
                    return cs1.Crimson;
                case "Cyan":
                    return cs1.Cyan;
                case "DarkBlue":
                    return cs1.DarkBlue;
                case "DarkCyan":
                    return cs1.DarkCyan;
                case "DarkGoldenrod":
                    return cs1.DarkGoldenrod;
                case "DarkGray":
                    return cs1.DarkGray;
                case "DarkGreen":
                    return cs1.DarkGreen;
                case "DarkKhaki":
                    return cs1.DarkKhaki;
                case "DarkMagenta":
                    return cs1.DarkMagenta;
                case "DarkOliveGreen":
                    return cs1.DarkOliveGreen;
                case "DarkOrange":
                    return cs1.DarkOrange;
                case "DarkOrchid":
                    return cs1.DarkOrchid;
                case "DarkRed":
                    return cs1.DarkRed;
                case "DarkSalmon":
                    return cs1.DarkSalmon;
                case "DarkSeaGreen":
                    return cs1.DarkSeaGreen;
                case "DarkSlateBlue":
                    return cs1.DarkSlateBlue;
                case "DarkSlateGray":
                    return cs1.DarkSlateGray;
                case "DarkTurquoise":
                    return cs1.DarkTurquoise;
                case "DarkViolet":
                    return cs1.DarkViolet;
                case "DeepPink":
                    return cs1.DeepPink;
                case "DeepSkyBlue":
                    return cs1.DeepSkyBlue;
                case "DimGray":
                    return cs1.DimGray;
                case "DodgerBlue":
                    return cs1.DodgerBlue;
                case "Firebrick":
                    return cs1.Firebrick;
                case "FloralWhite":
                    return cs1.FloralWhite;
                case "ForestGreen":
                    return cs1.ForestGreen;
                case "Fuchsia":
                    return cs1.Fuchsia;
                case "Gainsboro":
                    return cs1.Gainsboro;
                case "GhostWhite":
                    return cs1.GhostWhite;
                case "Gold":
                    return cs1.Gold;
                case "Goldenrod":
                    return cs1.Goldenrod;
                case "Gray":
                    return cs1.Gray;
                case "Green":
                    return cs1.Green;
                case "GreenYellow":
                    return cs1.GreenYellow;
                case "Honeydew":
                    return cs1.Honeydew;
                case "HotPink":
                    return cs1.HotPink;
                case "IndianRed":
                    return cs1.IndianRed;
                case "Indigo":
                    return cs1.Indigo;
                case "Ivory":
                    return cs1.Ivory;
                case "Khaki":
                    return cs1.Khaki;
                case "Lavender":
                    return cs1.Lavender;
                case "LavenderBlush":
                    return cs1.LavenderBlush;
                case "LawnGreen":
                    return cs1.LawnGreen;
                case "LemonChiffon":
                    return cs1.LemonChiffon;
                case "LightBlue":
                    return cs1.LightBlue;
                case "LightCoral":
                    return cs1.LightCoral;
                case "LightCyan":
                    return cs1.LightCyan;
                case "LightGoldenrodYellow":
                    return cs1.LightGoldenrodYellow;
                case "LightGray":
                    return cs1.LightGray;
                case "LightGreen":
                    return cs1.LightGreen;
                case "LightPink":
                    return cs1.LightPink;
                case "LightSalmon":
                    return cs1.LightSalmon;
                case "LightSeaGreen":
                    return cs1.LightSeaGreen;
                case "LightSkyBlue":
                    return cs1.LightSkyBlue;
                case "LightSlateGray":
                    return cs1.LightSlateGray;
                case "LightSteelBlue":
                    return cs1.LightSteelBlue;
                case "LightYellow":
                    return cs1.LightYellow;
                case "Lime":
                    return cs1.Lime;
                case "LimeGreen":
                    return cs1.LimeGreen;
                case "Linen":
                    return cs1.Linen;
                case "Magenta":
                    return cs1.Magenta;
                case "Maroon":
                    return cs1.Maroon;
                case "MediumAquamarine":
                    return cs1.MediumAquamarine;
                case "MediumBlue":
                    return cs1.MediumBlue;
                case "MediumOrchid":
                    return cs1.MediumOrchid;
                case "MediumPurple":
                    return cs1.MediumPurple;
                case "MediumSeaGreen":
                    return cs1.MediumSeaGreen;
                case "MediumSlateBlue":
                    return cs1.MediumSlateBlue;
                case "MediumSpringGreen":
                    return cs1.MediumSpringGreen;
                case "MediumTurquoise":
                    return cs1.MediumTurquoise;
                case "MediumVioletRed":
                    return cs1.MediumVioletRed;
                case "MidnightBlue":
                    return cs1.MidnightBlue;
                case "MintCream":
                    return cs1.MintCream;
                case "MistyRose":
                    return cs1.MistyRose;
                case "Moccasin":
                    return cs1.Moccasin;
                case "NavajoWhite":
                    return cs1.NavajoWhite;
                case "Navy":
                    return cs1.Navy;
                case "OldLace":
                    return cs1.OldLace;
                case "Olive":
                    return cs1.Olive;
                case "OliveDrab":
                    return cs1.OliveDrab;
                case "Orange":
                    return cs1.Orange;
                case "OrangeRed":
                    return cs1.OrangeRed;
                case "Orchid":
                    return cs1.Orchid;
                case "PaleGoldenrod":
                    return cs1.PaleGoldenrod;
                case "PaleGreen":
                    return cs1.PaleGreen;
                case "PaleTurquoise":
                    return cs1.PaleTurquoise;
                case "PaleVioletRed":
                    return cs1.PaleVioletRed;
                case "PapayaWhip":
                    return cs1.PapayaWhip;
                case "PeachPuff":
                    return cs1.PeachPuff;
                case "Peru":
                    return cs1.Peru;
                case "Pink":
                    return cs1.Pink;
                case "Plum":
                    return cs1.Plum;
                case "PowderBlue":
                    return cs1.PowderBlue;
                case "Purple":
                    return cs1.Purple;
                case "Red":
                    return cs1.Red;
                case "RosyBrown":
                    return cs1.RosyBrown;
                case "RoyalBlue":
                    return cs1.RoyalBlue;
                case "SaddleBrown":
                    return cs1.SaddleBrown;
                case "Salmon":
                    return cs1.Salmon;
                case "SandyBrown":
                    return cs1.SandyBrown;
                case "SeaGreen":
                    return cs1.SeaGreen;
                case "SeaShell":
                    return cs1.SeaShell;
                case "Sienna":
                    return cs1.Sienna;
                case "Silver":
                    return cs1.Silver;
                case "SkyBlue":
                    return cs1.SkyBlue;
                case "SlateBlue":
                    return cs1.SlateBlue;
                case "SlateGray":
                    return cs1.SlateGray;
                case "Snow":
                    return cs1.Snow;
                case "SpringGreen":
                    return cs1.SpringGreen;
                case "SteelBlue":
                    return cs1.SteelBlue;
                case "Tan":
                    return cs1.Tan;
                case "Teal":
                    return cs1.Teal;
                case "Thistle":
                    return cs1.Thistle;
                case "Tomato":
                    return cs1.Tomato;
                case "None":
                case "Transparent":
                    return cs1.Transparent;
                case "Turquoise":
                    return cs1.Turquoise;
                case "Violet":
                    return cs1.Violet;
                case "Wheat":
                    return cs1.Wheat;
                case "White":
                    return cs1.White;
                case "WhiteSmoke":
                    return cs1.WhiteSmoke;
                case "Yellow":
                    return cs1.Yellow;
                case "YellowGreen":
                    return cs1.YellowGreen;
                default:
                    if (showError == true)
                    {
                        throw new CustomBasicException($"No Color Found For {thisStr}");
                    }
                    else
                    {
                        return "";
                    }
            }
        }
    }
    extension<E>(E thisEnum)
        where E: struct, Enum
    {
        public string ToColor
        {
            get
            {
                string thisStr = thisEnum.ToString();
                return thisStr.ToColor(); //for now.
            }
        }
        
    }
}