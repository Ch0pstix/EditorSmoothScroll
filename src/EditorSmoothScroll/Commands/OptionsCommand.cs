namespace EditorSmoothScroll.Commands;

[Command(PackageIds.OptionsCommand)]
internal sealed class OptionsCommand : BaseCommand<OptionsCommand>
{
	protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
	{
		await VS.Settings.OpenAsync<OptionsProvider.ExtensionOptions>();
	}
}
