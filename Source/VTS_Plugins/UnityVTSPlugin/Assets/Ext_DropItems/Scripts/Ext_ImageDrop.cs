using Assets.ExtendedDropImages.Messages;
using System;
using UnityEngine;

namespace Assets.Ext_DropItems.Scripts
{
	public class Ext_ImageDrop : MonoBehaviour
	{
		public class Ext_ImageDrop_Bounce
		{
			public const int TOP_DEFAULT = 0;
			public const int BOTTOM_DEFAULT = 2;
			public const int LEFT_DEFAULT = 20;
			public const int RIGHT_DEFAULT = 20;

			private readonly int bounceTopCount_Allowed = 0;
			private readonly int bounceBottomCount_Allowed = 2;
			private readonly int bounceLeftCount_Allowed = 20;
			private readonly int bounceRightCount_Allowed = 20;

			private int BounceTopCount_Done = 0;
			private int BounceBottomCount_Done = 0;
			private int BounceLeftCount_Done = 0;
			private int BounceRightCount_Done = 0;

			public Ext_ImageDrop_Bounce(ExtendedDropItemDefinition definition)
			{
				this.bounceLeftCount_Allowed = definition.leftEdgeBounce;
				this.bounceBottomCount_Allowed = definition.bottomEdgeBounce;
				this.bounceRightCount_Allowed = definition.rightEdgeBounce;
				this.bounceTopCount_Allowed = definition.topEdgeBounce;
			}

			public bool Bounce_Top()
			{
				bool result = BounceTopCount_Done < bounceTopCount_Allowed;
				BounceTopCount_Done++;
				return result;
			}

			public bool Bounce_Bottom()
			{
				bool result = BounceBottomCount_Done < bounceBottomCount_Allowed;
				BounceBottomCount_Done++;
				return result;
			}

			public bool Bounce_Left()
			{
				bool result = BounceLeftCount_Done < bounceLeftCount_Allowed;
				BounceLeftCount_Done++;
				return result;
			}

			public bool Bounce_Right()
			{
				bool result = BounceRightCount_Done < bounceRightCount_Allowed;
				BounceRightCount_Done++;
				return result;
			}
		}
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
		private ExtendedDropItemDefinition definition;

		[SerializeField] private bool isTemplate = true;

		private bool m_Is_Dead;

		private CachedImageNormalOrAnimated cachedImage;

		private float m_time;

		private float m_rawTime;

		private float m_lifeTimeRandom;

		private bool rotationLocked;

		private Ext_ImageDrop_Bounce edgeBounce;

		public void Initialize(CachedImageNormalOrAnimated image, bool lockRotation, float modelOutlineX_min, float modelOutlineX_max, ExtendedDropItemDefinition definition)
		{
			this.CameraRenderer = VTSPluginExternals.Live2DCamera;
			this.isTemplate = false;
			this.rotationLocked = lockRotation;
			Vector3 localPosition = this.transform.localPosition;
			this.definition = definition;
			localPosition.x = RandomHelper.RandomFloatInclusive(modelOutlineX_min, modelOutlineX_max);
			localPosition.y += RandomHelper.RandomFloat01Inclusive() * 1E-06f;
			localPosition.z -= RandomHelper.RandomFloat01Inclusive();
			this.transform.localPosition = localPosition;
			this.dropPysicsMultiplierValue = this.dropPysicsMultiplier.RandomValue;
			this.riggidBody.gravityScale = this.dropPysicsMultiplierValue * definition.gravity;
			float finalVel1 = RandomHelper.RandomFloat01Inclusive().MapAndClamp(0f, 1f, -1f, 1f) * speedX_Max;
			float speed1 = Mathf.Lerp(speedMin_A, speedMax_A, definition.dropSpeed);
			float speed2 = Mathf.Lerp(speedMin_B, speedMax_B, definition.dropSpeed);
			float finalVel2 = RandomHelper.RandomFloatInclusive(speed1, speed2) * -1f;
			this.riggidBody.velocity = new Vector2(finalVel1, finalVel2);
			this.riggidBody.angularVelocity = RandomHelper.RandomFloat01Inclusive().MapAndClamp(0f, 1f, -initialAngularVelocity, initialAngularVelocity) * definition.rotation;
			this.cachedImage = image;
			if (this.cachedImage != null)
			{
				this.UpdateSprite(true);
			}
			this.edgeBounce = new Ext_ImageDrop_Bounce(definition);
			this.ReloadParameters();
			this.m_lifeTimeRandom = RandomHelper.RandomFloat01Inclusive() * 0.5f;
			float fadeoutDelay = definition.lifeTime + this.m_lifeTimeRandom;
			this.spriteRenderer.material = (definition.startWithSmoothBorder ? this.DropMaterial_Smooth : this.DropMaterial_Default);
			Invoke("startFadeout", fadeoutDelay);
			Ext_ImageDrop.totalOnScreen++;
			VTSPluginExternals.LogMessage("Initializing");
		}

		[EasyButtons.Button]
		private void EncapsulateAgain()
		{
			if (this.spriteRenderer.sprite == null)
				this.boxCollider.size = new Vector2(0.01f, 0.01f);
			else
			{
				this.boxCollider.size = new Vector2(this.spriteRenderer.sprite.textureRect.width, this.spriteRenderer.sprite.rect.height) / 100;
				var half = this.boxCollider.size / 2;
				this.circleCollider.radius = Mathf.Max(half.x, half.y);
			}
		}

		public void SetColliderSizesManually(Vector2 boxSize, float circleRadious)
		{
			this.boxCollider.size = boxSize;
			this.circleCollider.radius = circleRadious;
		}

		private void Update()
		{
			this.m_rawTime += Time.deltaTime;
			if (this.transform.localPosition.y < OFF_SCREEN_BOTTOM)
			{
				Destroy(base.gameObject);
			}
			else
			{
				this.UpdateSprite(false);
				this.Bounce();
			}
			if (this.rotationLocked)
				base.transform.rotation = Quaternion.identity;
			if (Ext_ImageDrop.updateMaterial)
			{
				Ext_ImageDrop.updateMaterial = false;
				this.spriteRenderer.material = (Ext_ImageDrop.smoothBorder ? this.DropMaterial_Smooth : this.DropMaterial_Default);
			}
		}

		private void Bounce()
		{
			if (this.m_Is_Dead)
				return;

			Vector2 velocity = this.riggidBody.velocity;
			float orthographicSize = this.CameraRenderer.orthographicSize;
			float num = orthographicSize * this.CameraRenderer.aspect;
			if (base.transform.localPosition.x > num && velocity.x > 0f)
			{
				if (this.edgeBounce.Bounce_Right())
				{
					velocity.x = -velocity.x * definition.bounciness;
					this.riggidBody.velocity = velocity;
				}
			}
			else if (base.transform.localPosition.x < -num && velocity.x <= 0f && this.edgeBounce.Bounce_Left())
			{
				velocity.x = -velocity.x * definition.bounciness;
				this.riggidBody.velocity = velocity;
			}
			if (base.transform.localPosition.y >= -(4f + orthographicSize * 2f) || velocity.y > 0f)
			{
				if (base.transform.localPosition.y > -5f && velocity.y > 0f && this.m_rawTime > 0.2f && this.edgeBounce.Bounce_Top())
				{
					velocity.y = -velocity.y * definition.bounciness;
					this.riggidBody.velocity = velocity;
				}
				return;
			}
			if (this.edgeBounce.Bounce_Bottom())
			{
				velocity.y = -velocity.y * definition.bounciness;
				this.riggidBody.velocity = velocity;
				return;
			}
			this.m_Is_Dead = true;
			this.circleCollider.enabled = false;
			this.boxCollider.enabled = false;
		}

		private void UpdateSprite(bool forceFirstFrame = false)
		{
			if (definition.animationSpeed > 0)
			{
				this.m_time += Time.deltaTime * definition.animationSpeed;
				this.spriteRenderer.sprite = this.cachedImage.GetForTime(this.m_time);
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
			if (definition.opacity <= 0.99f)
			{
				Color color = this.spriteRenderer.color;
				color.a *= definition.opacity;
				this.spriteRenderer.color = color;
			}
		}

		private void OnDestroy()
		{
			Ext_ImageDrop.totalOnScreen--;
			VTSPluginExternals.LogMessage("Deleting");
		}

		private void startFadeout()
		{
			this.m_Is_Dead = true;
			this.circleCollider.enabled = false;
			this.boxCollider.enabled = false;
			//this.EmoteAnimator.SetBool("alive", false);
		}

		public void ReloadParameters()
		{
			if (definition == null)
				definition = new ExtendedDropItemDefinition();

			PhysicsMaterial2D physicsMaterial2D = new PhysicsMaterial2D("NewBouncyMaterial");
			physicsMaterial2D.bounciness = definition.bounciness;
			this.riggidBody.sharedMaterial = physicsMaterial2D;
			this.riggidBody.gravityScale = this.dropPysicsMultiplierValue * definition.gravity;
			this.transform.localScale = new Vector3(definition.sizeScale, definition.sizeScale, 1f);
			this.riggidBody.angularDrag = definition.rotation.MapAndClamp(1f, 0f, 0f, 5f);
			this.rotationLocked = false;
		}

		public void ReloadLifeTime()
		{
			float num = definition.lifeTime + this.m_lifeTimeRandom;
			if (num < this.m_rawTime)
			{
				this.startFadeout();
				return;
			}
			(num - this.m_rawTime).ClampBetween(0.1f, 60f);
			this.CancelInvoke("startFadeout");
			this.Invoke("startFadeout", num);
		}
	}
}
