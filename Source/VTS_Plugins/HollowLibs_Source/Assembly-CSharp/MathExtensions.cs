using UnityEngine;

public static class MathExtensions
{
	// Token: 0x06000757 RID: 1879 RVA: 0x0002AE3B File Offset: 0x0002903B
	public static Vector2 ClampAll(this Vector2 toClamp, float min, float max)
	{
		toClamp.x = toClamp.x.ClampBetween(min, max);
		toClamp.y = toClamp.y.ClampBetween(min, max);
		return toClamp;
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x0002AE66 File Offset: 0x00029066
	public static Vector3 ClampAll(this Vector3 toClamp, float min, float max)
	{
		toClamp.x = toClamp.x.ClampBetween(min, max);
		toClamp.y = toClamp.y.ClampBetween(min, max);
		toClamp.z = toClamp.z.ClampBetween(min, max);
		return toClamp;
	}

	public static Vector4 ClampAll(this Vector4 toClamp, float min, float max)
	{
		toClamp.x = toClamp.x.ClampBetween(min, max);
		toClamp.y = toClamp.y.ClampBetween(min, max);
		toClamp.z = toClamp.z.ClampBetween(min, max);
		toClamp.w = toClamp.w.ClampBetween(min, max);
		return toClamp;
	}

	public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
	{
		if (fromSource == toSource)
		{
			return (fromTarget + toTarget) / 2f;
		}
		if (fromTarget == toTarget)
		{
			return fromTarget;
		}
		return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
	}

	public static float MapAndClamp(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
	{
		return value.Map(fromSource, toSource, fromTarget, toTarget).ClampBetween(fromTarget, toTarget);
	}

	public static bool FloatEquals(this float value1, float value2, float epsilon = 0.0001f)
	{
		return Mathf.Abs(value1 - value2) < epsilon;
	}

	public static float Average(this float value1, float value2)
	{
		return (value1 + value2) / 2f;
	}

	public static float ClampBetween(this float value, float minOrMax1, float minOrMax2)
	{
		bool flag = minOrMax1 < minOrMax2;
		return Mathf.Clamp(value, flag ? minOrMax1 : minOrMax2, flag ? minOrMax2 : minOrMax1);
	}

	public static Vector3 Make360AngleBetweenPlusMinus180(Vector3 rotationToTransform)
	{
		rotationToTransform.x = ((rotationToTransform.x > 180f) ? (rotationToTransform.x - 360f) : rotationToTransform.x);
		rotationToTransform.y = ((rotationToTransform.y > 180f) ? (rotationToTransform.y - 360f) : rotationToTransform.y);
		rotationToTransform.z = ((rotationToTransform.z > 180f) ? (rotationToTransform.z - 360f) : rotationToTransform.z);
		return rotationToTransform;
	}

	public static bool IsValidNumber(this float value)
	{
		return !float.IsNaN(value) && !float.IsInfinity(value);
	}

	public static bool IsValid(this Quaternion quaternion)
	{
		bool flag = float.IsNaN(quaternion.x + quaternion.y + quaternion.z + quaternion.w);
		bool flag2 = quaternion.x == 0f && quaternion.y == 0f && quaternion.z == 0f && quaternion.w == 0f;
		return !flag && !flag2;
	}

	public static float Clamp01(this float value)
	{
		if (value >= 1f)
		{
			return 1f;
		}
		if (value <= 0f)
		{
			return 0f;
		}
		return value;
	}

	public static float PercentChangeFromTo(float before, float after)
	{
		if (before == 0f)
		{
			return 0f;
		}
		return (after - before) / before;
	}

	public static float GetQuaternionZRotation(Quaternion rotation)
	{
		if (rotation.IsValid())
		{
			return rotation.eulerAngles.z;
		}
		return 0f;
	}

	public static int ClampBetween(this int value, int minOrMax1, int minOrMax2)
	{
		if (minOrMax1 > minOrMax2)
		{
			if (value >= minOrMax1)
			{
				return minOrMax1;
			}
			if (value <= minOrMax2)
			{
				return minOrMax2;
			}
		}
		else
		{
			if (value >= minOrMax2)
			{
				return minOrMax2;
			}
			if (value <= minOrMax1)
			{
				return minOrMax1;
			}
		}
		return value;
	}
}