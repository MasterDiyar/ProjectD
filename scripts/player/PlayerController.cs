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
    [Signal] public delegate void HealthChangedEventHandler(float currentHp, float maxHp);
    [Signal] public delegate void DamageTakenEventHandler(float amount, int elementType);
    [Signal] public delegate void HealedEventHandler(float amount);
    [Signal] public delegate void SoulChangedEventHandler(int newSoulIndex);
    [Signal] public delegate void WeaponChangedEventHandler(int newWeaponIndex);
    [Signal] public delegate void ItemAddedEventHandler(string itemName);

    [Export] private Camera2D _camera;
    [Export] public Node2D HandNode;
    [Export] public Weapon Weapon;
    
    private float _currentHp;
    private bool _inBattle = false;
    
    // Используем List вместо массивов, так как мы будем добавлять предметы по ходу игры
    private List<string> _souls = new(); // Пока string как плейсхолдер для класса Soul
    private List<WeaponResource> _weapons = new(); // Плейсхолдер для класса Weapon
    
    private int _currentSoulIndex = 0;
    private int _currentWeaponIndex = 0;
    
    private float _shakeStrength = 0f;
    private readonly float _shakeDecayRate = 5f;
    private readonly Vector2 _normalZoom = new Vector2(2f, 2f);
    private readonly Vector2 _battleZoom = new Vector2(1.8f, 1.8f); 
    private RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        base._Ready();
        _rng.Randomize();
        _currentHp = Stats?.BaseHp ?? 100f; 
        
        AddSoul("Пустая кукла");
    }

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

    public override void TakeDamage(HitData hit)
    {
        float armorMultiplier = 100f / (100f + (Stats?.BaseArmor ?? 0f));
        float finalDamage = hit.Damage * armorMultiplier;

        _currentHp -= finalDamage;
        
        ApplyCameraShake(finalDamage * 0.5f);

        EmitSignal(SignalName.DamageTaken, finalDamage, (int)hit.Element);
        EmitSignal(SignalName.HealthChanged, _currentHp, Stats?.BaseHp ?? 100f);

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        _currentHp = Mathf.Min(_currentHp + amount, Stats?.BaseHp ?? 100f);
        EmitSignal(SignalName.Healed, amount);
        EmitSignal(SignalName.HealthChanged, _currentHp, Stats?.BaseHp ?? 100f);
    }

    private void Die()
    {
        GD.Print("Кукла сломана!");
    }

    public void AddWeapon(WeaponResource weapon)
    {
        _weapons.Add(weapon);
        EmitSignal(SignalName.ItemAdded, weapon);
        if (_weapons.Count == 1)
            Weapon.ResourceLoad(_weapons[0]);
    }

    public void AddSoul(string soul)
    {
        _souls.Add(soul);
        EmitSignal(SignalName.ItemAdded, soul);
    }

    private void SwitchSoul(int direction)
    {
        if (_souls.Count == 0) return;
        
        _currentSoulIndex = (_currentSoulIndex + direction % _souls.Count + _souls.Count) % _souls.Count;
        EmitSignal(SignalName.SoulChanged, _currentSoulIndex);
    }

    private void SwitchWeapon(int direction)
    {
        if (_weapons.Count == 0) return;
        
        _currentWeaponIndex = (_currentWeaponIndex + direction % _weapons.Count + _weapons.Count) % _weapons.Count;
        Weapon.ResourceLoad(_weapons[_currentWeaponIndex]);
        EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex);
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