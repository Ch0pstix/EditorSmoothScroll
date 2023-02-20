namespace EditorSmoothScroll.Options;

using DisplayName = System.ComponentModel.DisplayNameAttribute;

internal partial class OptionsProvider
{
	[ComVisible(true)]
	public class ExtensionOptions : BaseOptionPage<Options.ExtensionOptions> { }
}

public class ExtensionOptions : BaseOptionModel<ExtensionOptions>
{
	[Category("General")]
	[DisplayName("Enable editor smooth scroll")]
	[Description("Sets whether or not the extension is enabled.")]
	[DefaultValue(true)]
	public bool IsExtensionEnabled { get; set; } = true;

	[Category("General")]
	[DisplayName("Scroll horizontally with shift")]
	[Description("Sets whether or not the shift key should cause the editor to scroll horizontally.")]
	public bool UseShiftForHorizontalScrolling { get; set; } = true;

	[Category("General")]
	[DisplayName("Easing function")]
	[Description("Sets the easing function that the smooth scrolling engine should use.")]
	[DefaultValue(EasingFunction.Quadratic)]
	public EasingFunction EasingFunction { get; set; } = EasingFunction.Quadratic;
}
