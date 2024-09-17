using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CachedImageNormalOrAnimated
{
	private static Dictionary<string, CachedImageNormalOrAnimated> cache = new Dictionary<string, CachedImageNormalOrAnimated>();

	private const string LOG_TAG = "[AutoImageCache]";
	public const int DEFAULT_AUTO_TEXTURE_CACHE_SIZE_PC = 512;
	public readonly string Identifier = string.Empty;
	public readonly Texture2D mainTexture;
	public readonly Sprite mainSprite;
	public readonly List<Texture2D> allTextures = new List<Texture2D>();
	public readonly List<Sprite> allSprites = new List<Sprite>();
	public readonly List<float> secondDelays = new List<float>();
	public readonly int frames;
	public readonly float totalTime;
	public readonly bool animated;
	public readonly bool valid;
	public readonly int width;
	public readonly int height;
	private static int textureCacheSize = -1;

	private static int currentExistingElementsCount = 0;

	public CachedImageNormalOrAnimated(string id, List<Sprite> frameList)
	{
		this.Identifier = id;
		this.frames = frameList.Count;
		this.mainTexture = frameList[0].texture;
		this.width = this.mainTexture.width;
		this.height = this.mainTexture.height;
		this.animated = this.frames > 1;
		int num = 1024;
		float num2 = (float)(num / this.mainTexture.width);
		_ = num / this.mainTexture.height;
		float num4 = 100f / num2;
		this.mainSprite = Sprite.Create(this.mainTexture, new Rect(0f, 0f, (float)this.mainTexture.width, (float)this.mainTexture.height), new Vector2(0.5f, 0.5f), num4);
		float num5 = 0.1f;
		this.totalTime = 0f;
		foreach (Sprite sprite in frameList)
		{
			Sprite sprite2 = Sprite.Create(sprite.texture, new Rect(0f, 0f, (float)sprite.texture.width, (float)sprite.texture.height), new Vector2(0.5f, 0.5f), num4);
			this.allTextures.Add(sprite.texture);
			this.allSprites.Add(sprite2);
			this.secondDelays.Add(num5);
			this.totalTime += num5;
		}
		this.valid = true;
	}

	private CachedImageNormalOrAnimated(string id, List<Texture2D> textures)
	{
		this.Identifier = id;
		this.frames = 1;
		this.mainTexture = textures[0];
		this.width = this.mainTexture.width;
		this.height = this.mainTexture.height;
		int num = 1024;
		float num2 = (float)(num / this.mainTexture.width);
		//int num3 = num / this.mainTexture.height;
		float num4 = 100f / num2;
		this.mainSprite = Sprite.Create(this.mainTexture, new Rect(0f, 0f, (float)this.mainTexture.width, (float)this.mainTexture.height), new Vector2(0.5f, 0.5f), num4);
		this.allTextures.Add(this.mainTexture);
		this.allSprites.Add(this.mainSprite);
		this.valid = true;
	}

	public static CachedImageNormalOrAnimated CreateOrGetCachedFromTexture(string id, List<Texture2D> textures)
	{
		Debug.Log("Getting cache: " + id);
		if (textures == null || id == null || textures.Count <= 0)
		{
			return null;
		}
		CachedImageNormalOrAnimated cachedImageNormalOrAnimated;
		if (CachedImageNormalOrAnimated.cache.TryGetValue(id, out cachedImageNormalOrAnimated))
		{
			return cachedImageNormalOrAnimated;
		}
		cachedImageNormalOrAnimated = new CachedImageNormalOrAnimated(id, textures);
		return cachedImageNormalOrAnimated;
	}

	public static int GetAllExistingElemetsCount()
	{
		return CachedImageNormalOrAnimated.currentExistingElementsCount;
	}

	public static CachedImageNormalOrAnimated CreateOrGetCached(string id, DownloadHandlerTexture dh)
	{
		Debug.Log("Getting cache: " + id);

		if (id != null)
		{
			CachedImageNormalOrAnimated cachedImageNormalOrAnimated;
			if (CachedImageNormalOrAnimated.cache.TryGetValue(id, out cachedImageNormalOrAnimated))
			{
				return cachedImageNormalOrAnimated;
			}
			if (dh != null)
			{
				cachedImageNormalOrAnimated = new CachedImageNormalOrAnimated(id, dh);
				return cachedImageNormalOrAnimated;
			}
		}
		return null;
	}

	private CachedImageNormalOrAnimated(string id, DownloadHandlerTexture dh)
	{
		try
		{
			if (CachedImageNormalOrAnimated.textureCacheSize <= 0)
			{
				Debug.Log(string.Format("{0} Initializing auto image cache. Cache size: {1} images.", "[AutoImageCache]", CachedImageNormalOrAnimated.textureCacheSize));
			}
			if (id == null || id.Length < 1 || dh == null || dh.data == null)
			{
				return;
			}
			this.Identifier = id;
			Texture2D texture = dh.texture;
			this.valid = texture != null && texture.width != 8 && texture.height != 8;
			if (this.valid)
			{
				texture.filterMode = FilterMode.Bilinear;
				texture.wrapMode = TextureWrapMode.Clamp;
				this.frames = 1;
				this.mainTexture = texture;
				this.width = texture.width;
				this.height = texture.height;
				int num = 1024;
				float num2 = (float)(num / this.mainTexture.width);
				int num3 = num / this.mainTexture.height;
				float num4 = 100f / num2;
				this.mainSprite = Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), new Vector2(0.5f, 0.5f), num4, 0U, SpriteMeshType.FullRect, Vector4.zero, false);
				this.allTextures.Add(this.mainTexture);
				this.allSprites.Add(this.mainSprite);
			}
			else
			{
				//TODO: Add this to hollows

				/*using (GifStream gifStream = new GifStream(dh.data))
				{
					while (gifStream.HasMoreData)
					{
						GifStream.Token currentToken = gifStream.CurrentToken;
						if (currentToken != GifStream.Token.Image)
						{
							if (currentToken != GifStream.Token.Comment)
							{
								gifStream.SkipToken();
							}
							else
							{
								gifStream.SkipToken();
							}
						}
						else
						{
							GifImage gifImage = gifStream.ReadImage();
							bool flag = true;
							Texture2D texture2D = new Texture2D(gifStream.Header.width, gifStream.Header.height, TextureFormat.ARGB32, flag);
							texture2D.SetPixels32(gifImage.colors);
							texture2D.Apply();
							texture2D.filterMode = FilterMode.Bilinear;
							texture2D.wrapMode = TextureWrapMode.Clamp;
							float safeDelaySeconds = gifImage.SafeDelaySeconds;
							int num5 = 1024;
							float num6 = (float)(num5 / texture2D.width);
							int num7 = num5 / texture2D.height;
							float num8 = 100f / num6;
							Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), num8);
							this.totalTime += safeDelaySeconds;
							this.secondDelays.Add(safeDelaySeconds);
							this.allSprites.Add(sprite);
							this.allTextures.Add(texture2D);
							if (this.frames == 0)
							{
								this.mainTexture = texture2D;
								this.mainSprite = sprite;
								this.width = this.mainTexture.width;
								this.height = this.mainTexture.height;
							}
							this.frames++;
						}
					}
				}
				this.valid = true;
				this.animated = true;*/
			}
			if (this.allSprites.Count == 0 || (this.allSprites.Count > 0 && this.allSprites[0] == null))
			{
				this.valid = false;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[AutoImageCache] Failed to decode image/GIF. Reason: " + ex.Message + " --- " + ex.StackTrace);
			this.valid = false;
		}
		if (this.valid)
		{
			this.removeElementIfCacheFull();
			CachedImageNormalOrAnimated.cache.Add(id, this);
			CachedImageNormalOrAnimated.currentExistingElementsCount++;
			return;
		}
		this.Cleanup();
	}

	private void removeElementIfCacheFull()
	{
		if (CachedImageNormalOrAnimated.cache.Count > 512)
		{
			System.Random random = new global::System.Random();
			KeyValuePair<string, CachedImageNormalOrAnimated> keyValuePair = CachedImageNormalOrAnimated.cache.ElementAt(random.Next(0, CachedImageNormalOrAnimated.cache.Count));
			CachedImageNormalOrAnimated.cache.Remove(keyValuePair.Key);
		}
	}

	public Sprite GetForTime(float time)
	{
		if (!this.animated || (this.animated && this.frames == 1))
		{
			return this.mainSprite;
		}
		float mod = time % this.totalTime;
		float t = 0f;
		int sprite = 0;
		foreach (float delay in this.secondDelays)
		{
			t += delay;
			if (mod <= t)
			{
				return this.allSprites[sprite];
			}
			sprite++;
		}
		return null;
	}

	~CachedImageNormalOrAnimated()
	{
		CachedImageNormalOrAnimated.currentExistingElementsCount--;
		this.Cleanup();
	}

	public void Cleanup()
	{
		if (this.allTextures != null)
		{
			foreach (Texture2D texture2D in this.allTextures)
			{
				if (texture2D != null)
					UnityEngine.Object.Destroy(texture2D);
			}
			this.allTextures.Clear();
		}
	}

	public static void ClearCache()
	{
		//Internals are here in proper DLL
	}

	public static bool HasValidInCache(string id)
	{
		CachedImageNormalOrAnimated cachedImageNormalOrAnimated;
		return id != null && CachedImageNormalOrAnimated.cache != null && CachedImageNormalOrAnimated.cache.Count > 0 && CachedImageNormalOrAnimated.cache.TryGetValue(id, out cachedImageNormalOrAnimated) && cachedImageNormalOrAnimated != null && cachedImageNormalOrAnimated.valid;
	}
}