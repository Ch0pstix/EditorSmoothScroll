namespace EditorSmoothScroll.Common;

internal static class Utilities
{
	public static bool CheckKeys(params Key[] keys)
	{
		foreach (Key key in keys)
		{
			if (Keyboard.IsKeyDown(key))
				return true;
		}

		return false;
	}

	public static double Interpolate(double start, double end, double t, EasingFunction ease)
	{
		switch (ease)
		{
			default:
			case EasingFunction.Linear:
				return start + (end - start) * t;
			case EasingFunction.Quadratic:
				return start + (end - start) * t * (2 - t);
			case EasingFunction.Cubic:
				t--; return start + (end - start) * (Math.Pow(t, 3) + 1);
			case EasingFunction.Quartic:
				t--; return start - (end - start) * (Math.Pow(t, 4) - 1);
			case EasingFunction.Quintic:
				t--; return start + (end - start) * (Math.Pow(t, 5) + 1);
			case EasingFunction.Exponential:
				return start + (end - start) * (1 - Math.Pow(2, -10 * t));
			case EasingFunction.Sine:
				return start + (end - start) * Math.Sin(t * Math.PI / 2);
		}
	}

	public static double GetTimeOffset()
	{
		return Environment.TickCount * 1E-03;
	}
}
