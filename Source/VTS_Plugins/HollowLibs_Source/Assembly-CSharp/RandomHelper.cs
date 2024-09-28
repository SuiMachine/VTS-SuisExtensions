using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomHelper : MonoBehaviour
{
	public static Tuple<bool, RandomTracker> InitRandomTracker(string _rollName, float _initialChance, float _chanceIncreaseOnRoll, float _maxChance, int _guaranteedTrueAfterRolls, bool _lockWhenTrue)
	{
		if (_rollName == null || _rollName.IsNullOrEmptyOrWhitespace() || RandomHelper.randomTrackers.ContainsKey(_rollName))
		{
			return new Tuple<bool, RandomTracker>(false, null);
		}
		RandomTracker randomTracker = new RandomTracker(_rollName, _initialChance, _chanceIncreaseOnRoll, _maxChance, _guaranteedTrueAfterRolls, _lockWhenTrue);
		RandomHelper.randomTrackers.Add(_rollName, randomTracker);
		return new Tuple<bool, RandomTracker>(true, randomTracker);
	}

	public static bool TryGetTracker(string trackerName, out RandomTracker tracker)
	{
		if (trackerName == null)
		{
			tracker = null;
			return false;
		}
		return RandomHelper.randomTrackers.TryGetValue(trackerName, out tracker);
	}

	public static bool RollFor(string trackerName)
	{
		RandomTracker randomTracker;
		return RandomHelper.TryGetTracker(trackerName, out randomTracker) && randomTracker.Roll();
	}

	public static bool RandomBool()
	{
		return RandomHelper.RandomInt01() == 0;
	}

	public static int RandomInt01()
	{
		return UnityEngine.Random.Range(0, 2);
	}

	public static float RandomFloat01Inclusive()
	{
		return UnityEngine.Random.Range(0f, 1f);
	}

	public static float RandomFloatPlusMinusOneInclusive()
	{
		return UnityEngine.Random.Range(-1f, 1f);
	}

	public static bool RollRandom(float probability)
	{
		return probability > 0f && (probability >= 1f || UnityEngine.Random.Range(0f, 1f) <= probability);
	}

	public static int RandomIntInclusive(int min, int max)
	{
		if (min >= max)
		{
			return min;
		}
		return UnityEngine.Random.Range(min, max + 1);
	}

	public static float RandomFloatInclusive(float min, float max)
	{
		if (min >= max)
		{
			return UnityEngine.Random.Range(max, min);
		}
		return UnityEngine.Random.Range(min, max);
	}

	public static Tuple<float, float, float> ThreeFloatsThatAddUpToOne()
	{
		float num = RandomHelper.RandomFloatInclusive(2f, 1024f);
		float num2 = RandomHelper.RandomFloatInclusive(2f, 1024f);
		float num3 = RandomHelper.RandomFloatInclusive(2f, 1024f);
		float num4 = num + num2 + num3;
		return new Tuple<float, float, float>(num / num4, num2 / num4, num3 / num4);
	}

	private static Dictionary<string, RandomTracker> randomTrackers = new Dictionary<string, RandomTracker>();
}

[Serializable]
public class RandomTracker
{
	[SerializeField] private readonly string rollName = string.Empty;
	[SerializeField] private readonly float initialChance;
	[SerializeField] private readonly float chanceIncreaseOnRoll;
	[SerializeField] private readonly float maxChance;
	[SerializeField] private readonly int guaranteedTrueAfterRolls = -1;
	[SerializeField] private readonly bool lockWhenTrue;
	[SerializeField] private float currentChance;
	[SerializeField] private int totalRollsSoFar;
	[SerializeField] private bool lastRollResult;

	public RandomTracker(string _rollName, float _initialChance, float _chanceIncreaseOnRoll, float _maxChance, int _guaranteedTrueAfterRolls, bool _lockWhenTrue)
	{
		this.rollName = _rollName;
		this.initialChance = _initialChance.Clamp01();
		this.chanceIncreaseOnRoll = _chanceIncreaseOnRoll.Clamp01();
		this.maxChance = _maxChance.Clamp01();
		this.guaranteedTrueAfterRolls = _guaranteedTrueAfterRolls;
		this.lockWhenTrue = _lockWhenTrue;
		this.currentChance = this.initialChance;
		this.totalRollsSoFar = 0;
		this.lastRollResult = false;
	}

	public bool Roll()
	{
		if (this.lastRollResult && this.lockWhenTrue)
		{
			return this.lastRollResult;
		}
		if (this.guaranteedTrueAfterRolls > -1 && this.totalRollsSoFar > this.guaranteedTrueAfterRolls)
		{
			this.lastRollResult = true;
			return this.lastRollResult;
		}
		this.lastRollResult = RandomHelper.RandomFloat01Inclusive() <= this.currentChance;
		this.totalRollsSoFar++;
		this.currentChance = (this.currentChance + this.chanceIncreaseOnRoll).ClampBetween(0f, this.maxChance);
		return this.lastRollResult;
	}

	public static explicit operator string(RandomTracker tracker)
	{
		if (tracker != null)
		{
			return tracker.rollName;
		}
		return string.Empty;
	}

	public bool GetLastRollResult()
	{
		return this.lastRollResult;
	}

	public float GetCurrentRollChance()
	{
		return this.currentChance;
	}

	public int GetTotalAmountOfRollsSoFar()
	{
		return this.totalRollsSoFar;
	}
}
