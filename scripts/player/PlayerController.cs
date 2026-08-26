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
}

public partial class PlayerController : Unit
{
    
    [Signal] public delegate void SoulChangedEventHandler(int newSoulIndex);
    [Signal] public delegate void WeaponChangedEventHandler(int newWeaponIndex);
    [Signal] public delegate void ItemAddedEventHandler(string itemName);
    [Signal] public delegate void SoulAddedEventHandler(Soul soul, int index);
    [Signal] public delegate void WeaponAddedEventHandler(WeaponResource weaponResource, int index);

    [Export] private Camera2D _camera;
    [Export] public Node2D HandNode;
    
    private bool _inBattle = false;
    
    // Используем List вместо массивов, так как мы будем добавлять предметы по ходу игры
    private List<Soul> _souls = new(); // Пока string как плейсхолдер для класса Soul
    private List<WeaponResource> _weapons = new(); // Плейсхолдер для класса Weapon

    public int CurrentSoulIndex = 0;
    public int CurrentWeaponIndex = 0;
    
    private float _shakeStrength = 0f;
    private readonly float _shakeDecayRate = 5f;
    private readonly Vector2 _normalZoom = new Vector2(2f, 2f);
    private readonly Vector2 _battleZoom = new Vector2(1.8f, 1.8f); 
    private RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        base._Ready();
        _rng.Randomize();
        Hp = Stats?.BaseHp ?? 100f;

        CallDeferred(MethodName.UISetup);

    }
    
    void UISetup() => Game.Instance.UI.PlayerUI.SetPlayer(this);

    public override void _PhysicsProcess(double delta)
    {
        HandleMovement();
        HandleSelectionInputs();
        if (Input.IsActionPressed("lm"))
            HandleAttack();
    }

    public override void _Process(double delta)
    {
        HandleCameraEffects((float)delta);
    }

    private void HandleMovement()
    {
        Vector2 direction = Input.GetVector("a", "d", "w", "s");
        
        Velocity = direction * (Stats?.BaseSpeed ?? 300f);
        MoveAndSlide();
    }

    private void HandleSelectionInputs()
    {
        if (Input.IsActionJustPressed("mwu")) SwitchSoul(1);
        if (Input.IsActionJustPressed("mwd")) SwitchSoul(-1);
        
        if (Input.IsActionJustPressed("q")) SwitchWeapon(1);
        if (Input.IsActionJustPressed("e")) SwitchWeapon(-1);
    }

    void HandleAttack()
    {
        var angle = ( GetGlobalMousePosition() -GlobalPosition).Angle();
        
        if (Weapon.Resource == null) return;
            Weapon.ExecuteShoot(angle, this);
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
        EmitSignal(Unit.SignalName.HealthChanged, Hp, Stats?.BaseHp ?? 100f);

        if (Hp <= 0)
        {
            ExecuteDie();
        }
    }

    public void Heal(float amount)
    {
        Hp = Mathf.Min(Hp + amount, Stats?.BaseHp ?? 100f);
        EmitSignal(SignalName.Healed, amount);
        EmitSignal(SignalName.HealthChanged, Hp, Stats?.BaseHp ?? 100f);
    }

    public override void ExecuteDie()
    {
        Game.Instance.QueueFree();
        GetTree().Root.AddChild(GD.Load<PackedScene>("res://scenes/game.tscn").Instantiate<Game>());   
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
        _souls.Add(soul);
        if  (_souls.Count == 1)
            
        EmitSignal(SignalName.ItemAdded, soul);
    }

    private void SwitchSoul(int direction)
    {
        if (_souls.Count == 0) return;
        
        CurrentSoulIndex = (CurrentSoulIndex + direction % _souls.Count + _souls.Count) % _souls.Count;
        EmitSignal(SignalName.SoulChanged, CurrentSoulIndex);
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