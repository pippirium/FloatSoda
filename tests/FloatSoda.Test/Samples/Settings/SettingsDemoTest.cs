using FloatSoda.Abstractions.Geometries;
using FloatSoda.Abstractions.Input;
using FloatSoda.Core;
using FloatSoda.RenderObjects;
using FloatSoda.RenderObjects.Gesture;
using FloatSoda.Samples.Settings;

namespace FloatSoda.Test.Samples.Settings;

public class SettingsDemoTest
{
    [Fact]
    public void 起動ボタンを押す_起動を一度要求しDropdown展開後もキャンバス内に収まる()
    {
        var starts = 0;
        var view = new RenderView();
        var binding = new WidgetBinding(view);
        binding.AttachRootWidget(
            new SettingsDemo
            {
                LaunchScreenView = () =>
                {
                    starts++;
                    return "ScreenViewを起動しました";
                },
            }
        );
        binding.FlushBuildAndLayout();

        var button = Assert.Single(
            FindListeners(view, 300),
            item => item.Listener.OnPointerMove is null
        );
        Tap(binding, button.Position + new Offset(150, 28));
        binding.FlushBuildAndLayout();
        Assert.Equal(1, starts);

        var dropdown = Assert.Single(FindListeners(view, 228));
        Tap(binding, dropdown.Position + new Offset(114, 28));
        binding.FlushBuildAndLayout();
        Assert.Equal(4, FindListeners(view, 228).Count);
        Assert.InRange(view.Size.Height, 1, 1000);
        Assert.Equal(960, view.Size.Width);
    }

    private static void Tap(WidgetBinding binding, Offset position)
    {
        binding.HandlePointerEvent(new PointerEvent(1, PointerEventPhase.Add, position));
        binding.HandlePointerEvent(new PointerEvent(1, PointerEventPhase.Down, position));
        binding.HandlePointerEvent(new PointerEvent(1, PointerEventPhase.Up, position));
        binding.HandlePointerEvent(new PointerEvent(1, PointerEventPhase.Remove, position));
    }

    private static List<(RenderPointerListener Listener, Offset Position)> FindListeners(
        RenderObject root,
        double width
    )
    {
        var found = new List<(RenderPointerListener, Offset)>();
        void Visit(RenderObject node, Offset position)
        {
            if (
                node is RenderPointerListener listener
                && listener.Size.Width == width
                && listener.Size.Height == 56
            )
                found.Add((listener, position));
            node.VisitChildren(child =>
                Visit(
                    child,
                    position + (child.ParentData is BoxParentData data ? data.Offset : Offset.Zero)
                )
            );
        }
        Visit(root, Offset.Zero);
        return found;
    }
}
