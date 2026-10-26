using System.Numerics;
using FloatSoda.Abstractions.Input;
using FloatSoda.Elements;
using FloatSoda.Geometrics;
using FloatSoda.Gesture;
using FloatSoda.Painting;
using FloatSoda.Widgets.Gesture;
using FloatSoda.Widgets.Layout;

namespace FloatSoda.Widgets.Settings;

/// <summary>コントローラーのレーザーを押しながら移動して数値を選ぶ制御済みスライダーです。</summary>
/// <remarks>ポインター座標はHitTestEntryの変換行列でローカル座標に変換します。</remarks>
public sealed record SettingsSlider : StatefulWidget<SettingsSlider>
{
    /// <summary>現在値。描画時にMin/Maxへクランプします。</summary>
    public double Value { get; init; }

    /// <summary>選べる最小値。</summary>
    public double Min { get; init; }

    /// <summary>選べる最大値。Minより大きい有限値を指定します。</summary>
    public double Max { get; init; } = 1;

    /// <summary>新しい値を通知します。nullの場合は読み取り専用です。</summary>
    public Action<double>? OnChanged { get; init; }

    /// <inheritdoc/>
    public override State<SettingsSlider> CreateState() => new SettingsSliderState();
}

/// <summary>スライダー上の押下ポインターを追跡します。</summary>
public sealed class SettingsSliderState : State<SettingsSlider>
{
    private const double TrackWidth = 300;
    private const double TrackHeight = 56;
    private int? _activePointer;

    /// <summary>値の範囲を検証し、範囲内に収めます。</summary>
    public static double ClampValue(double value, double min, double max)
    {
        if (!double.IsFinite(min) || !double.IsFinite(max) || max <= min || !double.IsFinite(max - min))
        {
            throw new ArgumentOutOfRangeException(nameof(max), "MinとMaxは有限値で、MaxはMinより大きくしてください。");
        }
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Valueには有限値を指定してください。");
        }

        return Math.Clamp(value, min, max);
    }

    /// <inheritdoc/>
    public override Widget Build(IBuildContext context)
    {
        var model = Widget!;
        var value = ClampValue(model.Value, model.Min, model.Max);
        var ratio = (value - model.Min) / (model.Max - model.Min);
        var thumbLeft = Math.Clamp(ratio * TrackWidth - 13, 0, TrackWidth - 26);
        return new Column
        {
            MainAxisSize = MainAxisSize.Min,
            CrossAxisAlignment = CrossAxisAlignment.Center,
            Children =
            {
                new Text(value.ToString("0.##")) { Style = new TextStyle { FontSize = 23, Color = SettingsPalette.Text } },
                new SizedBox { Height = 8 },
                new Listener
                {
                    Behaviour = HitTestBehaviour.Opaque,
                    OnPointerDown = HandleDown,
                    OnPointerMove = HandleMove,
                    OnPointerUp = HandleEnd,
                    OnPointerCancel = HandleEnd,
                    Child = new SizedBox
                    {
                        Width = TrackWidth,
                        Height = TrackHeight,
                        Child = new Stack
                        {
                            Children =
                            {
                                new Align
                                {
                                    Alignment = Alignment.Center,
                                    Child = new Container
                                    {
                                        Width = TrackWidth,
                                        Height = 12,
                                        Decoration = new BoxDecoration
                                        {
                                            Color = SettingsPalette.Control,
                                            BorderRadius = BorderRadius.Circular(6)
                                        }
                                    }
                                },
                                new Align
                                {
                                    Alignment = Alignment.CenterLeft,
                                    Child = new Container
                                    {
                                        Width = ratio * TrackWidth,
                                        Height = 12,
                                        Decoration = new BoxDecoration
                                        {
                                            Color = SettingsPalette.Accent,
                                            BorderRadius = BorderRadius.Circular(6)
                                        }
                                    }
                                },
                                new Positioned
                                {
                                    Left = thumbLeft,
                                    Top = (TrackHeight - 26) / 2,
                                    Child = new Container
                                    {
                                        Width = 26,
                                        Height = 26,
                                        Decoration = new BoxDecoration
                                        {
                                            Color = SettingsPalette.Text,
                                            BorderRadius = BorderRadius.Circular(13)
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private void HandleDown(PointerEvent e)
    {
        if (Widget?.OnChanged is null || _activePointer.HasValue) return;
        _activePointer = e.PointerId;
        Emit(e);
    }

    private void HandleMove(PointerEvent e)
    {
        if (_activePointer == e.PointerId) Emit(e);
    }

    private void HandleEnd(PointerEvent e)
    {
        if (_activePointer == e.PointerId) _activePointer = null;
    }

    private void Emit(PointerEvent e)
    {
        var model = Widget!;
        if (model.OnChanged is null) return;
        var position = new Vector2((float)e.Position.X, (float)e.Position.Y);
        if (e.Transform is { } localTransform) position = Vector2.Transform(position, localTransform);
        var ratio = Math.Clamp((double)position.X / TrackWidth, 0, 1);
        var next = model.Min + (model.Max - model.Min) * ratio;
        model.OnChanged(ClampValue(next, model.Min, model.Max));
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        _activePointer = null;
        base.Dispose();
    }
}
