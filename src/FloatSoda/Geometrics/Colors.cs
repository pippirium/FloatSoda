namespace FloatSoda.Geometrics;

/// <summary>CSS の名前付きの色を <see cref="Color"/> として並べた定数です。</summary>
/// <remarks>
/// <para>
/// 名前と値は、CSS Color Module Level 4 の名前付きの色に揃えています。
/// 綴りが Grey の別名7つ(<c>grey</c>、<c>darkgrey</c>、<c>lightgrey</c>、<c>dimgrey</c>、<c>slategrey</c>、<c>darkslategrey</c>、<c>lightslategrey</c>)と <c>rebeccapurple</c> は含めず、透明な黒の <see cref="Transparent"/> を足しています。
/// 名前の集まりは SkiaSharp の <c>SKColors</c> と同じなので、<c>SKColors.Red</c> は <c>Colors.Red</c> に書き換えられます。
/// </para>
/// <para>
/// 値が <c>SKColors</c> と違う色が2つあります。<see cref="DarkSeaGreen"/> は CSS の <c>#8FBC8F</c> で、<c>SKColors.DarkSeaGreen</c> は <c>#8FBC8B</c> です。
/// <see cref="Transparent"/> は CSS と同じ透明な黒で、<c>SKColors.Transparent</c> は透明な白です。
/// </para>
/// </remarks>
public static class Colors
{
    /// <summary>CSS の <c>aliceblue</c>(<c>#F0F8FF</c>)です。</summary>
    public static readonly Color AliceBlue = Color.FromArgb(0xFFF0F8FF);

    /// <summary>CSS の <c>antiquewhite</c>(<c>#FAEBD7</c>)です。</summary>
    public static readonly Color AntiqueWhite = Color.FromArgb(0xFFFAEBD7);

    /// <summary>CSS の <c>aqua</c>(<c>#00FFFF</c>)です。</summary>
    public static readonly Color Aqua = Color.FromArgb(0xFF00FFFF);

    /// <summary>CSS の <c>aquamarine</c>(<c>#7FFFD4</c>)です。</summary>
    public static readonly Color Aquamarine = Color.FromArgb(0xFF7FFFD4);

    /// <summary>CSS の <c>azure</c>(<c>#F0FFFF</c>)です。</summary>
    public static readonly Color Azure = Color.FromArgb(0xFFF0FFFF);

    /// <summary>CSS の <c>beige</c>(<c>#F5F5DC</c>)です。</summary>
    public static readonly Color Beige = Color.FromArgb(0xFFF5F5DC);

    /// <summary>CSS の <c>bisque</c>(<c>#FFE4C4</c>)です。</summary>
    public static readonly Color Bisque = Color.FromArgb(0xFFFFE4C4);

    /// <summary>CSS の <c>black</c>(<c>#000000</c>)です。</summary>
    public static readonly Color Black = Color.FromArgb(0xFF000000);

    /// <summary>CSS の <c>blanchedalmond</c>(<c>#FFEBCD</c>)です。</summary>
    public static readonly Color BlanchedAlmond = Color.FromArgb(0xFFFFEBCD);

    /// <summary>CSS の <c>blue</c>(<c>#0000FF</c>)です。</summary>
    public static readonly Color Blue = Color.FromArgb(0xFF0000FF);

    /// <summary>CSS の <c>blueviolet</c>(<c>#8A2BE2</c>)です。</summary>
    public static readonly Color BlueViolet = Color.FromArgb(0xFF8A2BE2);

    /// <summary>CSS の <c>brown</c>(<c>#A52A2A</c>)です。</summary>
    public static readonly Color Brown = Color.FromArgb(0xFFA52A2A);

    /// <summary>CSS の <c>burlywood</c>(<c>#DEB887</c>)です。</summary>
    public static readonly Color BurlyWood = Color.FromArgb(0xFFDEB887);

    /// <summary>CSS の <c>cadetblue</c>(<c>#5F9EA0</c>)です。</summary>
    public static readonly Color CadetBlue = Color.FromArgb(0xFF5F9EA0);

    /// <summary>CSS の <c>chartreuse</c>(<c>#7FFF00</c>)です。</summary>
    public static readonly Color Chartreuse = Color.FromArgb(0xFF7FFF00);

    /// <summary>CSS の <c>chocolate</c>(<c>#D2691E</c>)です。</summary>
    public static readonly Color Chocolate = Color.FromArgb(0xFFD2691E);

    /// <summary>CSS の <c>coral</c>(<c>#FF7F50</c>)です。</summary>
    public static readonly Color Coral = Color.FromArgb(0xFFFF7F50);

    /// <summary>CSS の <c>cornflowerblue</c>(<c>#6495ED</c>)です。</summary>
    public static readonly Color CornflowerBlue = Color.FromArgb(0xFF6495ED);

    /// <summary>CSS の <c>cornsilk</c>(<c>#FFF8DC</c>)です。</summary>
    public static readonly Color Cornsilk = Color.FromArgb(0xFFFFF8DC);

    /// <summary>CSS の <c>crimson</c>(<c>#DC143C</c>)です。</summary>
    public static readonly Color Crimson = Color.FromArgb(0xFFDC143C);

    /// <summary>CSS の <c>cyan</c>(<c>#00FFFF</c>)です。</summary>
    public static readonly Color Cyan = Color.FromArgb(0xFF00FFFF);

    /// <summary>CSS の <c>darkblue</c>(<c>#00008B</c>)です。</summary>
    public static readonly Color DarkBlue = Color.FromArgb(0xFF00008B);

    /// <summary>CSS の <c>darkcyan</c>(<c>#008B8B</c>)です。</summary>
    public static readonly Color DarkCyan = Color.FromArgb(0xFF008B8B);

    /// <summary>CSS の <c>darkgoldenrod</c>(<c>#B8860B</c>)です。</summary>
    public static readonly Color DarkGoldenrod = Color.FromArgb(0xFFB8860B);

    /// <summary>CSS の <c>darkgray</c>(<c>#A9A9A9</c>)です。</summary>
    public static readonly Color DarkGray = Color.FromArgb(0xFFA9A9A9);

    /// <summary>CSS の <c>darkgreen</c>(<c>#006400</c>)です。</summary>
    public static readonly Color DarkGreen = Color.FromArgb(0xFF006400);

    /// <summary>CSS の <c>darkkhaki</c>(<c>#BDB76B</c>)です。</summary>
    public static readonly Color DarkKhaki = Color.FromArgb(0xFFBDB76B);

    /// <summary>CSS の <c>darkmagenta</c>(<c>#8B008B</c>)です。</summary>
    public static readonly Color DarkMagenta = Color.FromArgb(0xFF8B008B);

    /// <summary>CSS の <c>darkolivegreen</c>(<c>#556B2F</c>)です。</summary>
    public static readonly Color DarkOliveGreen = Color.FromArgb(0xFF556B2F);

    /// <summary>CSS の <c>darkorange</c>(<c>#FF8C00</c>)です。</summary>
    public static readonly Color DarkOrange = Color.FromArgb(0xFFFF8C00);

    /// <summary>CSS の <c>darkorchid</c>(<c>#9932CC</c>)です。</summary>
    public static readonly Color DarkOrchid = Color.FromArgb(0xFF9932CC);

    /// <summary>CSS の <c>darkred</c>(<c>#8B0000</c>)です。</summary>
    public static readonly Color DarkRed = Color.FromArgb(0xFF8B0000);

    /// <summary>CSS の <c>darksalmon</c>(<c>#E9967A</c>)です。</summary>
    public static readonly Color DarkSalmon = Color.FromArgb(0xFFE9967A);

    /// <summary>CSS の <c>darkseagreen</c>(<c>#8FBC8F</c>)です。</summary>
    public static readonly Color DarkSeaGreen = Color.FromArgb(0xFF8FBC8F);

    /// <summary>CSS の <c>darkslateblue</c>(<c>#483D8B</c>)です。</summary>
    public static readonly Color DarkSlateBlue = Color.FromArgb(0xFF483D8B);

    /// <summary>CSS の <c>darkslategray</c>(<c>#2F4F4F</c>)です。</summary>
    public static readonly Color DarkSlateGray = Color.FromArgb(0xFF2F4F4F);

    /// <summary>CSS の <c>darkturquoise</c>(<c>#00CED1</c>)です。</summary>
    public static readonly Color DarkTurquoise = Color.FromArgb(0xFF00CED1);

    /// <summary>CSS の <c>darkviolet</c>(<c>#9400D3</c>)です。</summary>
    public static readonly Color DarkViolet = Color.FromArgb(0xFF9400D3);

    /// <summary>CSS の <c>deeppink</c>(<c>#FF1493</c>)です。</summary>
    public static readonly Color DeepPink = Color.FromArgb(0xFFFF1493);

    /// <summary>CSS の <c>deepskyblue</c>(<c>#00BFFF</c>)です。</summary>
    public static readonly Color DeepSkyBlue = Color.FromArgb(0xFF00BFFF);

    /// <summary>CSS の <c>dimgray</c>(<c>#696969</c>)です。</summary>
    public static readonly Color DimGray = Color.FromArgb(0xFF696969);

    /// <summary>CSS の <c>dodgerblue</c>(<c>#1E90FF</c>)です。</summary>
    public static readonly Color DodgerBlue = Color.FromArgb(0xFF1E90FF);

    /// <summary>CSS の <c>firebrick</c>(<c>#B22222</c>)です。</summary>
    public static readonly Color Firebrick = Color.FromArgb(0xFFB22222);

    /// <summary>CSS の <c>floralwhite</c>(<c>#FFFAF0</c>)です。</summary>
    public static readonly Color FloralWhite = Color.FromArgb(0xFFFFFAF0);

    /// <summary>CSS の <c>forestgreen</c>(<c>#228B22</c>)です。</summary>
    public static readonly Color ForestGreen = Color.FromArgb(0xFF228B22);

    /// <summary>CSS の <c>fuchsia</c>(<c>#FF00FF</c>)です。</summary>
    public static readonly Color Fuchsia = Color.FromArgb(0xFFFF00FF);

    /// <summary>CSS の <c>gainsboro</c>(<c>#DCDCDC</c>)です。</summary>
    public static readonly Color Gainsboro = Color.FromArgb(0xFFDCDCDC);

    /// <summary>CSS の <c>ghostwhite</c>(<c>#F8F8FF</c>)です。</summary>
    public static readonly Color GhostWhite = Color.FromArgb(0xFFF8F8FF);

    /// <summary>CSS の <c>gold</c>(<c>#FFD700</c>)です。</summary>
    public static readonly Color Gold = Color.FromArgb(0xFFFFD700);

    /// <summary>CSS の <c>goldenrod</c>(<c>#DAA520</c>)です。</summary>
    public static readonly Color Goldenrod = Color.FromArgb(0xFFDAA520);

    /// <summary>CSS の <c>gray</c>(<c>#808080</c>)です。</summary>
    public static readonly Color Gray = Color.FromArgb(0xFF808080);

    /// <summary>CSS の <c>green</c>(<c>#008000</c>)です。</summary>
    public static readonly Color Green = Color.FromArgb(0xFF008000);

    /// <summary>CSS の <c>greenyellow</c>(<c>#ADFF2F</c>)です。</summary>
    public static readonly Color GreenYellow = Color.FromArgb(0xFFADFF2F);

    /// <summary>CSS の <c>honeydew</c>(<c>#F0FFF0</c>)です。</summary>
    public static readonly Color Honeydew = Color.FromArgb(0xFFF0FFF0);

    /// <summary>CSS の <c>hotpink</c>(<c>#FF69B4</c>)です。</summary>
    public static readonly Color HotPink = Color.FromArgb(0xFFFF69B4);

    /// <summary>CSS の <c>indianred</c>(<c>#CD5C5C</c>)です。</summary>
    public static readonly Color IndianRed = Color.FromArgb(0xFFCD5C5C);

    /// <summary>CSS の <c>indigo</c>(<c>#4B0082</c>)です。</summary>
    public static readonly Color Indigo = Color.FromArgb(0xFF4B0082);

    /// <summary>CSS の <c>ivory</c>(<c>#FFFFF0</c>)です。</summary>
    public static readonly Color Ivory = Color.FromArgb(0xFFFFFFF0);

    /// <summary>CSS の <c>khaki</c>(<c>#F0E68C</c>)です。</summary>
    public static readonly Color Khaki = Color.FromArgb(0xFFF0E68C);

    /// <summary>CSS の <c>lavender</c>(<c>#E6E6FA</c>)です。</summary>
    public static readonly Color Lavender = Color.FromArgb(0xFFE6E6FA);

    /// <summary>CSS の <c>lavenderblush</c>(<c>#FFF0F5</c>)です。</summary>
    public static readonly Color LavenderBlush = Color.FromArgb(0xFFFFF0F5);

    /// <summary>CSS の <c>lawngreen</c>(<c>#7CFC00</c>)です。</summary>
    public static readonly Color LawnGreen = Color.FromArgb(0xFF7CFC00);

    /// <summary>CSS の <c>lemonchiffon</c>(<c>#FFFACD</c>)です。</summary>
    public static readonly Color LemonChiffon = Color.FromArgb(0xFFFFFACD);

    /// <summary>CSS の <c>lightblue</c>(<c>#ADD8E6</c>)です。</summary>
    public static readonly Color LightBlue = Color.FromArgb(0xFFADD8E6);

    /// <summary>CSS の <c>lightcoral</c>(<c>#F08080</c>)です。</summary>
    public static readonly Color LightCoral = Color.FromArgb(0xFFF08080);

    /// <summary>CSS の <c>lightcyan</c>(<c>#E0FFFF</c>)です。</summary>
    public static readonly Color LightCyan = Color.FromArgb(0xFFE0FFFF);

    /// <summary>CSS の <c>lightgoldenrodyellow</c>(<c>#FAFAD2</c>)です。</summary>
    public static readonly Color LightGoldenrodYellow = Color.FromArgb(0xFFFAFAD2);

    /// <summary>CSS の <c>lightgray</c>(<c>#D3D3D3</c>)です。</summary>
    public static readonly Color LightGray = Color.FromArgb(0xFFD3D3D3);

    /// <summary>CSS の <c>lightgreen</c>(<c>#90EE90</c>)です。</summary>
    public static readonly Color LightGreen = Color.FromArgb(0xFF90EE90);

    /// <summary>CSS の <c>lightpink</c>(<c>#FFB6C1</c>)です。</summary>
    public static readonly Color LightPink = Color.FromArgb(0xFFFFB6C1);

    /// <summary>CSS の <c>lightsalmon</c>(<c>#FFA07A</c>)です。</summary>
    public static readonly Color LightSalmon = Color.FromArgb(0xFFFFA07A);

    /// <summary>CSS の <c>lightseagreen</c>(<c>#20B2AA</c>)です。</summary>
    public static readonly Color LightSeaGreen = Color.FromArgb(0xFF20B2AA);

    /// <summary>CSS の <c>lightskyblue</c>(<c>#87CEFA</c>)です。</summary>
    public static readonly Color LightSkyBlue = Color.FromArgb(0xFF87CEFA);

    /// <summary>CSS の <c>lightslategray</c>(<c>#778899</c>)です。</summary>
    public static readonly Color LightSlateGray = Color.FromArgb(0xFF778899);

    /// <summary>CSS の <c>lightsteelblue</c>(<c>#B0C4DE</c>)です。</summary>
    public static readonly Color LightSteelBlue = Color.FromArgb(0xFFB0C4DE);

    /// <summary>CSS の <c>lightyellow</c>(<c>#FFFFE0</c>)です。</summary>
    public static readonly Color LightYellow = Color.FromArgb(0xFFFFFFE0);

    /// <summary>CSS の <c>lime</c>(<c>#00FF00</c>)です。</summary>
    public static readonly Color Lime = Color.FromArgb(0xFF00FF00);

    /// <summary>CSS の <c>limegreen</c>(<c>#32CD32</c>)です。</summary>
    public static readonly Color LimeGreen = Color.FromArgb(0xFF32CD32);

    /// <summary>CSS の <c>linen</c>(<c>#FAF0E6</c>)です。</summary>
    public static readonly Color Linen = Color.FromArgb(0xFFFAF0E6);

    /// <summary>CSS の <c>magenta</c>(<c>#FF00FF</c>)です。</summary>
    public static readonly Color Magenta = Color.FromArgb(0xFFFF00FF);

    /// <summary>CSS の <c>maroon</c>(<c>#800000</c>)です。</summary>
    public static readonly Color Maroon = Color.FromArgb(0xFF800000);

    /// <summary>CSS の <c>mediumaquamarine</c>(<c>#66CDAA</c>)です。</summary>
    public static readonly Color MediumAquamarine = Color.FromArgb(0xFF66CDAA);

    /// <summary>CSS の <c>mediumblue</c>(<c>#0000CD</c>)です。</summary>
    public static readonly Color MediumBlue = Color.FromArgb(0xFF0000CD);

    /// <summary>CSS の <c>mediumorchid</c>(<c>#BA55D3</c>)です。</summary>
    public static readonly Color MediumOrchid = Color.FromArgb(0xFFBA55D3);

    /// <summary>CSS の <c>mediumpurple</c>(<c>#9370DB</c>)です。</summary>
    public static readonly Color MediumPurple = Color.FromArgb(0xFF9370DB);

    /// <summary>CSS の <c>mediumseagreen</c>(<c>#3CB371</c>)です。</summary>
    public static readonly Color MediumSeaGreen = Color.FromArgb(0xFF3CB371);

    /// <summary>CSS の <c>mediumslateblue</c>(<c>#7B68EE</c>)です。</summary>
    public static readonly Color MediumSlateBlue = Color.FromArgb(0xFF7B68EE);

    /// <summary>CSS の <c>mediumspringgreen</c>(<c>#00FA9A</c>)です。</summary>
    public static readonly Color MediumSpringGreen = Color.FromArgb(0xFF00FA9A);

    /// <summary>CSS の <c>mediumturquoise</c>(<c>#48D1CC</c>)です。</summary>
    public static readonly Color MediumTurquoise = Color.FromArgb(0xFF48D1CC);

    /// <summary>CSS の <c>mediumvioletred</c>(<c>#C71585</c>)です。</summary>
    public static readonly Color MediumVioletRed = Color.FromArgb(0xFFC71585);

    /// <summary>CSS の <c>midnightblue</c>(<c>#191970</c>)です。</summary>
    public static readonly Color MidnightBlue = Color.FromArgb(0xFF191970);

    /// <summary>CSS の <c>mintcream</c>(<c>#F5FFFA</c>)です。</summary>
    public static readonly Color MintCream = Color.FromArgb(0xFFF5FFFA);

    /// <summary>CSS の <c>mistyrose</c>(<c>#FFE4E1</c>)です。</summary>
    public static readonly Color MistyRose = Color.FromArgb(0xFFFFE4E1);

    /// <summary>CSS の <c>moccasin</c>(<c>#FFE4B5</c>)です。</summary>
    public static readonly Color Moccasin = Color.FromArgb(0xFFFFE4B5);

    /// <summary>CSS の <c>navajowhite</c>(<c>#FFDEAD</c>)です。</summary>
    public static readonly Color NavajoWhite = Color.FromArgb(0xFFFFDEAD);

    /// <summary>CSS の <c>navy</c>(<c>#000080</c>)です。</summary>
    public static readonly Color Navy = Color.FromArgb(0xFF000080);

    /// <summary>CSS の <c>oldlace</c>(<c>#FDF5E6</c>)です。</summary>
    public static readonly Color OldLace = Color.FromArgb(0xFFFDF5E6);

    /// <summary>CSS の <c>olive</c>(<c>#808000</c>)です。</summary>
    public static readonly Color Olive = Color.FromArgb(0xFF808000);

    /// <summary>CSS の <c>olivedrab</c>(<c>#6B8E23</c>)です。</summary>
    public static readonly Color OliveDrab = Color.FromArgb(0xFF6B8E23);

    /// <summary>CSS の <c>orange</c>(<c>#FFA500</c>)です。</summary>
    public static readonly Color Orange = Color.FromArgb(0xFFFFA500);

    /// <summary>CSS の <c>orangered</c>(<c>#FF4500</c>)です。</summary>
    public static readonly Color OrangeRed = Color.FromArgb(0xFFFF4500);

    /// <summary>CSS の <c>orchid</c>(<c>#DA70D6</c>)です。</summary>
    public static readonly Color Orchid = Color.FromArgb(0xFFDA70D6);

    /// <summary>CSS の <c>palegoldenrod</c>(<c>#EEE8AA</c>)です。</summary>
    public static readonly Color PaleGoldenrod = Color.FromArgb(0xFFEEE8AA);

    /// <summary>CSS の <c>palegreen</c>(<c>#98FB98</c>)です。</summary>
    public static readonly Color PaleGreen = Color.FromArgb(0xFF98FB98);

    /// <summary>CSS の <c>paleturquoise</c>(<c>#AFEEEE</c>)です。</summary>
    public static readonly Color PaleTurquoise = Color.FromArgb(0xFFAFEEEE);

    /// <summary>CSS の <c>palevioletred</c>(<c>#DB7093</c>)です。</summary>
    public static readonly Color PaleVioletRed = Color.FromArgb(0xFFDB7093);

    /// <summary>CSS の <c>papayawhip</c>(<c>#FFEFD5</c>)です。</summary>
    public static readonly Color PapayaWhip = Color.FromArgb(0xFFFFEFD5);

    /// <summary>CSS の <c>peachpuff</c>(<c>#FFDAB9</c>)です。</summary>
    public static readonly Color PeachPuff = Color.FromArgb(0xFFFFDAB9);

    /// <summary>CSS の <c>peru</c>(<c>#CD853F</c>)です。</summary>
    public static readonly Color Peru = Color.FromArgb(0xFFCD853F);

    /// <summary>CSS の <c>pink</c>(<c>#FFC0CB</c>)です。</summary>
    public static readonly Color Pink = Color.FromArgb(0xFFFFC0CB);

    /// <summary>CSS の <c>plum</c>(<c>#DDA0DD</c>)です。</summary>
    public static readonly Color Plum = Color.FromArgb(0xFFDDA0DD);

    /// <summary>CSS の <c>powderblue</c>(<c>#B0E0E6</c>)です。</summary>
    public static readonly Color PowderBlue = Color.FromArgb(0xFFB0E0E6);

    /// <summary>CSS の <c>purple</c>(<c>#800080</c>)です。</summary>
    public static readonly Color Purple = Color.FromArgb(0xFF800080);

    /// <summary>CSS の <c>red</c>(<c>#FF0000</c>)です。</summary>
    public static readonly Color Red = Color.FromArgb(0xFFFF0000);

    /// <summary>CSS の <c>rosybrown</c>(<c>#BC8F8F</c>)です。</summary>
    public static readonly Color RosyBrown = Color.FromArgb(0xFFBC8F8F);

    /// <summary>CSS の <c>royalblue</c>(<c>#4169E1</c>)です。</summary>
    public static readonly Color RoyalBlue = Color.FromArgb(0xFF4169E1);

    /// <summary>CSS の <c>saddlebrown</c>(<c>#8B4513</c>)です。</summary>
    public static readonly Color SaddleBrown = Color.FromArgb(0xFF8B4513);

    /// <summary>CSS の <c>salmon</c>(<c>#FA8072</c>)です。</summary>
    public static readonly Color Salmon = Color.FromArgb(0xFFFA8072);

    /// <summary>CSS の <c>sandybrown</c>(<c>#F4A460</c>)です。</summary>
    public static readonly Color SandyBrown = Color.FromArgb(0xFFF4A460);

    /// <summary>CSS の <c>seagreen</c>(<c>#2E8B57</c>)です。</summary>
    public static readonly Color SeaGreen = Color.FromArgb(0xFF2E8B57);

    /// <summary>CSS の <c>seashell</c>(<c>#FFF5EE</c>)です。</summary>
    public static readonly Color SeaShell = Color.FromArgb(0xFFFFF5EE);

    /// <summary>CSS の <c>sienna</c>(<c>#A0522D</c>)です。</summary>
    public static readonly Color Sienna = Color.FromArgb(0xFFA0522D);

    /// <summary>CSS の <c>silver</c>(<c>#C0C0C0</c>)です。</summary>
    public static readonly Color Silver = Color.FromArgb(0xFFC0C0C0);

    /// <summary>CSS の <c>skyblue</c>(<c>#87CEEB</c>)です。</summary>
    public static readonly Color SkyBlue = Color.FromArgb(0xFF87CEEB);

    /// <summary>CSS の <c>slateblue</c>(<c>#6A5ACD</c>)です。</summary>
    public static readonly Color SlateBlue = Color.FromArgb(0xFF6A5ACD);

    /// <summary>CSS の <c>slategray</c>(<c>#708090</c>)です。</summary>
    public static readonly Color SlateGray = Color.FromArgb(0xFF708090);

    /// <summary>CSS の <c>snow</c>(<c>#FFFAFA</c>)です。</summary>
    public static readonly Color Snow = Color.FromArgb(0xFFFFFAFA);

    /// <summary>CSS の <c>springgreen</c>(<c>#00FF7F</c>)です。</summary>
    public static readonly Color SpringGreen = Color.FromArgb(0xFF00FF7F);

    /// <summary>CSS の <c>steelblue</c>(<c>#4682B4</c>)です。</summary>
    public static readonly Color SteelBlue = Color.FromArgb(0xFF4682B4);

    /// <summary>CSS の <c>tan</c>(<c>#D2B48C</c>)です。</summary>
    public static readonly Color Tan = Color.FromArgb(0xFFD2B48C);

    /// <summary>CSS の <c>teal</c>(<c>#008080</c>)です。</summary>
    public static readonly Color Teal = Color.FromArgb(0xFF008080);

    /// <summary>CSS の <c>thistle</c>(<c>#D8BFD8</c>)です。</summary>
    public static readonly Color Thistle = Color.FromArgb(0xFFD8BFD8);

    /// <summary>CSS の <c>tomato</c>(<c>#FF6347</c>)です。</summary>
    public static readonly Color Tomato = Color.FromArgb(0xFFFF6347);

    /// <summary>CSS の <c>turquoise</c>(<c>#40E0D0</c>)です。</summary>
    public static readonly Color Turquoise = Color.FromArgb(0xFF40E0D0);

    /// <summary>CSS の <c>violet</c>(<c>#EE82EE</c>)です。</summary>
    public static readonly Color Violet = Color.FromArgb(0xFFEE82EE);

    /// <summary>CSS の <c>wheat</c>(<c>#F5DEB3</c>)です。</summary>
    public static readonly Color Wheat = Color.FromArgb(0xFFF5DEB3);

    /// <summary>CSS の <c>white</c>(<c>#FFFFFF</c>)です。</summary>
    public static readonly Color White = Color.FromArgb(0xFFFFFFFF);

    /// <summary>CSS の <c>whitesmoke</c>(<c>#F5F5F5</c>)です。</summary>
    public static readonly Color WhiteSmoke = Color.FromArgb(0xFFF5F5F5);

    /// <summary>CSS の <c>yellow</c>(<c>#FFFF00</c>)です。</summary>
    public static readonly Color Yellow = Color.FromArgb(0xFFFFFF00);

    /// <summary>CSS の <c>yellowgreen</c>(<c>#9ACD32</c>)です。</summary>
    public static readonly Color YellowGreen = Color.FromArgb(0xFF9ACD32);

    /// <summary>透明な黒です。CSS の <c>transparent</c> と同じく、全成分が0です。</summary>
    public static readonly Color Transparent = Color.FromArgb(0x00000000);
}
