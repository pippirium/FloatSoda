using FloatSoda.RenderObjects;
using FloatSoda.Widgets;

namespace FloatSoda.Elements;

/// <summary>
/// <see cref="WindowWidget"/> の Element。マウント・更新時にウィンドウ宣言の
/// <see cref="WindowWidget.Size"/> をルートの <see cref="RenderView.FixedSize"/> へ反映する。
/// </summary>
public class WindowElement : InheritedElement
{
    /// <inheritdoc />
    public override void Mount(Element? parent)
    {
        base.Mount(parent);
        ApplySizeToRenderView();
    }

    /// <inheritdoc />
    public override void Update(Widget newWidget)
    {
        base.Update(newWidget);

        // ルートの寸法を新しい宣言の値へ差し替える。
        // base.Update(<see cref="InheritedElement.Update"/>)は宣言を受け取ったうえで依存 Element へ
        // <see cref="Element.DidChangeDependencies"/> を呼び、そこで再構築が予約される。子孫が実測に
        // 合わせて宣言を作り直す使い方（ScreenView のオーバーレイサイズ追従）は、予約された再構築が
        // 新しい <see cref="RenderView.FixedSize"/> と組むことに依存するため、ここでの反映を
        // <see cref="InheritedElement.Update"/> より後に置くこと。寸法が変わらなかったときは同じ値を
        // 再代入するだけで、Layout Dirty は立たない。
        ApplySizeToRenderView();
    }

    private void ApplySizeToRenderView()
    {
        for (var ancestor = Parent; ancestor is not null; ancestor = ancestor.Parent)
        {
            if (ancestor is not RenderObjectElement { RenderObject: RenderView view })
                continue;

            view.FixedSize = (Widget as WindowWidget)?.Size;
            return;
        }
    }
}
