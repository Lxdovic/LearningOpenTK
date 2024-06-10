using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LearningOpenTK;

internal sealed class Camera(float width, float height, Vector3 position) {
    private const float Sensitivity = 50f;
    private const float Speed = 16f;
    private bool _firstMove = true;
    private Vector3 _front = -Vector3.UnitZ;
    private float _pitch;

    private Vector3 _right = Vector3.UnitX;
    private Vector3 _up = Vector3.UnitY;
    private float _yaw = -90f;

    private Vector2 LastPos { get; set; }

    private void UpdateVectors() {
        if (_pitch > 89.0f)
            _pitch = 89.0f;

        if (_pitch < -89.0f)
            _pitch = -89.0f;

        _front.X = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Cos(MathHelper.DegreesToRadians(_yaw));
        _front.Y = MathF.Sin(MathHelper.DegreesToRadians(_pitch));
        _front.Z = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Sin(MathHelper.DegreesToRadians(_yaw));

        _front = Vector3.Normalize(_front);
        _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
    }

    public Matrix4 GetViewMatrix() {
        return Matrix4.LookAt(position, position + _front, _up);
    }

    public Matrix4 GetProjectionMatrix() {
        return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), width / height, 0.1f,
            1000f);
    }

    public void Update(KeyboardState input, MouseState mouse, FrameEventArgs e) {
        InputController(input, mouse, e);
    }

    private void InputController(KeyboardState input, MouseState mouse, FrameEventArgs e) {
        if (input.IsKeyDown(Keys.W))
            position += _front * Speed * (float)e.Time;
        if (input.IsKeyDown(Keys.S))
            position -= _front * Speed * (float)e.Time;
        if (input.IsKeyDown(Keys.A))
            position -= _right * Speed * (float)e.Time;
        if (input.IsKeyDown(Keys.D))
            position += _right * Speed * (float)e.Time;
        if (input.IsKeyDown(Keys.Space))
            position.Y += Speed * (float)e.Time;
        if (input.IsKeyDown(Keys.C))
            position.Y -= Speed * (float)e.Time;

        if (_firstMove) {
            LastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
        }

        else {
            var deltaX = mouse.X - LastPos.X;
            var deltaY = mouse.Y - LastPos.Y;

            LastPos = new Vector2(mouse.X, mouse.Y);

            _yaw += deltaX * Sensitivity * (float)e.Time;
            _pitch -= deltaY * Sensitivity * (float)e.Time;
        }

        UpdateVectors();
    }
}