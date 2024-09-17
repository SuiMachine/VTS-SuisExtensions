using System;

// Token: 0x02000569 RID: 1385
public class TwitchDropEdgeBounce
{
	private const int MAX_BOUNCE_COUNT = 20;
	private const int ZERO = 0;
	public const int TOP_DEFAULT = 0;
	public const int BOTTOM_DEFAULT = 2;
	public const int LEFT_DEFAULT = 20;
	public const int RIGHT_DEFAULT = 20;
	private static int BounceTopCount_Allowed = 0;
	private static int BounceBottomCount_Allowed = 2;
	private static int BounceLeftCount_Allowed = 20;
	private static int BounceRightCount_Allowed = 20;
	private int BounceTopCount_Done;
	private int BounceBottomCount_Done;
	private int BounceLeftCount_Done;
	private int BounceRightCount_Done;

	public TwitchDropEdgeBounce()
	{
		this.BounceTopCount_Done = 0;
		this.BounceBottomCount_Done = 0;
		this.BounceLeftCount_Done = 0;
		this.BounceRightCount_Done = 0;
	}

	public bool Bounce_Top()
	{
		bool flag = this.BounceTopCount_Done < TwitchDropEdgeBounce.BounceTopCount_Allowed;
		this.BounceTopCount_Done++;
		return flag;
	}

	public bool Bounce_Bottom()
	{
		bool flag = this.BounceBottomCount_Done < TwitchDropEdgeBounce.BounceBottomCount_Allowed;
		this.BounceBottomCount_Done++;
		return flag;
	}

	public bool Bounce_Left()
	{
		bool flag = this.BounceLeftCount_Done < TwitchDropEdgeBounce.BounceLeftCount_Allowed;
		this.BounceLeftCount_Done++;
		return flag;
	}

	public bool Bounce_Right()
	{
		bool flag = this.BounceRightCount_Done < TwitchDropEdgeBounce.BounceRightCount_Allowed;
		this.BounceRightCount_Done++;
		return flag;
	}

	public static void Set_Top_Allowed(int newValue)
	{
		TwitchDropEdgeBounce.BounceTopCount_Allowed = newValue.ClampBetween(0, 20);
	}

	public static void Set_Bottom_Allowed(int newValue)
	{
		TwitchDropEdgeBounce.BounceBottomCount_Allowed = newValue.ClampBetween(0, 20);
	}

	public static void Set_Left_Allowed(int newValue)
	{
		TwitchDropEdgeBounce.BounceLeftCount_Allowed = newValue.ClampBetween(0, 20);
	}

	public static void Set_Right_Allowed(int newValue)
	{
		TwitchDropEdgeBounce.BounceRightCount_Allowed = newValue.ClampBetween(0, 20);
	}

	public static int Get_Top_Allowed()
	{
		return TwitchDropEdgeBounce.BounceTopCount_Allowed;
	}

	public static int Get_Bottom_Allowed()
	{
		return TwitchDropEdgeBounce.BounceBottomCount_Allowed;
	}

	public static int Get_Left_Allowed()
	{
		return TwitchDropEdgeBounce.BounceLeftCount_Allowed;
	}

	public static int Get_Right_Allowed()
	{
		return TwitchDropEdgeBounce.BounceRightCount_Allowed;
	}
}
