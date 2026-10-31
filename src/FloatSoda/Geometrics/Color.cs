using System.Globalization;
using SkiaSharp;

namespace FloatSoda.Geometrics;

/// <summary>赤、緑、青、不透明度の各成分を8ビットで表す不変の色値です。</summary>
/// <remarks>
/// 引数なしの <c>new Color()</c> と <c>default(Color)</c> は、全成分が0の透明な黒になります。
/// 不透明度だけを変えた色は <c>color with { Alpha = 128 }</c> で作れます。
/// </remarks>
/// <param name="Red">赤成分。0から255まで。</param>
/// <param name="Green">緑成分。0から255まで。</param>
/// <param name="Blue">青成分。0から255まで。</param>
/// <param name="Alpha">不透明度。0は完全な透明、255は完全な不透明で、既定値は255です。</param>
public readonly record struct Color(byte Red, byte Green, byte Blue, byte Alpha = 255)
{
    /// <summary>32ビットの <c>0xAARRGGBB</c> 形式の整数から色値を作成します。</summary>
    /// <param name="argb">上位から順に不透明度、赤、緑、青を8ビットずつ並べた値。例: <c>0xFF2196F3</c>。</param>
    /// <returns>各成分を取り出した色値。</returns>
    public static Color FromArgb(uint argb) =>
        new((byte)(argb >> 16), (byte)(argb >> 8), (byte)argb, (byte)(argb >> 24));

    /// <summary>不透明度、赤、緑、青の順に渡した成分から色値を作成します。</summary>
    /// <param name="a">不透明度。0は完全な透明、255は完全な不透明。</param>
    /// <param name="r">赤成分。0から255まで。</param>
    /// <param name="g">緑成分。0から255まで。</param>
    /// <param name="b">青成分。0から255まで。</param>
    /// <returns><c>new Color(r, g, b, a)</c> と等しい色値。</returns>
    public static Color FromArgb(byte a, byte r, byte g, byte b) => new(r, g, b, a);

    /// <summary>赤、緑、青と、0.0から1.0までの不透明度から色値を作成します。</summary>
    /// <param name="r">赤成分。0から255まで。</param>
    /// <param name="g">緑成分。0から255まで。</param>
    /// <param name="b">青成分。0から255まで。</param>
    /// <param name="opacity">不透明度。0.0は完全な透明、1.0は完全な不透明。</param>
    /// <returns><see cref="Alpha"/> が <c>opacity * 255</c> を四捨五入した値になる色値。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="opacity"/> が0.0から1.0の範囲の外か、<see cref="double.NaN"/> です。</exception>
    public static Color FromRgbo(byte r, byte g, byte b, double opacity)
    {
        if (double.IsNaN(opacity) || opacity is < 0.0 or > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(opacity),
                opacity,
                "不透明度は0.0から1.0までの値で指定してください。"
            );
        }

        return new Color(r, g, b, (byte)Math.Round(opacity * 255, MidpointRounding.AwayFromZero));
    }

    /// <summary>CSS と同じ16進数のウェブカラー表記から色値を作成します。</summary>
    /// <remarks>
    /// 受け付ける形式は次の4つで、不透明度は末尾に置きます。
    /// <list type="bullet">
    /// <item><description><c>#RRGGBB</c>: 例 <c>#2196F3</c>。不透明度は255になります。</description></item>
    /// <item><description><c>#RRGGBBAA</c>: 例 <c>#FF000080</c> は <c>new Color(255, 0, 0, 128)</c> です。</description></item>
    /// <item><description><c>#RGB</c>: 各文字を2回繰り返して読みます。<c>#F80</c> は <c>#FF8800</c> と同じです。不透明度は255になります。</description></item>
    /// <item><description><c>#RGBA</c>: 各文字を2回繰り返して読みます。<c>#F008</c> は <c>#FF000088</c> と同じです。</description></item>
    /// </list>
    /// <see cref="FromArgb(uint)"/> の <c>0xAARRGGBB</c> は不透明度が先頭で、並び順が逆です。
    /// </remarks>
    /// <param name="webColor">先頭の番号記号を省略できる、3桁・4桁・6桁・8桁の16進数表記。大文字と小文字は区別しません。</param>
    /// <returns>表記から読み取った色値。3桁と6桁のときは、不透明度が255です。</returns>
    /// <exception cref="ArgumentException"><paramref name="webColor"/>が<see langword="null"/>、空、または空白だけです。</exception>
    /// <exception cref="FormatException">文字数が3桁・4桁・6桁・8桁のどれでもないか、16進数以外の文字を含みます。</exception>
    public static Color Parse(string webColor)
    {
        if (string.IsNullOrWhiteSpace(webColor))
        {
            throw new ArgumentException("カラーコードが空です。", nameof(webColor));
        }

        // 先頭の '#' を取り除く
        ReadOnlySpan<char> colorSpan = webColor.AsSpan();

        if (colorSpan[0] == '#')
        {
            colorSpan = colorSpan[1..];
        }

        switch (colorSpan.Length)
        {
            // #RRGGBBAA 形式の場合 (文字数が8)
            case 8:
            {
                byte r = ParseHex(colorSpan[0..2]);
                byte g = ParseHex(colorSpan[2..4]);
                byte b = ParseHex(colorSpan[4..6]);
                byte a = ParseHex(colorSpan[6..8]);

                return new Color(r, g, b, a);
            }
            // #RRGGBB 形式の場合 (文字数が6)
            case 6:
            {
                byte r = ParseHex(colorSpan[0..2]);
                byte g = ParseHex(colorSpan[2..4]);
                byte b = ParseHex(colorSpan[4..6]);

                return new Color(r, g, b);
            }
            // #RGBA 形式の場合 (文字数が4)
            case 4:
            {
                // 1文字を2回繰り返す (F -> FF)
                byte r = ParseHex([colorSpan[0], colorSpan[0]]);
                byte g = ParseHex([colorSpan[1], colorSpan[1]]);
                byte b = ParseHex([colorSpan[2], colorSpan[2]]);
                byte a = ParseHex([colorSpan[3], colorSpan[3]]);

                return new Color(r, g, b, a);
            }
            // #RGB 形式の場合 (文字数が3)
            case 3:
            {
                // 1文字を2回繰り返す (F -> FF)
                byte r = ParseHex([colorSpan[0], colorSpan[0]]);
                byte g = ParseHex([colorSpan[1], colorSpan[1]]);
                byte b = ParseHex([colorSpan[2], colorSpan[2]]);

                return new Color(r, g, b);
            }
            default:
                throw new FormatException($"無効なカラーコード形式です: {webColor}");
        }

        // HexNumber は前後の空白を許すため、"# F0000" が通ってしまう。16進数の文字だけを受け付ける
        static byte ParseHex(ReadOnlySpan<char> chars) =>
            byte.Parse(chars, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
    }

    /// <summary>各成分を保持したまま描画用の色へ変換します。</summary>
    /// <param name="color">変換する色値。</param>
    /// <returns>同じ赤、緑、青、不透明度を持つ描画用の色。</returns>
    public static implicit operator SKColor(Color color) =>
        new(color.Red, color.Green, color.Blue, color.Alpha);

    /// <summary>描画用の色から各成分を保持した色値へ変換します。</summary>
    /// <param name="skColor">変換する描画用の色。</param>
    /// <returns>同じ赤、緑、青、不透明度を持つ色値。</returns>
    public static implicit operator Color(SKColor skColor) =>
        new(skColor.Red, skColor.Green, skColor.Blue, skColor.Alpha);
}

/// <summary>色相、彩度、明度で色を表す不変値です。</summary>
/// <param name="Hue">色相を表す0度から360度までの角度。</param>
/// <param name="Saturation">彩度を表す0パーセントから100パーセントまでの値。</param>
/// <param name="Value">明度を表す0パーセントから100パーセントまでの値。</param>
/// <seealso cref="Color"/>
internal readonly record struct HSVColor(double Hue, double Saturation, double Value)
{
    /// <summary>0度から360度までの色相を取得します。</summary>
    /// <exception cref="ArgumentOutOfRangeException">設定値が0未満または360を上回っています。</exception>
    public double Hue
    {
        get;
        init
        {
            if (value is < 0 or > 360)
                throw new ArgumentOutOfRangeException();
            field = value;
        }
    } = Hue;

    /// <summary>0パーセントから100パーセントまでの彩度を取得します。</summary>
    /// <exception cref="ArgumentOutOfRangeException">設定値が0未満または100を上回っています。</exception>
    public double Saturation
    {
        get;
        init
        {
            if (value is < 0 or > 100)
                throw new ArgumentOutOfRangeException();
            field = value;
        }
    } = Saturation;

    /// <summary>0パーセントから100パーセントまでの明度を取得します。</summary>
    /// <exception cref="ArgumentOutOfRangeException">設定値が0未満または100を上回っています。</exception>
    public double Value
    {
        get;
        init
        {
            if (value is < 0 or > 100)
                throw new ArgumentOutOfRangeException();
            field = value;
        }
    } = Value;

    /// <summary>赤、緑、青の色値を色相、彩度、明度の表現へ変換します。</summary>
    /// <param name="color">変換する色値。</param>
    /// <returns>変換後の色相、彩度、明度。</returns>
    /// <exception cref="NotImplementedException">この変換は現在実装されていません。</exception>
    public static implicit operator HSVColor(Color color) => throw new NotImplementedException();

    /// <summary>色相、彩度、明度の表現を赤、緑、青の色値へ変換します。</summary>
    /// <param name="skColor">変換する色相、彩度、明度。</param>
    /// <returns>変換後の色値。</returns>
    /// <exception cref="NotImplementedException">この変換は現在実装されていません。</exception>
    public static implicit operator Color(HSVColor skColor) => throw new NotImplementedException();
}

/// <summary>色相、彩度、輝度で色を表す不変値です。</summary>
/// <param name="Hue">色相を表す0度から360度までの角度。</param>
/// <param name="Saturation">彩度を表す0パーセントから100パーセントまでの値。</param>
/// <param name="Value">輝度を表す0パーセントから100パーセントまでの値。</param>
/// <seealso cref="Color"/>
internal readonly record struct HSLColor(double Hue, double Saturation, double Value)
{
    /// <summary>0度から360度までの色相を取得します。</summary>
    /// <exception cref="ArgumentOutOfRangeException">設定値が0未満または360を上回っています。</exception>
    public double Hue
    {
        get;
        init
        {
            if (value is < 0 or > 360)
                throw new ArgumentOutOfRangeException();
            field = value;
        }
    } = Hue;

    /// <summary>0パーセントから100パーセントまでの彩度を取得します。</summary>
    /// <exception cref="ArgumentOutOfRangeException">設定値が0未満または100を上回っています。</exception>
    public double Saturation
    {
        get;
        init
        {
            if (value is < 0 or > 100)
                throw new ArgumentOutOfRangeException();
            field = value;
        }
    } = Saturation;

    /// <summary>0パーセントから100パーセントまでの輝度を取得します。</summary>
    /// <exception cref="ArgumentOutOfRangeException">設定値が0未満または100を上回っています。</exception>
    public double Value
    {
        get;
        init
        {
            if (value is < 0 or > 100)
                throw new ArgumentOutOfRangeException();
            field = value;
        }
    } = Value;

    /// <summary>赤、緑、青の色値を色相、彩度、輝度の表現へ変換します。</summary>
    /// <param name="color">変換する色値。</param>
    /// <returns>変換後の色相、彩度、輝度。</returns>
    /// <exception cref="NotImplementedException">この変換は現在実装されていません。</exception>
    public static implicit operator HSLColor(Color color) => throw new NotImplementedException();

    /// <summary>色相、彩度、輝度の表現を赤、緑、青の色値へ変換します。</summary>
    /// <param name="skColor">変換する色相、彩度、輝度。</param>
    /// <returns>変換後の色値。</returns>
    /// <exception cref="NotImplementedException">この変換は現在実装されていません。</exception>
    public static implicit operator Color(HSLColor skColor) => throw new NotImplementedException();
}
