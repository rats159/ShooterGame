using System.Security.Cryptography.X509Certificates;
using OpenTK.Graphics.Vulkan;
using OpenTK.Mathematics;
using Shooter.Physics;

namespace Shooter.ECS.Components;

// Separate from the Transform, but should generally be synced before rendering
public class VerletObject : IComponent
{
    public VerletPoint v0;
    public VerletPoint v1;
    public VerletPoint v2;
    public VerletPoint v3;

    public Vector2 AverageVelocity()
    {
        Vector2 vel0 = this.v0.position.Value - this.v0.prevPosition.Value;
        Vector2 vel1 = this.v1.position.Value - this.v1.prevPosition.Value;
        Vector2 vel2 = this.v2.position.Value - this.v2.prevPosition.Value;
        Vector2 vel3 = this.v3.position.Value - this.v3.prevPosition.Value;

        return (vel0 + vel1 + vel2 + vel3) / 4;
    }
    
    public readonly bool isStatic;
    
    public Vector2 Center => (this.v0.position.Value + this.v1.position.Value + this.v2.position.Value + this.v3.position.Value) / 4;

    public readonly EdgeConstraint[] edges;

    public readonly VerletPoint[] points;

    public VerletObject(float x, float y,float width, float height, bool isStatic = false)
    {
        this.v0 = new(x,y,this);
        this.v1 = new(x+width,y,this);
        this.v2 = new(x+width,y+height,this);
        this.v3 = new(x,y+height,this);
        this.points = [this.v0, this.v1, this.v2, this.v3];
        this.edges = [
            new(this, this.v0.position, this.v1.position),
            new(this, this.v1.position, this.v2.position),
            new(this, this.v2.position, this.v3.position),
            new(this, this.v3.position, this.v0.position),
            new(this, this.v0.position, this.v2.position),
        ];
        this.isStatic = isStatic;

    }
}