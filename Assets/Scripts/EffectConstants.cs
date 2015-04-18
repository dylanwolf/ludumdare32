using UnityEngine;
using System.Collections;

public static class EffectConstants {

	static readonly Color DAMAGE_COLOR = new Color(1, 0, 0);
	static readonly Color FREEZE_COLOR = new Color(0, 0, 1);
	static readonly Color BREAK_COLOR = new Color(0.5f, 1, 0.5f);

	public static Color GetEffectColor(LightPower power)
	{
		switch (power)
		{
			case LightPower.Damage:
				return DAMAGE_COLOR;

			case LightPower.BreakTerrain:
				return BREAK_COLOR;

			case LightPower.Freeze:
				return FREEZE_COLOR;

			default:
				return Color.white;
		}
	}
}
