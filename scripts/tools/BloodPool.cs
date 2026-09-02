using Godot;

public partial class BloodPool : Node2D
{
    public static readonly Rect2
       BloodRect       = new Rect2(0,0,32,32), 
       YellowBloodRect = new Rect2(32,0,32,32),
       GreenBloodRect  = new Rect2(64,0,32,32),
       BlackBloodRect  = new Rect2(96,0,32,32);

    [Export] public int Size = 100;
    [Export] public Texture2D BloodTexture, MiniBloodTexture;
    
    public Sprite2D[] BloodSprites; 
    public float[] Time;
    public int CurrentTicket = 0; 
    
    public Sprite2D[] MiniSprites;
    public float[] MiniTime;
    public int CurrentMiniTicket = 0;
    
    RandomNumberGenerator _rng = new RandomNumberGenerator();
    
    private const float LifeTime = 20f;
    private const float FadeTime = 3f;
    private const float GrowTime = 0.5f;
    
    private const float MiniLifeTime = 4f; // Жизнь мини-брызг
    private const float MiniFadeTime = 1f; // Время затухания мини-брызг
    
    public override void _Ready()
    {
       Init();
    }

    void Init()
    {
       _rng.Randomize();
       BloodSprites = new Sprite2D[Size];
       Time = new float[Size];
       
       int miniPoolSize = Size * 5;
       MiniSprites = new Sprite2D[miniPoolSize];
       MiniTime = new float[miniPoolSize];
       
       for (int i = 0; i < Size; i++) {
          var sprite = new Sprite2D() {
             Texture = BloodTexture,
             Visible = false,
             RegionEnabled = true,
             RegionRect = BloodRect,
             Offset = new Vector2(16, -16) 
          };
          BloodSprites[i] = sprite;
          AddChild(sprite);
          Time[i] = 0f;
       }
       
       for (int i = 0; i < miniPoolSize; i++) {
          var miniSprite = new Sprite2D() {
             Texture = MiniBloodTexture,
             Visible = false,
             RegionEnabled = true,
             RegionRect = BloodRect,
             Offset = new Vector2(16, -16) 
          };
          MiniSprites[i] = miniSprite;
          AddChild(miniSprite);
          MiniTime[i] = 0f;
       }
    }

    public void CreateTicket(Vector2 pos, Rect2 region, float rot)
    {
       var sprite = BloodSprites[CurrentTicket];
       sprite.Position = pos;
       sprite.Rotation = rot + Mathf.Pi/4;
       sprite.Visible = true;
       sprite.Modulate = Colors.White;
       sprite.RegionRect = new Rect2(region.Position + Vector2.Down * _rng.RandiRange(0, 3) * 32, region.Size);
       
       float startScale = 1f / 32f;
       sprite.Scale = new Vector2(startScale, startScale);
       
       Time[CurrentTicket] = LifeTime;
       CurrentTicket = (CurrentTicket + 1) % Size;
       
       int miniCount = _rng.RandiRange(2, 5); 
       
       for (int i = 0; i < miniCount; i++)
       {
           var mini = MiniSprites[CurrentMiniTicket];
           
           Vector2 randomOffset = new Vector2(_rng.RandfRange(-20f, 20f), _rng.RandfRange(-20f, 20f));
           mini.Position = pos + randomOffset;
           
           mini.Rotation = rot + _rng.RandfRange(-Mathf.Pi,  Mathf.Pi)/4 + Mathf.Pi/4; 
           mini.Visible = true;
           mini.Modulate = Colors.White;
           mini.RegionRect = new Rect2(region.Position + Vector2.Down * _rng.RandiRange(0, 3) * 32, region.Size);
           
           float miniScale = _rng.RandfRange(0.2f, 0.6f), maxiScale = _rng.RandfRange(0.2f, 0.6f);
           mini.Scale = new Vector2(miniScale, maxiScale);
           
           MiniTime[CurrentMiniTicket] = MiniLifeTime;
           CurrentMiniTicket = (CurrentMiniTicket + 1) % (Size * 5);
       }
    }

    public override void _Process(double delta)
    {
       var dt = (float)delta;
       for (var i = 0; i < Size; i++) {
          var blood = BloodSprites[i];
          if (!blood.Visible) continue;
          Time[i] -= dt;
          
          switch (Time[i]) {
             case > LifeTime - GrowTime: {
                float elapsed = LifeTime - Time[i];
                float t = elapsed / GrowTime;
                float currentScale = Mathf.Lerp(1f / 32f, 1f, Mathf.Sqrt(t));
                blood.Scale = new Vector2(currentScale, currentScale);
                break; }
             case > FadeTime: {
                if (blood.Scale.X != 1f) blood.Scale = Vector2.One;
                break; }
             case <= FadeTime and > 0f: {
                float alpha = Time[i] / FadeTime; 
                blood.Modulate = new Color(1, 1, 1, alpha);
                break; }
             case <= 0f:
                blood.Visible = false; break;
          }
       }
       
       for (var i = 0; i < Size * 5; i++) {
          var mini = MiniSprites[i];
          if (!mini.Visible) continue;
   
          MiniTime[i] -= dt;
          float elapsed = 4f - MiniTime[i];
   
          if (elapsed < 0.5f) {
             float t = elapsed / 0.5f; 
             float speed = 250f * Mathf.Pow(1f - t, 3); 
      
             mini.Position += Vector2.FromAngle(mini.Rotation - Mathf.Pi/4) * speed * dt;
          }
   
          if (MiniTime[i] <= MiniFadeTime && MiniTime[i] > 0f) {
             float alpha = MiniTime[i] / MiniFadeTime; 
             mini.Modulate = new Color(1, 1, 1, alpha);
          }else if (MiniTime[i] <= 0f)
             mini.Visible = false;
          
       }
    }
}