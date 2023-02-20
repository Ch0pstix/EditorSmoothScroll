namespace EditorSmoothScroll;

#pragma warning disable IDE0052

// TODO: Implement smooth zooming

internal class MouseProcessor : MouseProcessorBase
{
    public MouseProcessor(IWpfTextView wpfTextView, ExtensionOptions options)
    {
        this.wpfTextView = wpfTextView;
        this.options = options;
    }

    public override void PreprocessMouseWheel(MouseWheelEventArgs e)
    {
        if (!options?.IsExtensionEnabled ?? false)
            return;

        if (Utilities.CheckKeys(Key.LeftCtrl, Key.RightCtrl))
            return;

        if (options.UseShiftForHorizontalScrolling &&
            Utilities.CheckKeys(Key.LeftShift, Key.RightShift))
        {
            StartScroll(-e.Delta, ScrollDirection.Horizontal);
            e.Handled = true;
            return;
        }

        StartScroll(e.Delta, ScrollDirection.Vertical);
        e.Handled = true;
    }

    public override void PostprocessMouseDown(MouseButtonEventArgs e)
    {
        cancellationTokenSource?.Cancel();
        Cleanup();
    }

    private void ScrollWorker_Execute()
    {
        time = Utilities.GetTimeOffset();

        while (Math.Abs(currentDistance) > 0 && !cancellationTokenSource.IsCancellationRequested)
        {
            var offset = Utilities.GetTimeOffset();
            delta = offset - time;

            ScrollWorker_Update();

            time = offset;
        }

        Cleanup();
    }

    private void ScrollWorker_Update()
    {
        if (!options?.IsExtensionEnabled ?? false)
            return;

        lock (locker)
        {
            currentDistance = Utilities.Interpolate(currentDistance, 0, delta, options.EasingFunction);
            totalDistance = currentDistance;
        }

        ScrollViewport(totalDistance * delta, direction);
    }

    private void ScrollViewport(double amount, ScrollDirection direction)
    {
        ThreadHelper.JoinableTaskFactory.Run(async () =>
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            switch (direction)
            {
                case ScrollDirection.Horizontal:
                    wpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(amount);
                    break;
                case ScrollDirection.Vertical:
                    wpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(amount);
                    break;
            }
        });
    }

    private void StartScroll(double amount, ScrollDirection direction)
    {
        lock (locker)
        {
            bool isNewDirection = this.direction != direction;
            this.direction = direction;
            currentDistance = !isNewDirection ? currentDistance + amount : 0;
        }

        cancellationTokenSource = new();
        cancellationToken = cancellationTokenSource.Token;
        scrollWorker ??= Task.Run(ScrollWorker_Execute, cancellationToken);
    }

    private void Cleanup()
    {
        totalDistance = currentDistance = 0;
        cancellationToken = CancellationToken.None;
        cancellationTokenSource = null;
        scrollWorker = null;
    }

    private readonly Locker locker = new();
    private readonly IWpfTextView wpfTextView;
    private readonly ExtensionOptions options;

    private ScrollDirection direction = ScrollDirection.Vertical;
    private Task scrollWorker = null;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;
    private double totalDistance, currentDistance;
    private double time, delta;

    private enum ScrollDirection
    {
        Horizontal,
        Vertical,
    }
}

#pragma warning restore IDE0052
