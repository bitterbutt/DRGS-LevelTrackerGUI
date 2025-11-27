using System;
using Assets.Scripts.LevelGeneration;
using UnityEngine;

namespace LevelTracker
{
	public static class Utils
	{
		public static string GetDirectionArrow(Vector3 from, Vector3 to)
		{
			Vector3 normalized = (to - from).normalized;
			float num = (Mathf.Atan2(normalized.z, normalized.x) * 57.29578f + 360f) % 360f;
			string text;
			switch (Mathf.RoundToInt(num / 45f) % 8)
			{
			case 0:
				text = "Right";
				break;
			case 1:
				text = "UpRight";
				break;
			case 2:
				text = "Up";
				break;
			case 3:
				text = "UpLeft";
				break;
			case 4:
				text = "Left";
				break;
			case 5:
				text = "DownLeft";
				break;
			case 6:
				text = "Down";
				break;
			case 7:
				text = "DownRight";
				break;
			default:
				text = "Unknown";
				break;
			}
			return text;
		}

		public static string ExGetMaterialName(this MineableBlock block)
		{
			return Enum.GetName(typeof(ECurrency), block.materialType);
		}

		public static bool ExIsValid(this MineableBlock block)
		{
			return block.IsAlive;
		}

		public static string Extend(this string original, int length)
		{
			bool flag = original.Length >= length;
			string text;
			if (flag)
			{
				text = original;
			}
			else
			{
				int num = 0;
				while (original.Length < length && num++ < 50)
				{
					original += " ";
				}
				text = original;
			}
			return text;
		}
	}
}
