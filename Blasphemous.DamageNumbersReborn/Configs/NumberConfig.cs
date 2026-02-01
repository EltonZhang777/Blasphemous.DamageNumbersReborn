using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Configs;
public class NumberConfig
{
    public bool enabled = true;
    public string fontName = "MajesticExtended_Pixel_Scroll";
    public int fontSize = 16;
    public string outlineColor = "#FFFFFF";
    public int poolSize = 50;

    internal Color OutlineColor => MasterConfig.ParseHtmlToColorOrWhite(outlineColor);
}
