using FloatSoda.RenderObjects;
using FloatSoda.Widgets;

namespace FloatSoda.Elements;

/// <summary>
/// 既存のRenderObjectをルートとしてWidgetツリーを接続するアダプターに対応するElementです。
/// </summary>
/// <typeparam name="T">ルートとして使用するRenderObjectの型。</typeparam>
/// <seealso cref="RenderObjectToWidgetAdapter"/>
public class RenderObjectToWidgetElement<T> : RenderObjectElement<T>
    where T : RenderObject
{
    /// <summary>
    /// 次の再構築時に適用する新しいルートWidgetを取得または設定します。
    /// 保留中の更新がない場合は<see langword="null"/>です。
    /// </summary>
    /// <remarks>値は再構築時に一度だけ消費され、適用前に<see langword="null"/>へ戻されます。</remarks>
    public Widget? NewWidget { get; set; }

    /// <summary>
    /// このElementが描画するルートWidgetを取得します。
    /// </summary>
    /// <remarks>
    /// アダプターWidgetそのものではなく、アダプターが宣言する子（ウィジェットツリーのルート）を返します。
    /// アダプターWidgetがまだ設定されていない場合は<see langword="null"/>です。
    /// </remarks>
    public Widget? ChildWidget { get; private set; }
    private Element? Child { get; set; }

    /// <summary>
    /// ElementツリーとRenderObjectツリーへ接続し、アダプターが宣言する子Elementを生成します。
    /// </summary>
    /// <param name="parent">接続先の親Element。ルートとして接続する場合は<see langword="null"/>。</param>
    public override void Mount(Element? parent)
    {
        base.Mount(parent);
        OnRebuild();
    }

    /// <summary>
    /// ルートのアダプターWidgetを置き換え、その子Elementを更新します。
    /// </summary>
    /// <param name="newWidget">既存のルートRenderObjectを引き継ぐ新しいアダプターWidget。</param>
    public override void Update(Widget newWidget)
    {
        base.Update(newWidget);
        OnRebuild();
    }

    private void OnRebuild()
    {
        var childWidget = (Widget as RenderObjectToWidgetAdapter)?.Child;
        ChildWidget = childWidget;
        Child = UpdateChild(Child, childWidget);
    }

    /// <summary>
    /// 保留中のルートWidget更新を適用した後、現在の構成をルートRenderObjectへ反映します。
    /// </summary>
    public override void PerformRebuild()
    {
        if (NewWidget is not null)
        {
            var tmp = NewWidget;
            NewWidget = null;
            Update(tmp);
        }

        base.PerformRebuild();
    }

    /// <inheritdoc/>
    public override RenderObject? RenderObject { get; protected set; }

    /// <inheritdoc/>
    public override void InsertRenderObjectChild(RenderObject? child)
    {
        if (RenderObject is IHasSingleChildRenderObject ro)
            ro.Child = child;
    }

    /// <inheritdoc/>
    public override void VisitChildren(Action<Element> visitor)
    {
        if (Child is not null)
            visitor(Child);
    }
}
