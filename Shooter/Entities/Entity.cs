using OpenTK.Mathematics;
using Shooter.Render;
using Shooter.Render.Quads;

namespace Shooter.Entities;

public class Entity
{
    private static readonly Quad Quad = new UvQuad();
    public Matrix4 ModelMatrix { get; private set; }
    public Quad Model { get; }

    private Vector2 _pos = (0, 0);
    private float _rot = 0;
    private Vector2 _scale = (1, 1);

    public Vector2 Pos
    {
        get => this._pos;
        set
        {
            this._pos = value;
            this.RecalculateMatrix();
        }
    }

    public float Rotation
    {
        get => this._rot;
        set
        {
            this._rot = value;
            this.RecalculateMatrix();
        }
    }

    public Vector2 Scale
    {
        get => this._scale;
        set
        {
            this._scale = value;
            this.RecalculateMatrix();
        }
    }

    public float ScaleY
    {
        get => this._scale.Y;
        set
        {
            this._scale.Y = value;
            this.RecalculateMatrix();
        }
    }

    public Entity(Quad? model = null)
    {
        model ??= Entity.Quad;
        this.Model = model;
        this.ModelMatrix = Matrix4.Identity;
    }

    public Entity At(int x, int y)
    {
        this.Pos = (x, y);
        return this;
    }

    public Entity Rotated(float degrees)
    {
        this.Rotation = degrees;
        return this;
    }

    public Entity WithScale(float xScale, float yScale)
    {
        this.Scale = (xScale, yScale);
        return this;
    }

    private void RecalculateMatrix()
    {
        Matrix4 mat = Matrix4.Identity;
        mat *= Matrix4.CreateScale(this.Scale.X, this.Scale.Y, 1);
        mat *= Matrix4.CreateRotationZ(this.Rotation * 0.01745329f);
        mat *= Matrix4.CreateTranslation(this.Pos.X, this.Pos.Y, 0);

        this.ModelMatrix = mat;
    }
}