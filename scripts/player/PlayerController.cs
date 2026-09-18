using ProjectD.scripts.units;
using ProjectD.scripts.weapon;

namespace ProjectD.scripts.player;

using Godot;
using System.Collections.Generic;



public struct HitData 
{
    public float Damage;
    public ElementType Element;
    public WeaponType  Type;
    public float KnockbackForce;
    public float AttackAngle;
}

public partial class PlayerController : Unit
{
    
    [Signal] public delegate void SoulChangedEventHandler(int newSoulIndex);
    [Signal] public delegate void WeaponChangedEventHandler(int newWeaponIndex);
    [Signal] public delegate void ItemAddedEventHandler(string itemName);
    [Signal] public delegate void SoulAddedEventHandler(Soul soul, int index);
    [Signal] public delegate void WeaponAddedEventHandler(WeaponResource weaponResource, int index);
    [Signal] public delegate void AttackEventHandler(float angle, PlayerController player);

    [Export] private Camera2D _camera;
    [Export] public Node2D HandNode;
    [Export] public AnimatedSprite2D AnimSprite; //deprecased
    [Export] public AnimationPlayer Animator;
    [Export] public CpuParticles2D ChangeParticles;
    [Export] public Sprite2D Head, Body;
    [Export] public Marker2D LeftHand, RightHand;
    
    private bool _inBattle = false;
    
    
    
    private float _shakeStrength = 0f, _selectionCooldown = 0f;
    private readonly float _shakeDecayRate = 5f;
    private readonly Vector2 _normalZoom = new Vector2(2f, 2f);
    private readonly Vector2 _battleZoom = new Vector2(1.8f, 1.8f); 
    private RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        base._Ready();
        _rng.Randomize();

        CallDeferred(MethodName.UISetup);

        PlayerStatSaver.Instance.Player = this;
    }
    
    void UISetup() => Game.Instance.UI.PlayerUI.SetPlayer(this);

    public override void _PhysicsProcess(double delta)
    {
        if (_selectionCooldown > 0f)
            _selectionCooldown -= (float)delta;
        
        HandleMovement((float)delta);
        HandleSelectionInputs();
        if (Input.IsActionPressed("lm"))
            HandleAttack();
    }

    public override void _Process(double delta)
    {
        HandleCameraEffects((float)delta);
    }

    private float sin = 0;
    private void HandleMovement(float dt)
    {
        Vector2 direction = Input.GetVector("a", "d", "w", "s");
        Animator.Play((direction.Length() < 0.1f) ? "RESET":"move");
        if (direction.X < 0f) {
            Body.Scale = new Vector2(-1, 1);
            Head.Scale = new Vector2(-1, 1);
        }else {
            Body.Scale = Vector2.One;
            Head.Scale = Vector2.One;
        }
        Velocity = direction * Speed;
        MoveAndSlide();
        
        //AnimSprite.Play((direction.Length() < 0.1f) ? "idle":"move");
        //AnimSprite.FlipH = direction.X < 0;
    }

    private void HandleSelectionInputs()
    {
        if (_selectionCooldown > 0f) return;
        
        if (Input.IsActionJustPressed("mwu")) SwitchSoul(1);
        else if (Input.IsActionJustPressed("mwd")) SwitchSoul(-1);
        else if (Input.IsActionJustPressed("q")) SwitchWeapon(1);
        else if (Input.IsActionJustPressed("e")) SwitchWeapon(-1);
        else return; 
        _selectionCooldown = 1.5f;
    }

    void HandleAttack()
    {
        var angle = ( GetGlobalMousePosition() -GlobalPosition).Angle();
        
        if (Weapon.Resource == null) return;
            Weapon.ExecuteShoot(angle, this);
            //Weapon.Scale = new Vector2(AnimSprite.FlipH ? 1 : -1, 1);
            
    }

    public override void TakeDamage(HitData data)
    {
        var damage = data.Damage;
        if (Armor <= 0) {
            HittedElement = ElementShaker.Shake(HittedElement, data.Element);
            damage *= Elements.GetElement(data.Element).DamageModifier;
            damage *= WeaponTypeNums.GetModifier(UnitWeapon, data.Element);
            Hp -= damage;
            if (Hp <= 0) ExecuteDie();
        }else {
            Armor -= damage;
        }
        
        ApplyCameraShake(damage * 0.5f);

        EmitSignal(Unit.SignalName.DamageTaken, damage, (int)data.Element);
        EmitSignal(Unit.SignalName.HealthChanged, Hp, MaxHp);

        if (Hp <= 0)
            ExecuteDie();
        
    }

    public void Heal(float amount)
    {
        Hp = Mathf.Min(Hp + amount, Stats?.BaseHp ?? 100f);
        EmitSignal(SignalName.Healed, amount);
        EmitSignal(SignalName.HealthChanged, Hp, Stats?.BaseHp ?? 100f);
    }

    public override void ExecuteDie()
    {
        Heal(125);
    }

    public void AddWeapon(WeaponResource weapon)
    {
        _weapons.Add(weapon);
        if (_weapons.Count == 1)
            Weapon.ResourceLoad(_weapons[0]);
        EmitSignal(SignalName.ItemAdded, weapon);
    }

    public void AddSoul(Soul soul)
    {
        if (_souls.Count < 7) {
            _souls.Add(soul);
            EmitSignal(SignalName.SoulAdded, soul, _souls.Count-1);
        }else {
            _souls[CurrentSoulIndex] = soul;
            EmitSignal(SignalName.SoulAdded, soul, CurrentSoulIndex);
        }
        if (_souls.Count == 1)
            SetSoul(CurrentSoulIndex);
        EmitSignal(SignalName.SoulAdded, soul, CurrentSoulIndex);
    }

    private void SwitchSoul(int direction)
    {
        if (_souls.Count == 0) return;
        
        CurrentSoulIndex = (CurrentSoulIndex + direction % _souls.Count + _souls.Count) % _souls.Count;
        
        SetSoul(CurrentSoulIndex);
        
        EmitSignal(SignalName.SoulChanged, CurrentSoulIndex);
    }

    

    public void SetSoul(int index) {
        GD.Print(_souls[index].SoulName," added");
        var shader = (ShaderMaterial)Head.Material.Duplicate();
        Head.Material = shader;
        AddSoulStats(_souls[index]);
        var element = Elements.GetElement(UnitElement); 
        var gradient = new Gradient();
        gradient.Offsets = [0.0f, 0.5f, 1.0f];
        gradient.Colors = [element.ElementColor, element.SecondColor, element.ThirdColor];

        var gradientTexture = new GradientTexture1D();
        gradientTexture.Gradient = gradient;

        shader.SetShaderParameter("use_gradient", true);
        shader.SetShaderParameter("gradient_texture", gradientTexture);
        
        ChangeParticles.ColorRamp = gradient;
        ChangeParticles.Emitting = true;
    }
    

    private void SwitchWeapon(int direction)
    {
        if (_weapons.Count == 0) return;
        
        CurrentWeaponIndex = (CurrentWeaponIndex + direction % _weapons.Count + _weapons.Count) % _weapons.Count;
        Weapon.ResourceLoad(_weapons[CurrentWeaponIndex]);
        EmitSignal(SignalName.WeaponChanged, CurrentWeaponIndex);
    }

    public void SetBattleState(bool inBattle)
    {
        _inBattle = inBattle;
    }

    public void ApplyCameraShake(float strength)
    {
        _shakeStrength = Mathf.Max(_shakeStrength, strength);
    }

    private void HandleCameraEffects(float delta)
    {
        if (_camera == null) return;

        Vector2 targetZoom = _inBattle ? _battleZoom : _normalZoom;
        _camera.Zoom = _camera.Zoom.Lerp(targetZoom, delta * 3f);

        if (_shakeStrength > 0)
        {
            float xOffset = _rng.RandfRange(-1f, 1f) * _shakeStrength;
            float yOffset = _rng.RandfRange(-1f, 1f) * _shakeStrength;
            _camera.Offset = new Vector2(xOffset, yOffset);

            _shakeStrength = Mathf.Lerp(_shakeStrength, 0f, delta * _shakeDecayRate);
        }
        else
        {
            _camera.Offset = Vector2.Zero;
        }
    }
}