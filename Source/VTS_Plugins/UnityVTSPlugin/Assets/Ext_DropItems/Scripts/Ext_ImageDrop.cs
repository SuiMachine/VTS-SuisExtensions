using System;
using UnityEngine;

namespace Assets.Ext_DropItems.Scripts
{
	public class Ext_ImageDrop : MonoBehaviour
	{
		private const float initialAngularVelocity = 820f;
		private const float speedX_Max = 26f;
		private const float speedMin_A = 1f;
		private const float speedMax_A = 30f;
		private const float speedMin_B = 80f;
		private const float speedMax_B = 220f;
		private const int OFF_SCREEN_BOTTOM = -200;
		private static bool smoothBorder;
		private static bool updateMaterial = false;
		private static int totalOnScreen = 0;
		private static float bounciness = 0.5f;
		private static float size = 1f;
		private static float rotation = 1f;
		private static bool animate = true;
		private static float animationSpeed = 1f;
		private static float lifeTime = 10f;
		private static float opacity = 1f;
		private static float initialSpeed = 0.3f;
		private static float gravityMultiplier = 1f;
		private MinMax dropPysicsMultiplier = new MinMax(6f, 7f);
		private float dropPysicsMultiplierValue = 6.5f;

		[NonSerialized] public Camera CameraRenderer;
		public SpriteRenderer spriteRenderer;
		public CircleCollider2D circleCollider;
		public BoxCollider2D boxCollider;
		public Rigidbody2D riggidBody;
		//public Animator Animator;
		public Material DropMaterial_Default;
		public Material DropMaterial_Smooth;

		[SerializeField] private bool isTemplate = true;

		private bool dead;

		private CachedImageNormalOrAnimated cachedImage;

		private float time;

		private float rawTime;

		private float lifeTimeRandom;

		private bool rotationLocked;

		private TwitchDropEdgeBounce edgeBounce;

		public void Initialize(CachedImageNormalOrAnimated image, bool lockRotation, float modelOutlineX_min, float modelOutlineX_max, bool startWithSmoothBorder)
		{
			this.isTemplate = false;
			this.rotationLocked = lockRotation;
			Vector3 localPosition = this.transform.localPosition;
			localPosition.x = RandomHelper.RandomFloatInclusive(modelOutlineX_min, modelOutlineX_max);
			localPosition.y += RandomHelper.RandomFloat01Inclusive() * 1E-06f;
			localPosition.z -= RandomHelper.RandomFloat01Inclusive();
			this.transform.localPosition = localPosition;
			this.dropPysicsMultiplierValue = this.dropPysicsMultiplier.RandomValue;
			this.riggidBody.gravityScale = this.dropPysicsMultiplierValue * Ext_ImageDrop.gravityMultiplier;
			float num = RandomHelper.RandomFloat01Inclusive().MapAndClamp(0f, 1f, -1f, 1f) * 26f;
			float num2 = Mathf.Lerp(1f, 80f, Ext_ImageDrop.initialSpeed);
			float num3 = Mathf.Lerp(30f, 220f, Ext_ImageDrop.initialSpeed);
			float num4 = RandomHelper.RandomFloatInclusive(num2, num3) * -1f;
			this.riggidBody.velocity = new Vector2(num, num4);
			float num5 = RandomHelper.RandomFloat01Inclusive().MapAndClamp(0f, 1f, -820f, 820f) * Ext_ImageDrop.rotation;
			this.riggidBody.angularVelocity = num5;
			this.cachedImage = image;
			if (this.cachedImage != null)
			{
				this.updateSprite(true);
			}
			this.edgeBounce = new TwitchDropEdgeBounce();
			this.ReloadParameters();
			this.lifeTimeRandom = RandomHelper.RandomFloat01Inclusive() * 0.5f;
			float num6 = Ext_ImageDrop.lifeTime + this.lifeTimeRandom;
			this.spriteRenderer.material = (startWithSmoothBorder ? this.DropMaterial_Smooth : this.DropMaterial_Default);
			Invoke("startFadeout", num6);
			Ext_ImageDrop.totalOnScreen++;
		}

		private void Update()
		{
			this.rawTime += Time.deltaTime;
			if (this.transform.localPosition.y < -200f)
			{
				Destroy(base.gameObject);
			}
			else
			{
				this.updateSprite(false);
				this.bounce();
			}
			if (this.rotationLocked)
			{
				base.transform.rotation = Quaternion.identity;
			}
			if (Ext_ImageDrop.updateMaterial)
			{
				Ext_ImageDrop.updateMaterial = false;
				this.spriteRenderer.material = (Ext_ImageDrop.smoothBorder ? this.DropMaterial_Smooth : this.DropMaterial_Default);
			}
		}

		private void bounce()
		{
			if (this.dead)
			{
				return;
			}
			Vector2 velocity = this.riggidBody.velocity;
			float orthographicSize = this.CameraRenderer.orthographicSize;
			float num = orthographicSize * this.CameraRenderer.aspect;
			if (base.transform.localPosition.x > num && velocity.x > 0f)
			{
				if (this.edgeBounce.Bounce_Right())
				{
					velocity.x = -velocity.x * Ext_ImageDrop.bounciness;
					this.riggidBody.velocity = velocity;
				}
			}
			else if (base.transform.localPosition.x < -num && velocity.x <= 0f && this.edgeBounce.Bounce_Left())
			{
				velocity.x = -velocity.x * Ext_ImageDrop.bounciness;
				this.riggidBody.velocity = velocity;
			}
			if (base.transform.localPosition.y >= -(4f + orthographicSize * 2f) || velocity.y > 0f)
			{
				if (base.transform.localPosition.y > -5f && velocity.y > 0f && this.rawTime > 0.2f && this.edgeBounce.Bounce_Top())
				{
					velocity.y = -velocity.y * Ext_ImageDrop.bounciness;
					this.riggidBody.velocity = velocity;
				}
				return;
			}
			if (this.edgeBounce.Bounce_Bottom())
			{
				velocity.y = -velocity.y * Ext_ImageDrop.bounciness;
				this.riggidBody.velocity = velocity;
				return;
			}
			this.dead = true;
			this.circleCollider.enabled = false;
			this.boxCollider.enabled = false;
		}

		private void updateSprite(bool forceFirstFrame = false)
		{
			if (Ext_ImageDrop.animate)
			{
				this.time += Time.deltaTime * Ext_ImageDrop.animationSpeed;
				this.spriteRenderer.sprite = this.cachedImage.GetForTime(this.time);
				return;
			}
			if (forceFirstFrame)
			{
				this.spriteRenderer.sprite = this.cachedImage.GetForTime(0f);
			}
		}

		public void Remove()
		{
			if (!this.isTemplate)
			{
				Destroy(base.gameObject);
			}
		}

		private void LateUpdate()
		{
			if (Ext_ImageDrop.opacity <= 0.99f)
			{
				Color color = this.spriteRenderer.color;
				color.a *= Ext_ImageDrop.opacity;
				this.spriteRenderer.color = color;
			}
		}

		private void OnDestroy()
		{
			Ext_ImageDrop.totalOnScreen--;
		}

		private void startFadeout()
		{
			this.dead = true;
			this.circleCollider.enabled = false;
			this.boxCollider.enabled = false;
			//this.EmoteAnimator.SetBool("alive", false);
		}

		public void ReloadParameters()
		{
			PhysicsMaterial2D physicsMaterial2D = new PhysicsMaterial2D("NewBouncyMaterial");
			physicsMaterial2D.bounciness = Ext_ImageDrop.bounciness;
			this.riggidBody.sharedMaterial = physicsMaterial2D;
			this.riggidBody.gravityScale = this.dropPysicsMultiplierValue * Ext_ImageDrop.gravityMultiplier;
			base.transform.localScale = new Vector3(Ext_ImageDrop.size, Ext_ImageDrop.size, 1f);
			this.riggidBody.angularDrag = Ext_ImageDrop.rotation.MapAndClamp(1f, 0f, 0f, 5f);
			this.rotationLocked = false;
		}

		public void ReloadLifeTime()
		{
			float num = Ext_ImageDrop.lifeTime + this.lifeTimeRandom;
			if (num < this.rawTime)
			{
				this.startFadeout();
				return;
			}
			(num - this.rawTime).ClampBetween(0.1f, 60f);
			base.CancelInvoke("startFadeout");
			base.Invoke("startFadeout", num);
		}

		public static void SetBounciness(float newBounciness)
		{
			Ext_ImageDrop.bounciness = newBounciness.MapAndClamp(0f, 1f, 0f, 0.95f);
		}

		public static void SetSize(float newSize)
		{
			Ext_ImageDrop.size = newSize.ClampBetween(0.5f, 3f);
		}

		public static void SetRotation(float newRotation)
		{
			Ext_ImageDrop.rotation = newRotation.Clamp01();
		}

		public static void SetAnimationSpeed(float newAnimationSpeed)
		{
			Ext_ImageDrop.animationSpeed = newAnimationSpeed;
			Ext_ImageDrop.animate = Ext_ImageDrop.animationSpeed > 0.01f;
		}

		// Token: 0x06001EBB RID: 7867 RVA: 0x000169EF File Offset: 0x00014BEF
		public static void SetLifeTime(float newLifeTime)
		{
			Ext_ImageDrop.lifeTime = newLifeTime;
		}

		// Token: 0x06001EBC RID: 7868 RVA: 0x000169F7 File Offset: 0x00014BF7
		public static void SetOpacity(float newOpacity)
		{
			Ext_ImageDrop.opacity = newOpacity.ClampBetween(0.1f, 1f);
		}

		// Token: 0x06001EBD RID: 7869 RVA: 0x00016A0E File Offset: 0x00014C0E
		public static void SetInitialSpeed(float newInitialSpeed)
		{
			Ext_ImageDrop.initialSpeed = newInitialSpeed.ClampBetween(0f, 1f);
		}

		// Token: 0x06001EBE RID: 7870 RVA: 0x00016A25 File Offset: 0x00014C25
		public static void SetGravityMultiplier(float newGravityMultiplier)
		{
			Ext_ImageDrop.gravityMultiplier = newGravityMultiplier;
		}

		// Token: 0x06001EBF RID: 7871 RVA: 0x00016A2D File Offset: 0x00014C2D
		public static void SetSmoothBorder(bool useSmoothBorderMaterial)
		{
			Ext_ImageDrop.smoothBorder = useSmoothBorderMaterial;
			Ext_ImageDrop.updateMaterial = true;
		}
	}
}
