namespace EditorSmoothScroll.Common;

internal static class Utilities
{
	public static bool CheckKeys(params Key[] keys)
        => keys.Any(Keyboard.IsKeyDown);

    public static double GetTimeOffset()
        => Environment.TickCount * 1E-03;

    public static double Interpolate(double start, double end, double t, EasingFunction ease)
    {
        return ease switch
        {
            EasingFunction.Quadratic => start + (end - start) * t * (2 - t),
            EasingFunction.Cubic => start + (end - start) * (Math.Pow(--t, 3) + 1),
            EasingFunction.Quartic => start - (end - start) * (Math.Pow(--t, 4) - 1),
            EasingFunction.Quintic => start + (end - start) * (Math.Pow(--t, 5) + 1),
            EasingFunction.Exponential => start + (end - start) * (1 - Math.Pow(2, -10 * t)),
            EasingFunction.Sine => start + (end - start) * Math.Sin(t * Math.PI / 2),
            _ => start + (end - start) * t,
        };
    }
}
