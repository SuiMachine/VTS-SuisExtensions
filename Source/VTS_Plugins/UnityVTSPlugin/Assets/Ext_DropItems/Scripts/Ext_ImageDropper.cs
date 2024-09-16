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

		private void Start()
		{
			this.DropTemplate.gameObject.SetActive(false);
			this.ModelPolyCollider.enabled = false;
			TwitchDropper.on = false;
			base.gameObject.SetActive(false);
			this.loadFromConfig();
			base.Invoke("checkOnInitiallyDelayed", 6.1f);
		}

		private void loadFromConfig()
		{
			TwitchDropper.on = ConfigManager.GetBool(ConfigManager.C_TW_F_DROPPER_ONOFF, false);
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
			TwitchDrop.SetSmoothBorder(TwitchDropper.smoothBorders);
		}

		private void checkOnInitiallyDelayed()
		{
			if (TwitchDropper.on)
			{
				this.Show(true, true);
			}
		}

		public void Show(bool show, bool initial)
		{
			Debug.Log("[TwitchInteraction] Turning " + (show ? "on" : "off") + " Twitch Emote Dropper.");
			TwitchDropper.on = show;
			base.gameObject.SetActive(show);
			if ((TwitchDropper.showNotifications & !initial) && !this.twitchDropperConfigActive)
			{
				string text = StringHelper.EmphasisNonBold(L.GetString("twitch_ft_emote_dropper_sel_1", null));
				string text2 = (TwitchDropper.on ? L.GetString("m_status_on", null) : L.GetString("m_status_off", null));
				InfoPushdownType infoPushdownType = (TwitchDropper.on ? InfoPushdownType.CatWoozy : InfoPushdownType.CatCancel);
				SingletonMonoBehaviour<InfoPushdown>.Instance().Push(infoPushdownType, text + ":  " + text2, 3);
			}
		}


		private void Update()
		{
			if (!TwitchDropper.on || !TwitchDropper.bounceFromModel || !TwitchConfigItem.IsOn())
			{
				if (this.ModelPolyCollider.enabled)
				{
					this.ModelPolyCollider.enabled = false;
				}
				return;
			}
			this.cacheCount = CachedImageNormalOrAnimated.GetCurrentCacheSize();
			this.allCount = CachedImageNormalOrAnimated.GetAllExistingElemetsCount();
			List<Vector2> currentModelHull = SingletonMonoBehaviour<ModelMeshHullChecker>.Instance().GetCurrentModelHull(false, true);
			float num = float.MaxValue;
			float num2 = float.MinValue;
			this.currentMin_x = 0f;
			this.currentMax_x = 0f;
			if (currentModelHull != null && currentModelHull.Count >= 3)
			{
				this.currentCenter = Handler_ModelOutlineEvent.GetCenter(currentModelHull);
				Vector2[] array = new Vector2[currentModelHull.Count];
				float num3 = 0.98f;
				int num4 = 0;
				bool flag = false;
				foreach (Vector2 vector in currentModelHull)
				{
					if (vector.x < num)
					{
						flag = true;
						num = vector.x;
					}
					if (vector.x > num2)
					{
						flag = true;
						num2 = vector.x;
					}
					array[num4] = this.currentCenter * (1f - num3) + vector * num3;
					num4++;
				}
				this.currentMin_x = (flag ? num : 0f);
				this.currentMax_x = (flag ? num2 : 0f);
				this.ModelPolyCollider.enabled = true;
				this.ModelPolyCollider.points = array;
				return;
			}
			this.ModelPolyCollider.enabled = false;
		}

		public void DropImage(string imageURL)
		{
			if (!TwitchDropper.on)
			{
				return;
			}
			UnityMainThreadDispatcher.Instance().Enqueue(delegate
			{
				this.StartCoroutine(this.GetTextureAndDrop(imageURL, null));
			});
		}

		private IEnumerator GetTextureAndDrop(string dropEmoteURL, ExtraEmote extraEmote)
		{
			yield return new WaitForSeconds(0.1f);
			if (extraEmote != null && extraEmote.Source == EmoteSource.SRC_7TV && extraEmote.Animated == EmoteAnimationType.Unknown)
			{
				yield return extraEmote.DetermineSevenTVType();
			}
			if (extraEmote != null)
			{
				dropEmoteURL = extraEmote.URL;
			}
			bool hasCached = CachedImageNormalOrAnimated.HasValidInCache(dropEmoteURL);
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(dropEmoteURL);
			if (!hasCached)
			{
				yield return www.SendWebRequest();
			}
			if (!hasCached && (www.isNetworkError || www.isHttpError))
			{
				Debug.Log("[TwitchInteraction] Emote download error: " + www.error);
			}
			else
			{
				DownloadHandlerTexture downloadHandlerTexture = (hasCached ? null : ((DownloadHandlerTexture)www.downloadHandler));
				CachedImageNormalOrAnimated cachedImageNormalOrAnimated = CachedImageNormalOrAnimated.CreateOrGetCached(dropEmoteURL, downloadHandlerTexture);
				if (cachedImageNormalOrAnimated != null && cachedImageNormalOrAnimated.valid)
				{
					this.drop(cachedImageNormalOrAnimated);
					using (List<TwitchDropperSpecialEmote>.Enumerator enumerator = this.SpecialEmotes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TwitchDropperSpecialEmote twitchDropperSpecialEmote = enumerator.Current;
							if (twitchDropperSpecialEmote.Roll())
							{
								CachedImageNormalOrAnimated image = twitchDropperSpecialEmote.GetImage();
								this.drop(image);
							}
						}
						yield break;
					}
				}
			}
			yield break;
		}

		public static int GetTotalAllowed()
		{
			return TwitchDropper.emotesTotalOnScreen;
		}

		public static int GetAllowedPerMessage()
		{
			return TwitchDropper.emotesPerMessage;
		}

		public static void RemoveAllDrops()
		{
			foreach (TwitchDrop twitchDrop in global::UnityEngine.Object.FindObjectsOfType<TwitchDrop>())
			{
				if (twitchDrop != null)
				{
					twitchDrop.Remove();
				}
			}
		}
	}
}
