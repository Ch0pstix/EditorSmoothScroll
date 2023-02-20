namespace EditorSmoothScroll;

[PackageRegistration(
    UseManagedResourcesOnly = true,
    AllowsBackgroundLoading = true)]
[InstalledProductRegistration(
    Vsix.Name,
    Vsix.Description,
    Vsix.Version)]
[ProvideAutoLoad(
    VSConstants.UICONTEXT.NoSolution_string,
    PackageAutoLoadFlags.BackgroundLoad)]
[ProvideService(
    typeof(Package),
    IsAsyncQueryable = true)]
[ProvideMenuResource(
    "Menus.ctmenu", 1)]
[ProvideOptionPage(
    typeof(OptionsProvider.ExtensionOptions),
    "Editor Smooth Scroll", "Extension",
    0, 0, true, SupportsProfiles = true)]
[ProvideProfile(
    typeof(OptionsProvider.ExtensionOptions),
    "Editor Smooth Scroll", "Extension",
    0, 0, true)]
[Guid(PackageGuids.EditorSmoothScrollString)]
public sealed partial class EditorSmoothScrollPackage : ToolkitPackage
{
    public EditorSmoothScrollPackage()
    {
        Current ??= this;
    }

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await this.RegisterCommandsAsync();
        await base.InitializeAsync(cancellationToken, progress);
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
    }

    public async Task<ExtensionOptions> GetOptionsAsync()
    {
        return await ExtensionOptions.GetLiveInstanceAsync();
    }

    public static Package Current { get; private set; }
}
