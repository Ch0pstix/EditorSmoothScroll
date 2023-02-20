namespace EditorSmoothScroll;

[Export(typeof(IMouseProcessorProvider))]
[ContentType("text")]
[Name("EditorSmoothScroll.MouseProcessor")]
[TextViewRole(PredefinedTextViewRoles.Interactive)]
internal sealed class MouseProcessorProvider : IMouseProcessorProvider
{
    IMouseProcessor IMouseProcessorProvider.GetAssociatedProcessor(IWpfTextView wpfTextView)
    {
        var options = ThreadHelper.JoinableTaskFactory.Run(Package.Current.GetOptionsAsync);
        return new MouseProcessor(wpfTextView, options);
    }
}
