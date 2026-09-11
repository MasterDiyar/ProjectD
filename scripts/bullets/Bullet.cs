using Godot;
using System;
using ProjectD.scripts.player;
using ProjectD.scripts.units;
using ProjectD.scripts.weapon;

public partial class Bullet : Area2D, IHasUnit
{
	[Export] public BulletResource BulletRes;
	[Export] public Sprite2D BulletSprite;
	public float Damage;
	public float Spread;
	public float Speed;
	public float Lifetime;
	public float PierceCount = 1;
	public float KnockbackStrength;
	[Export]public Unit unit { get; set; }
	public WeaponType WeaponType = WeaponType.Melee;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		if (BulletRes.Texture.Length > 0) 
			BulletSprite.Texture = BulletRes.Texture[0];
		if (BulletSprite != null)
			ApplyElementColors(BulletSprite, Elements.GetElement(unit.UnitElement));
		
		if (BulletRes != null)
			SetRes(BulletRes);
			
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not Unit player || player == unit) return;
		
			

		PierceCount--;
		if (PierceCount == 0)
			SetDeferred("monitoring", false);
		player.TakeDamage(new HitData {Damage = Damage, Element = BulletRes.ElementType, KnockbackForce = KnockbackStrength, Type = WeaponType, AttackAngle = GlobalRotation});
	}

	public void SetRes(BulletResource bulletRes)
	{
		BulletRes =(BulletResource) bulletRes.Duplicate();
		Damage = bulletRes.Damage;
		Spread = bulletRes.Spread;
		Lifetime = bulletRes.Lifetime;
		PierceCount = bulletRes.PierceCount;
		Speed = bulletRes.Speed;

		if (GetNodeOrNull<CollisionShape2D>("CollisionShape2D")?.Shape is CircleShape2D circleShape)
		{
			circleShape = (CircleShape2D)circleShape.Duplicate();
			circleShape.Radius = bulletRes.CollisionRadius;
			GetNode<CollisionShape2D>("CollisionShape2D").Shape = circleShape;
		}
	}

	public static Bullet operator +(Bullet a, BulletResource b)
	{
		a.SetRes(b);
		return a;
	}

	public override void _Process(double delta)
	{
		float d = (float)delta;
		Lifetime -= d;
		if (Lifetime <= 0) {
			QueueFree();
			return; 
		}

		if (BulletRes.Texture is { Length: > 0 })
		{
			float elapsedTime = BulletRes.Lifetime - Lifetime;
        
			int frameIndex = (int)(elapsedTime * BulletRes.AnimationSpeed) % BulletRes.Texture.Length;
			BulletSprite.Texture = BulletRes.Texture[frameIndex];
		}
		Position += d * Speed * Vector2.FromAngle(Rotation);
	}
	
	public void ApplyElementColors(Sprite2D sprite, Element element)
	{
		ShaderMaterial mat = sprite.Material as ShaderMaterial;
		if (mat == null) return;
    
		ShaderMaterial uniqueMat = (ShaderMaterial)mat.Duplicate();
		sprite.Material = uniqueMat;

		uniqueMat.SetShaderParameter("color_for_black", element.ElementColor);
		uniqueMat.SetShaderParameter("color_for_gray", element.SecondColor);
		uniqueMat.SetShaderParameter("color_for_white", element.ThirdColor);
	}
	
}
