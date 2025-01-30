// using OpenTK.Mathematics;
//
// namespace Shooter.Render;
//
// public class Camera
// {
//     public Matrix4 Projection { get; } =
//         Matrix4.CreateOrthographicOffCenter(0, ShooterGameWindow.PIXELS_X, 0, ShooterGameWindow.PIXELS_Y, -1, 1);
//
//     public Matrix4 View { get; } = Matrix4.LookAt(0, 0, 0, 0, 0, -1, 0, 1, 0);
// }