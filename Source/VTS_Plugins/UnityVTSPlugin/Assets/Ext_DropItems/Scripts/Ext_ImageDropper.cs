using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Ext_DropItems.Scripts
{
	public class Ext_ImageDropper : MonoBehaviour
	{
		public static Ext_ImageDropper Instance { get; private set; }
		public int cacheCount;
		public int allCount;

		public PhysicsMaterial2D DropEmoteMaterial;
		public bool lockRotation;

		public Ext_ImageDrop DropTemplate;
		private static System.Random rand = new System.Random();

		private Vector2 currentCenter = Vector2.zero;

		private float currentMax_x;
		private float currentMin_x;
		private static List<string> currentFilterList = new List<string>();
		//private MultiSelectionWindowRequest twitchDropperConfig;

		
		private static float bounciness = 0.5f;
		private static float size = 1f;
		private static float rotation = 1f;
		private static float animationSpeed = 1f;
		private static float lifeTime = 9f;
		private static float dropSpeed = 0.3f;
		private static float gravityMultiplier = 1f;
		private static bool smoothBorders = true;
		private static float opacity = 1f;
		private static float dropChancePercent = 100f;
		private static bool bounceFromModel = true;
		private static bool showNotifications = true;
		public const string DEFAULT_EMOTE_NAME_FILTER = "";
		public const int DEFAULT_PER_MESSAGE_LIMIT = 3;
		public const int DEFAULT_ON_SCREEN_LIMIT = 50;
		public const bool DEFAULT_REQUIRE_SUB = false;
		public const bool DEFAULT_REQUIRE_SP = false;
		public const bool DEFAULT_SMOOTH_BORDERS = true;
		public const bool DEFAULT_BOUNCE_MODEL = true;
		public const float DEFAULT_BOUNCINESS = 0.5f;
		public const float DEFAULT_SIZE = 1f;
		public const float DEFAULT_ROTATION = 1f;
		public const float DEFAULT_ANIM_SPEED = 1f;
		public const float DEFAULT_DROP_SPEED = 0.3f;
		public const float DEFAULT_GRAVITY_MULT = 1f;
		public const float DEFAULT_LIFETIME = 9f;
		public const float DEFAULT_OPACITY = 1f;
		public const float DEFAULT_DROP_CHANCE = 100f;

		public PolygonCollider2D ModelPolyCollider; // TODO: this has to reference an object in the scene

		private void Awake()
		{
			this.cacheCount = 0;
			this.allCount = 0;

			if (Instance == null)
				Instance = this;
			else
				Destroy(this.gameObject);
		}

		// Token: 0x06001ED1 RID: 7889 RVA: 0x000D1EB8 File Offset: 0x000D00B8
		private void Start()
		{
			//this.DropTemplate.gameObject.SetActive(false);
			this.loadFromConfig();
			this.gameObject.SetActive(false);
			this.Invoke(nameof(checkOnInitiallyDelayed), 6.1f);
		}

		private void loadFromConfig()
		{
			/*TwitchDropper.on = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_ONOFF, false);
			TwitchDropper.emotesPerMessage = ConfigManager.GetInt(ConfigManager.C_TW_F_DROPPER_PER_MESSAGE_LIMIT, TwitchDropper.emotesPerMessage).ClampBetween(1, 30);
			TwitchDropper.emotesTotalOnScreen = ConfigManager.GetInt(ConfigManager.C_TW_F_DROPPER_TOTAL_LIMIT, TwitchDropper.emotesTotalOnScreen).ClampBetween(1, 400);
			TwitchDropper.bounciness = ConfigManager.GetFloat(ConfigManager.C_TW_F_DROPPER_BOUNCINESS, TwitchDropper.bounciness).ClampBetween(0f, 1f);
			TwitchDropper.size = ConfigManager.GetFloat(ConfigManager.C_TW_F_DROPPER_SIZE, TwitchDropper.size).ClampBetween(0.5f, 3f);
			TwitchDropper.rotation = ConfigManager.GetFloat(ConfigManager.C_TW_F_DROPPER_ROTATION, TwitchDropper.rotation).ClampBetween(0f, 1f);
			TwitchDropper.animationSpeed = ConfigManager.GetFloat(ConfigManager.C_TW_F_DROPPER_ANIMATION_SPEED, TwitchDropper.animationSpeed).ClampBetween(0f, 2f);
			TwitchDropper.lifeTime = ConfigManager.GetFloat(ConfigManager.C_TW_F_DROPPER_LIFETIME, TwitchDropper.lifeTime).ClampBetween(1f, 15f);
			TwitchDropper.dropSpeed = ConfigManager.GetFloat(ConfigManager.C_TW_F_DROPPER_INITIAL_DROP_SPEED, TwitchDropper.dropSpeed).ClampBetween(0f, 1f);
			TwitchDropper.gravityMultiplier = ConfigManager.GetFloat(ConfigManager.C_TW_F_DROPPER_GRAVITY_MULTIPLIER, TwitchDropper.gravityMultiplier).ClampBetween(0.1f, 2f);
			TwitchDropper.opacity = ConfigManager.GetFloat(ConfigManager.C_TW_F_DROPPER_OPACITY, TwitchDropper.opacity).ClampBetween(0.1f, 1f);
			TwitchDropper.dropChancePercent = ConfigManager.GetFloat(ConfigManager.C_TW_F_DROPPER_DROP_CHANCE_PERCENT, TwitchDropper.dropChancePercent).ClampBetween(0.01f, 100f);
			TwitchDropper.bounceFromModel = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_BOUNCE_MODEL, TwitchDropper.bounceFromModel);
			TwitchDropper.requireSubscribe = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_REQUIRE_SUBSCRIBE, TwitchDropper.requireSubscribe);
			TwitchDropper.requireYouModVIP = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_REQUIRE_YOU_MOD_VIP, TwitchDropper.requireYouModVIP);
			TwitchDropper.smoothBorders = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_SMOOTH_EMT_BORDERS, TwitchDropper.smoothBorders);
			TwitchDropper.showNotifications = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_SHOW_NOTIFICATIONS, TwitchDropper.showNotifications);
			TwitchDropper.currentFilter = ConfigManager.GetString(ConfigManager.C_TW_F_DROPPER_EMOTE_NAME_FILTER, TwitchDropper.currentFilter);
			if (TwitchDropper.currentFilter == null || TwitchDropper.currentFilter.Length > 512)
			{
				TwitchDropper.currentFilter = ((TwitchDropper.currentFilter == null) ? "" : TwitchDropper.currentFilter);
				TwitchDropper.currentFilter = StringHelper.MakeSureAtMostThisLong(TwitchDropper.currentFilter, 512);
				ConfigManager.SetString(ConfigManager.C_TW_F_DROPPER_EMOTE_NAME_FILTER, TwitchDropper.currentFilter);
			}
			TwitchDropperIncludedEmotes.Include_BTTV_User = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_EMOTES_BTTV_USER, true);
			TwitchDropperIncludedEmotes.Include_BTTV_Global = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_EMOTES_BTTV_GLOBAL, false);
			TwitchDropperIncludedEmotes.Include_7TV_User = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_EMOTES_7TV_USER, true);
			TwitchDropperIncludedEmotes.Include_7TV_Global = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_EMOTES_7TV_GLOBAL, false);
			TwitchDropperIncludedEmotes.Include_FFZ_User = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_EMOTES_FFZ_USER, true);
			TwitchDropperIncludedEmotes.Include_FFZ_Global = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_EMOTES_FFZ_GLOBAL, false);
			TwitchDropEdgeBounce.Set_Top_Allowed(ConfigManager.GetInt(ConfigManager.C_TW_F_DROPPER_BOUNCE_TOP, TwitchDropEdgeBounce.Get_Top_Allowed()));
			TwitchDropEdgeBounce.Set_Bottom_Allowed(ConfigManager.GetInt(ConfigManager.C_TW_F_DROPPER_BOUNCE_BOTTOM, TwitchDropEdgeBounce.Get_Bottom_Allowed()));
			TwitchDropEdgeBounce.Set_Left_Allowed(ConfigManager.GetInt(ConfigManager.C_TW_F_DROPPER_BOUNCE_LEFT, TwitchDropEdgeBounce.Get_Left_Allowed()));
			TwitchDropEdgeBounce.Set_Right_Allowed(ConfigManager.GetInt(ConfigManager.C_TW_F_DROPPER_BOUNCE_RIGHT, TwitchDropEdgeBounce.Get_Right_Allowed()));
			this.setCurrentFilter(TwitchDropper.currentFilter);
			TwitchDrop.SetBounciness(TwitchDropper.bounciness);
			TwitchDrop.SetSize(TwitchDropper.size);
			TwitchDrop.SetRotation(TwitchDropper.rotation);
			TwitchDrop.SetAnimationSpeed(TwitchDropper.animationSpeed);
			TwitchDrop.SetLifeTime(TwitchDropper.lifeTime);
			TwitchDrop.SetOpacity(TwitchDropper.opacity);
			TwitchDrop.SetInitialSpeed(TwitchDropper.dropSpeed);
			TwitchDrop.SetGravityMultiplier(TwitchDropper.gravityMultiplier);
			TwitchDrop.SetSmoothBorder(TwitchDropper.smoothBorders);*/
		}

		private void checkOnInitiallyDelayed()
		{
			Debug.Log($"[{nameof(Ext_ImageDropper)}] Turning on");
			this.gameObject.SetActive(true);
		}

		public void DropImage(string fileName)
		{
			if (!gameObject.activeSelf)
				return;

			string Path = Application.streamingAssetsPath + "/Items/" + fileName;

			UnityMainThreadDispatcher.Instance().Enqueue(delegate
			{
				this.StartCoroutine(this.GetTextureAndDrop(Path));
			});
		}

		private IEnumerator GetTextureAndDrop(string dropEmoteURL)
		{
			yield return new WaitForSeconds(0.1f);
			bool hasCached = CachedImageNormalOrAnimated.HasValidInCache(dropEmoteURL);
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(dropEmoteURL);
			if (!hasCached)
			{
				yield return www.SendWebRequest();
			}
			DownloadHandlerTexture downloadHandlerTexture = (hasCached ? null : ((DownloadHandlerTexture)www.downloadHandler));
			CachedImageNormalOrAnimated cachedImageNormalOrAnimated = CachedImageNormalOrAnimated.CreateOrGetCached(dropEmoteURL, downloadHandlerTexture);
			if (cachedImageNormalOrAnimated != null && cachedImageNormalOrAnimated.valid)
			{
				this.drop(cachedImageNormalOrAnimated);
			}
			yield break;
		}

		private void drop(CachedImageNormalOrAnimated cachedImage)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.DropTemplate.gameObject, base.transform);
			gameObject.SetActive(true);
			gameObject.GetComponent<Ext_ImageDrop>().Initialize(cachedImage, this.lockRotation, this.currentMin_x, this.currentMax_x, Ext_ImageDropper.smoothBorders);
		}

		public void ShowConfig()
		{
			/*ConfigWindowController configWindowController = ConfigWindowController.Instance();
			if (configWindowController != null)
			{
				configWindowController.Hide();
			}
			this.twitchDropperConfig = new MultiSelectionWindowRequest();
			this.twitchDropperConfig.AllowedItems = -1;
			this.twitchDropperConfig.ReSortMode = MultiSelectionRequestSortMode.DoNotReSort;
			this.twitchDropperConfig.ShowAnimationGraph = false;
			this.twitchDropperConfig.ShowHelp = false;
			this.twitchDropperConfig.OkButtonTextOverride = L.GetString("done_b_1", null);
			this.twitchDropperConfig.HideCancelButton = true;
			this.twitchDropperConfig.ExplanationString = StringHelper.EmphasisNonBold(L.GetString("twitch_ft_emote_dropper_sel_1", null)) + "\n" + L.GetString("twitch_ft_emote_dropper_sel_2", null);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("general_ft_on", null)).SetSecondLine(L.GetString("general_ft_on_e", null))
				.SetHelpLine(L.GetString("tw_f_drop_help", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_ONOFF)
				.SetCurrentlyOn(TwitchDropper.on)
				.SetInitiallyOn(TwitchDropper.on)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff)
				.SetInfoType(MultiSelectionRequestUIInfoType.Questionmark);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_filter_1", null)).SetSecondLine(L.GetString("tw_f_drop_filter_2", null))
				.SetHelpLine(L.GetString("tw_f_drop_filter_h", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_EMOTE_NAME_FILTER)
				.SetCurrentText(TwitchDropper.currentFilter)
				.SetDefaultText("")
				.SetRequestType(MultiSelectionRequestUIEntryType.TextRegular)
				.SetItemTextValidationType(MultiSelectionRequestUIEntryTextValidationType.AlphanumericAndSpace)
				.SetInfoType(MultiSelectionRequestUIInfoType.Questionmark);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_sub_1", null)).SetSecondLine(L.GetString("tw_f_drop_sub_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_REQUIRE_SUBSCRIBE)
				.SetCurrentlyOn(TwitchDropper.requireSubscribe)
				.SetInitiallyOn(TwitchDropper.requireSubscribe)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_vip_1", null)).SetSecondLine(L.GetString("tw_f_drop_vip_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_REQUIRE_YOU_MOD_VIP)
				.SetCurrentlyOn(TwitchDropper.requireYouModVIP)
				.SetInitiallyOn(TwitchDropper.requireYouModVIP)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_smooth_br_1", null)).SetSecondLine(L.GetString("tw_f_drop_smooth_br_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_SMOOTH_EMT_BORDERS)
				.SetCurrentlyOn(TwitchDropper.smoothBorders)
				.SetInitiallyOn(TwitchDropper.smoothBorders)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_lim_msg_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_lim_msg_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_PER_MESSAGE_LIMIT)
				.SetDefaultValue(3f)
				.SetInitialValue((float)TwitchDropper.emotesPerMessage)
				.SetMinValue(1f)
				.SetMaxValue(30f)
				.SetOnlyIntegers(true);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_lim_total_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_lim_total_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_TOTAL_LIMIT)
				.SetDefaultValue(50f)
				.SetInitialValue((float)TwitchDropper.emotesTotalOnScreen)
				.SetMinValue(1f)
				.SetMaxValue(400f)
				.SetOnlyIntegers(true);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.Questionmark)
				.SetFirstLine(L.GetString("tw_f_drop_chance_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_chance_2", null))
				.SetHelpLine(L.GetString("tw_f_drop_chance_h", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_DROP_CHANCE_PERCENT)
				.SetDefaultValue(100f)
				.SetInitialValue(TwitchDropper.dropChancePercent)
				.SetMinValue(0.01f)
				.SetMaxValue(100f);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_emote_bttv_u_1", null)).SetSecondLine(L.GetString("tw_f_drop_emote_bttv_u_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_EMOTES_BTTV_USER)
				.SetCurrentlyOn(TwitchDropperIncludedEmotes.Include_BTTV_User)
				.SetInitiallyOn(TwitchDropperIncludedEmotes.Include_BTTV_User)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_emote_bttv_g_1", null)).SetSecondLine(L.GetString("tw_f_drop_emote_bttv_g_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_EMOTES_BTTV_GLOBAL)
				.SetCurrentlyOn(TwitchDropperIncludedEmotes.Include_BTTV_Global)
				.SetInitiallyOn(TwitchDropperIncludedEmotes.Include_BTTV_Global)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_emote_7tv_u_1", null)).SetSecondLine(L.GetString("tw_f_drop_emote_7tv_u_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_EMOTES_7TV_USER)
				.SetCurrentlyOn(TwitchDropperIncludedEmotes.Include_7TV_User)
				.SetInitiallyOn(TwitchDropperIncludedEmotes.Include_7TV_User)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_emote_7tv_g_1", null)).SetSecondLine(L.GetString("tw_f_drop_emote_7tv_g_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_EMOTES_7TV_GLOBAL)
				.SetCurrentlyOn(TwitchDropperIncludedEmotes.Include_7TV_Global)
				.SetInitiallyOn(TwitchDropperIncludedEmotes.Include_7TV_Global)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_emote_ffz_u_1", null)).SetSecondLine(L.GetString("tw_f_drop_emote_ffz_u_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_EMOTES_FFZ_USER)
				.SetCurrentlyOn(TwitchDropperIncludedEmotes.Include_7TV_User)
				.SetInitiallyOn(TwitchDropperIncludedEmotes.Include_7TV_User)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_emote_ffz_g_1", null)).SetSecondLine(L.GetString("tw_f_drop_emote_ffz_g_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_EMOTES_FFZ_GLOBAL)
				.SetCurrentlyOn(TwitchDropperIncludedEmotes.Include_7TV_Global)
				.SetInitiallyOn(TwitchDropperIncludedEmotes.Include_7TV_Global)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetFirstLine(L.GetString("tw_f_drop_time_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_time_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_LIFETIME)
				.SetDefaultValue(9f)
				.SetInitialValue(TwitchDropper.lifeTime)
				.SetMinValue(1f)
				.SetMaxValue(15f);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetFirstLine(L.GetString("tw_f_drop_amim_sp_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_amim_sp_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_ANIMATION_SPEED)
				.SetDefaultValue(1f)
				.SetInitialValue(TwitchDropper.animationSpeed)
				.SetMinValue(0f)
				.SetMaxValue(2f);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetFirstLine(L.GetString("tw_f_drop_init_sp_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_init_sp_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_INITIAL_DROP_SPEED)
				.SetDefaultValue(0.3f)
				.SetInitialValue(TwitchDropper.dropSpeed)
				.SetMinValue(0f)
				.SetMaxValue(1f);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetFirstLine(L.GetString("tw_f_drop_gravity_1", null))
				.SetSecondLine(L.GetString("", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_GRAVITY_MULTIPLIER)
				.SetDefaultValue(1f)
				.SetInitialValue(TwitchDropper.gravityMultiplier)
				.SetMinValue(0.1f)
				.SetMaxValue(2f);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_opacity_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_opacity_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_OPACITY)
				.SetDefaultValue(1f)
				.SetInitialValue(TwitchDropper.opacity)
				.SetMinValue(0.1f)
				.SetMaxValue(1f);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_bouncy_1", null))
				.SetSecondLine("")
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_BOUNCINESS)
				.SetDefaultValue(0.5f)
				.SetInitialValue(TwitchDropper.bounciness)
				.SetMinValue(0f)
				.SetMaxValue(1f);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_emote_size_1", null))
				.SetSecondLine("")
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_SIZE)
				.SetDefaultValue(1f)
				.SetInitialValue(TwitchDropper.size)
				.SetMinValue(0.5f)
				.SetMaxValue(3f);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_rotation_sp_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_rotation_sp_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_ROTATION)
				.SetDefaultValue(1f)
				.SetInitialValue(TwitchDropper.rotation)
				.SetMinValue(0f)
				.SetMaxValue(1f);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("tw_f_drop_bounce_model_1", null)).SetSecondLine(L.GetString("tw_f_drop_bounce_model_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_BOUNCE_MODEL)
				.SetCurrentlyOn(TwitchDropper.bounceFromModel)
				.SetInitiallyOn(TwitchDropper.bounceFromModel)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_bounce_edge_btm_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_bounce_edge_btm_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_BOUNCE_BOTTOM)
				.SetDefaultValue(2f)
				.SetInitialValue((float)TwitchDropEdgeBounce.Get_Bottom_Allowed())
				.SetMinValue(0f)
				.SetMaxValue(20f)
				.SetOnlyIntegers(true);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_bounce_edge_top_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_bounce_edge_top_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_BOUNCE_TOP)
				.SetDefaultValue(0f)
				.SetInitialValue((float)TwitchDropEdgeBounce.Get_Top_Allowed())
				.SetMinValue(0f)
				.SetMaxValue(20f)
				.SetOnlyIntegers(true);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_bounce_edge_left_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_bounce_edge_left_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_BOUNCE_LEFT)
				.SetDefaultValue(20f)
				.SetInitialValue((float)TwitchDropEdgeBounce.Get_Left_Allowed())
				.SetMinValue(0f)
				.SetMaxValue(20f)
				.SetOnlyIntegers(true);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetRequestType(MultiSelectionRequestUIEntryType.Value).SetInfoType(MultiSelectionRequestUIInfoType.None)
				.SetFirstLine(L.GetString("tw_f_drop_bounce_edge_right_1", null))
				.SetSecondLine(L.GetString("tw_f_drop_bounce_edge_right_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_BOUNCE_RIGHT)
				.SetDefaultValue(20f)
				.SetInitialValue((float)TwitchDropEdgeBounce.Get_Right_Allowed())
				.SetMinValue(0f)
				.SetMaxValue(20f)
				.SetOnlyIntegers(true);
			this.twitchDropperConfig.AddNewItemWithIDFromBuilder().SetFirstLine(L.GetString("show_notifications_m_h_1", null)).SetHelpLine(L.GetString("show_notifications_m_h_2", null))
				.SetGenericData(ConfigManager.C_TW_F_DROPPER_SHOW_NOTIFICATIONS)
				.SetCurrentlyOn(TwitchDropper.showNotifications)
				.SetInitiallyOn(TwitchDropper.showNotifications)
				.SetRequestType(MultiSelectionRequestUIEntryType.OnOff)
				.SetInfoType(MultiSelectionRequestUIInfoType.Questionmark);
			this.twitchDropperConfigActive = true;
			MultiSelectionWindow.GetInstance().ShowForResult(this.twitchDropperConfig);*/
		}

		public static void RemoveAllDrops()
		{
			foreach (Ext_ImageDrop twitchDrop in FindObjectsOfType<Ext_ImageDrop>())
			{
				if (twitchDrop != null)
				{
					twitchDrop.Remove();
				}
			}
		}

		public static bool RollIfDrop()
		{
			return Ext_ImageDropper.rand.NextDouble() * 100.0 <= (double)(Ext_ImageDropper.dropChancePercent + 1E-06f);
		}	
	}
}
