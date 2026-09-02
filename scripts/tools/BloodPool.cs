using Godot;

public partial class BloodPool : Node2D
{
    public static readonly Rect2
       BloodRect       = new Rect2(0,0,32,32), 
       YellowBloodRect = new Rect2(32,0,32,32),
       GreenBloodRect  = new Rect2(64,0,32,32),
       BlackBloodRect  = new Rect2(96,0,32,32);

    [Export] public int Size = 100;
    [Export] public Texture2D BloodTexture;
    
    public Sprite2D[] BloodSprites; 
    public float[] Time;
    
    public int CurrentTicket = 0; 
    RandomNumberGenerator _rng = new RandomNumberGenerator();
    
    private const float LifeTime = 20f;
    private const float FadeTime = 3f;
    private const float GrowTime = 0.5f;
    
    public override void _Ready()
    {
       Init();
    }

    void Init()
    {
       _rng.Randomize();
       BloodSprites = new Sprite2D[Size];
       Time = new float[Size];
       
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
    }

    public void CreateTicket(Vector2 pos,Rect2 region, float rot)
    {
       var sprite = BloodSprites[CurrentTicket];
       sprite.Position = pos;
       sprite.Rotation = rot;
       sprite.Visible = true;
       sprite.Modulate = Colors.White;
       sprite.RegionRect = new Rect2(region.Position + Vector2.Down * _rng.RandiRange(0, 3) * 32, region.Size);
       
       float startScale = 1f / 32f;
       sprite.Scale = new Vector2(startScale, startScale);
       
       Time[CurrentTicket] = LifeTime;
       CurrentTicket = (CurrentTicket + 1) % Size;
    }

    public override void _Process(double delta)
    {
       for (var i = 0; i < Size; i++) {
          var blood = BloodSprites[i];
          if (!blood.Visible) continue;

          Time[i] -= (float)delta;
          
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
    }
    
}