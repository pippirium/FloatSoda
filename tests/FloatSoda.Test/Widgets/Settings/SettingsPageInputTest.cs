using FloatSoda.Abstractions.Geometries;
using FloatSoda.Abstractions.Input;
using FloatSoda.Core;
using FloatSoda.RenderObjects;
using FloatSoda.RenderObjects.Gesture;
using FloatSoda.Widgets;
using FloatSoda.Widgets.Layout;
using FloatSoda.Widgets.Settings;

namespace FloatSoda.Test.Widgets.Settings;

public class SettingsPageInputTest
{
    [Fact]
    public void HandlePointerEvent_ページ内のSlider中央を押す_中間値を通知する()
    {
        double? changed = null;
        var view = new RenderView();
        var binding = new WidgetBinding(view);
        binding.AttachRootWidget(
            new SizedBox
            {
                Width = 960,
                Child = new SettingsPage
                {
                    Title = "Overlay Settings",
                    Sections =
                    [
                        new SettingsSection
                        {
                            Title = "General",
                            Children =
                            [
                                new SettingsTile
                                {
                                    Title = "Notifications",
                                    Trailing = new SettingsSwitch { Value = true },
                                },
                                new SettingsTile
                                {
                                    Title = "Controller haptics",
                                    Trailing = new SettingsSwitch { Value = true },
                                },
                            ],
                        },
                        new SettingsSection
                        {
                            Title = "Display",
                            Children =
                            [
                                new SettingsTile
                                {
                                    Title = "UI scale",
                                    Description = "Adjust the overlay's logical scale",
                                    Trailing = new SettingsSlider
                                    {
                                        Value = 0,
                                        OnChanged = value => changed = value,
                                    },
                                },
                                new SettingsTile
                                {
                                    Title = "Quality preset",
                                    Trailing = new SettingsDropdown
                                    {
                                        Value = "Standard",
                                        Options = ["Standard", "Comfort", "Performance"],
                                        OnChanged = _ => { },
                                    },
                                },
                            ],
                        },
                    ],
                },
            }
        );
        binding.FlushBuildAndLayout();
        Offset? target = null;
        void Visit(RenderObject node, Offset offset)
        {
            if (
                node is RenderPointerListener listener
                && listener.Size.Width == 300
                && listener.Size.Height == 56
            )
                target = offset + new Offset(150, 28);
            node.VisitChildren(child =>
                Visit(
                    child,
                    offset + (child.ParentData is BoxParentData data ? data.Offset : Offset.Zero)
                )
            );
        }
        Visit(view, Offset.Zero);
        Assert.NotNull(target);
        view.Owner!.FlushPaint();
        using var bitmap = new FloatSoda.Rendering.LayerBitmapRenderer().Render(
            view.Layer!.Clone(),
            new SkiaSharp.SKSizeI(960, (int)Math.Ceiling(view.Size.Height))
        );
        Assert.Equal(
            new SkiaSharp.SKColor(52, 66, 82),
            bitmap.GetPixel((int)target.Value.X, (int)target.Value.Y)
        );
        binding.HandlePointerEvent(new PointerEvent(1, PointerEventPhase.Add, target.Value));
        binding.HandlePointerEvent(new PointerEvent(1, PointerEventPhase.Down, target.Value));
        Assert.NotNull(changed);
        Assert.InRange(changed.Value, 0.49, 0.51);
    }
}
