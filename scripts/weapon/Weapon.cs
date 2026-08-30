using System.Threading.Tasks;
using Godot;
using ProjectD.scripts.player;
using ProjectD.scripts.units;

namespace ProjectD.scripts.weapon;

public partial class Weapon : Node2D
{
    [Export] public Timer _betweenShoot;
    [Export] public WeaponResource Resource;
    [Export] public WeaponType Type;
    [Export] public AnimationPlayer Animator;
    [Export] public Sprite2D[] SpriteSlots;
    
    PackedScene _bulletScene;
    
    private Vector2 _defaultPosition;
    private Tween _punchTween;
    
    bool _isShooted = false;
    public bool CanShoot => !_isShooted;

    public override void _Ready()
    {
        _defaultPosition = Position;
        _bulletScene = GD.Load<PackedScene>("res://scenes/bullet/bullet.tscn");
        _betweenShoot.Timeout += BetweenShootOnTimeout;

        Animator.AnimationFinished += AttackEnded;
        
        if (Resource != null) ResourceLoad(Resource);
    }
    void BetweenShootOnTimeout()
    {
        _isShooted = false;
        _betweenShoot.Stop();
    }

    public void ResourceLoad(WeaponResource resource)
    {
        Resource = resource;
        _betweenShoot.WaitTime = Resource.AttackSpeed;
        
        foreach (var slot in SpriteSlots) {
            slot.Texture = null;
            slot.Visible = false;
        }
        for (int i = 0; i < Resource.Texture.Length; i++) {
            GD.Print(i >= SpriteSlots.Length);
            if (i >= SpriteSlots.Length) break;
            SpriteSlots[i].Texture = Resource.Texture[i];
            SpriteSlots[i].Visible = true;
        }

        bool a = (Animator.HasAnimationLibrary(""));
        AnimationLibrary lib = a ? Animator.GetAnimationLibrary("") : new AnimationLibrary();
        if (!a) Animator.AddAnimationLibrary("", lib);
        
        if (lib.HasAnimation("onAttack"))
            lib.RemoveAnimation("onAttack");
        lib.AddAnimation("onAttack", Resource.AttackAnimation);
        if (lib.HasAnimation("onIdle"))
            lib.RemoveAnimation("onIdle");
        lib.AddAnimation("onIdle", Resource.IdleAnimation);
        Animator.Play("onIdle");
        _defaultPosition = Position;
        ZIndex = resource.ZIndex;
    }
    public async Task ExecuteShoot(float angle, Unit unit)
    {
        if (_isShooted) return;
        _isShooted = true;
        _betweenShoot.Start();
        Animator.Play("onAttack");

        Rotation = angle;
        TweenAnimation();

        await SpawnBullet(angle, unit);

    }

    public async Task SpawnBullet(float angle, Unit unit)
    {
        if (Resource.SpawnTimeOffset > 0) {
            await ToSignal(GetTree().CreateTimer(Resource.SpawnTimeOffset, false), SceneTreeTimer.SignalName.Timeout);
            if (!IsInstanceValid(this) || !IsInstanceValid(unit) || !IsInstanceValid(unit.GetParent())) 
                return;
        }
        
        for (int i = 0; i < Resource.BulletCount; i++)
        {
            Bullet bullet = _bulletScene.Instantiate<Bullet>();
            bullet.Rotation = Resource.OffsetAngle + angle + i * Resource.BetweenAngle;
            bullet.GlobalPosition = unit.GlobalPosition + Vector2.FromAngle(bullet.Rotation) * Resource.OffsetSpawn;
            bullet.Mother = unit;
            bullet.WeaponType = Type;
            bullet += Resource.Bullet;
            bullet.GlobalScale = Vector2.One * Resource.BulletScale;
            
            Game.Instance.SpawnNode(bullet);
        }
    }

    void TweenAnimation()
    {
        _punchTween?.Kill();
        _punchTween = CreateTween();
        
        Vector2 punchTarget = _defaultPosition + new Vector2(Resource.OffsetSpawn / 2f, 0);
        
        _punchTween.TweenProperty(this, "position", punchTarget, 0.1f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);
            
        float returnTime = Mathf.Max(0.1f, Resource.AttackSpeed - 0.1f);
        _punchTween.TweenProperty(this, "position", _defaultPosition, returnTime)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);
        _punchTween.TweenProperty(this, "rotation", 0, 0.01f);
    }

    void AttackEnded(StringName name)
    {
        if (!Animator.HasAnimation("onIdle") || !Animator.HasAnimation("onAttack")) return;
        if (name == "onAttack") Animator.Play("onIdle");

    }
    
}