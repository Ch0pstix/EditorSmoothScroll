namespace EditorSmoothScroll;

internal class MouseProcessor : MouseProcessorBase
{
    public MouseProcessor(IWpfTextView wpfTextView, ExtensionOptions options)
    {
        this.wpfTextView = wpfTextView;
        this.options = options;
    }

    public override void PreprocessMouseWheel(MouseWheelEventArgs e)
    {
        if (!options?.IsExtensionEnabled ?? false) return;

        if (Utilities.CheckKeys(Key.LeftCtrl, Key.RightCtrl))
        {
            StartZoomJob(Math.Sign(e.Delta) * 50);
            e.Handled = true;
            return;
        }

        StartScrollJob(
            scrollDirection == ScrollDirection.Horizontal ? -e.Delta : e.Delta, 
            Utilities.CheckKeys(Key.LeftShift, Key.RightShift) ? ScrollDirection.Horizontal : ScrollDirection.Vertical);
        e.Handled = true;
    }

    public override void PostprocessMouseDown(MouseButtonEventArgs e)
    {
        if (scrollJobWorker?.Status is TaskStatus.Running ||
            zoomJobWorker?.Status is TaskStatus.Running)
        {
            cancellationTokenSource?.Cancel();
            Reset();
        }
    }

    private async ValueTask ScrollViewportAsync(double distanceToScroll, ScrollDirection scrollDirection)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        switch (scrollDirection)
        {
            case ScrollDirection.Horizontal:
                wpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(distanceToScroll);
                break;
            case ScrollDirection.Vertical:
                wpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(distanceToScroll);
                break;
        }
    }

    private async ValueTask ZoomViewportAsync(double zoomLevelDifference)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        wpfTextView.ZoomLevel += zoomLevelDifference;
    }

    private void StartScrollJob(double distanceToScroll, ScrollDirection scrollDirection)
    {
        lock (locker)
        {
            bool isNewDirection = this.scrollDirection != scrollDirection;
            this.scrollDirection = scrollDirection;
            this.distanceToScroll = isNewDirection ? 0 : this.distanceToScroll + distanceToScroll;
        }

        cancellationTokenSource = new();
        cancellationToken = cancellationTokenSource.Token;
        scrollJobWorker ??= Task.Run(ScrollJob_ExecuteAsync, cancellationToken);
    }

    private void StartZoomJob(double zoomLevelDifference)
    {
        lock (locker)
            this.zoomLevelDifference += zoomLevelDifference;

        cancellationTokenSource = new();
        cancellationToken = cancellationTokenSource.Token;
        zoomJobWorker ??= Task.Run(ZoomJob_ExecuteAsync, cancellationToken);
    }

    private async ValueTask ScrollJob_ExecuteAsync()
    {
        time = Utilities.GetTimeOffset();

        while (Math.Abs(Math.Round(distanceToScroll)) > 0 && !cancellationTokenSource.IsCancellationRequested)
        {
            var offset = Utilities.GetTimeOffset();
            deltaTime = offset - time;
            await ScrollJob_UpdateAsync();
            time = offset;
        }

        Reset();
    }

    private async ValueTask ZoomJob_ExecuteAsync()
    {
        time = Utilities.GetTimeOffset();

        while (Math.Abs(Math.Round(zoomLevelDifference)) > 0 && !cancellationTokenSource.IsCancellationRequested)
        {
            var offset = Utilities.GetTimeOffset();
            deltaTime = offset - time;
            await ZoomJob_UpdateAsync();
            time = offset;
        }

        Reset();
    }

    private async ValueTask ScrollJob_UpdateAsync()
    {
        if (!options?.IsExtensionEnabled ?? false) return;

        lock (locker)
            distanceToScroll = Utilities.Interpolate(distanceToScroll, 0, deltaTime, options.EasingFunction);

        await ScrollViewportAsync(distanceToScroll * deltaTime, scrollDirection);
    }

    private async ValueTask ZoomJob_UpdateAsync()
    {
        if (!options?.IsExtensionEnabled ?? false) return;

        lock (locker)
            zoomLevelDifference = Utilities.Interpolate(zoomLevelDifference, 0, deltaTime, options.EasingFunction);

        await ZoomViewportAsync(zoomLevelDifference * deltaTime);
    }

    private void Reset()
    {
        distanceToScroll = 0;
        zoomLevelDifference = 0;
        cancellationToken = CancellationToken.None;
        cancellationTokenSource = null;
        scrollJobWorker = null;
        zoomJobWorker = null;
    }

    private readonly IWpfTextView wpfTextView;
    private readonly ExtensionOptions options;
    private readonly Locker locker = new();

    private double distanceToScroll, zoomLevelDifference, time, deltaTime;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;
    private Task scrollJobWorker, zoomJobWorker = null;
    private ScrollDirection scrollDirection = ScrollDirection.Vertical;

    private enum ScrollDirection
    {
        Horizontal,
        Vertical,
    }
}
