namespace EditorSmoothScroll.Options;

internal partial class OptionsProvider
{
	[ComVisible(true)]
	public class ExtensionOptions : BaseOptionPage<Options.ExtensionOptions> { }
}

public class ExtensionOptions : BaseOptionModel<ExtensionOptions>
{
	[Category("General")]
	[DisplayName("Enabled")]
	[Description("Sets whether or not the extension is enabled.")]
	[DefaultValue(true)]
	public bool IsExtensionEnabled { get; set; } = true;

	[Category("General")]
	[DisplayName("Smoothing function")]
	[Description("Sets the easing function that the smooth scrolling engine should use.")]
	[DefaultValue(EasingFunction.Quintic)]
	public EasingFunction EasingFunction { get; set; } = EasingFunction.Quintic;
}
