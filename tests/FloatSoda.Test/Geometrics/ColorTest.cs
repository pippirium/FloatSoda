using FloatSoda.Geometrics;
using SkiaSharp;

namespace FloatSoda.Test.Geometrics;

public class ColorTest
{
    // ── コンストラクター ───────────────────────────────────────────

    [Fact]
    public void Constructor_RGBを省いた呼び出し_コンパイルできない()
    {
        // new Color(Red: 255) がコンパイルエラーになることを、引数の既定値の有無で確かめる
        var parameters = typeof(Color).GetConstructor([typeof(byte), typeof(byte), typeof(byte), typeof(byte)])!
            .GetParameters();

        Assert.False(parameters[0].HasDefaultValue);
        Assert.False(parameters[1].HasDefaultValue);
        Assert.False(parameters[2].HasDefaultValue);
        Assert.True(parameters[3].HasDefaultValue);
        Assert.Equal((byte)255, parameters[3].DefaultValue);
    }

    [Fact]
    public void Constructor_RGBだけを渡す_不透明な色になる()
    {
        var color = new Color(255, 0, 0);

        Assert.Equal(new Color(255, 0, 0, 255), color);
    }

    [Fact]
    public void Constructor_引数なし_透明な黒になる()
    {
        Assert.Equal(new Color(0, 0, 0, 0), new Color());
        Assert.Equal(new Color(0, 0, 0, 0), default(Color));
    }

    // ── FromArgb(uint) ─────────────────────────────────────────────

    [Fact]
    public void FromArgb_32ビット値_AARRGGBBの順に読む()
    {
        Assert.Equal(new Color(0x21, 0x96, 0xF3), Color.FromArgb(0xFF2196F3));
    }

    [Fact]
    public void FromArgb_不透明度が上位8ビット_Alphaに入る()
    {
        Assert.Equal(new Color(0x12, 0x34, 0x56, 0x80), Color.FromArgb(0x80123456));
    }

    [Fact]
    public void FromArgb_0_透明な黒を返す()
    {
        Assert.Equal(new Color(0, 0, 0, 0), Color.FromArgb(0u));
    }

    // ── FromArgb(a, r, g, b) ───────────────────────────────────────

    [Fact]
    public void FromArgb_成分を4つ渡す_コンストラクターと同じ色を返す()
    {
        Assert.Equal(new Color(10, 20, 30, 40), Color.FromArgb(40, 10, 20, 30));
    }

    // ── FromRgbo ───────────────────────────────────────────────────

    [Fact]
    public void FromRgbo_opacityが0_5_Alphaが128になる()
    {
        Assert.Equal((byte)128, Color.FromRgbo(255, 0, 0, 0.5).Alpha);
    }

    [Theory]
    [InlineData(0.0, 0)]
    [InlineData(1.0, 255)]
    public void FromRgbo_opacityが境界値_Alphaが端の値になる(double opacity, int expectedAlpha)
    {
        Assert.Equal(new Color(1, 2, 3, (byte)expectedAlpha), Color.FromRgbo(1, 2, 3, opacity));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(double.NaN)]
    public void FromRgbo_opacityが範囲の外_ArgumentOutOfRangeExceptionを投げる(double opacity)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Color.FromRgbo(255, 0, 0, opacity));

        Assert.Equal("opacity", exception.ParamName);
    }

    // ── Parse ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("#2196F3")]
    [InlineData("2196F3")]
    [InlineData("#2196f3")]
    public void Parse_6桁_RRGGBBの順に読み不透明な色を返す(string webColor)
    {
        Assert.Equal(new Color(0x21, 0x96, 0xF3), Color.Parse(webColor));
    }

    [Theory]
    [InlineData("#F80")]
    [InlineData("F80")]
    public void Parse_3桁_1文字を2回繰り返して読む(string webColor)
    {
        Assert.Equal(new Color(0xFF, 0x88, 0x00), Color.Parse(webColor));
    }

    [Theory]
    [InlineData("#FF000080")]
    [InlineData("FF000080")]
    public void Parse_8桁_RRGGBBAAの順に読む(string webColor)
    {
        Assert.Equal(new Color(255, 0, 0, 128), Color.Parse(webColor));
    }

    [Fact]
    public void Parse_8桁_透明度を末尾から読む()
    {
        // FromArgb(uint) の 0xAARRGGBB とは逆に、透明度は末尾の2桁である
        Assert.Equal(new Color(0x12, 0x34, 0x56, 0x78), Color.Parse("#12345678"));
    }

    [Theory]
    [InlineData("#F008")]
    [InlineData("F008")]
    public void Parse_4桁_RGBAの順に1文字を2回繰り返して読む(string webColor)
    {
        Assert.Equal(new Color(255, 0, 0, 136), Color.Parse(webColor));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_空_ArgumentExceptionを投げる(string? webColor)
    {
        var exception = Assert.Throws<ArgumentException>(() => Color.Parse(webColor!));

        Assert.Equal("webColor", exception.ParamName);
    }

    [Theory]
    [InlineData("#")]
    [InlineData("#F")]
    [InlineData("#FF")]
    [InlineData("#FF000")]
    [InlineData("#FF00000")]
    [InlineData("#FF0000000")]
    public void Parse_桁数が3_4_6_8以外_FormatExceptionを投げる(string webColor)
    {
        Assert.Throws<FormatException>(() => Color.Parse(webColor));
    }

    [Theory]
    [InlineData("#GG0000")]
    [InlineData("#FF00GG80")]
    [InlineData("#G008")]
    public void Parse_16進数以外の文字_FormatExceptionを投げる(string webColor)
    {
        Assert.Throws<FormatException>(() => Color.Parse(webColor));
    }

    [Theory]
    [InlineData("# F0000")]
    [InlineData("#F0000 ")]
    [InlineData("#FF 0000")]
    [InlineData("#F 0")]
    [InlineData("#F00 ")]
    public void Parse_成分の中に空白_FormatExceptionを投げる(string webColor)
    {
        Assert.Throws<FormatException>(() => Color.Parse(webColor));
    }

    // ── SKColor との変換 ───────────────────────────────────────────

    [Fact]
    public void ImplicitOperator_SKColorと往復_成分が変わらない()
    {
        var color = new Color(1, 2, 3, 4);

        SKColor skColor = color;
        Color roundTripped = skColor;

        Assert.Equal(color, roundTripped);
    }
}
