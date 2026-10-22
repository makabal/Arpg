using System;
using UnityEngine;

public sealed class PlayerMovement
{
    private readonly Rigidbody2D _rigidbody;
    private readonly Transform _transform;
    private readonly Func<float> _moveSpeedProvider;
    private int _facing;

    public PlayerMovement(
        Rigidbody2D rigidbody,
        Transform transform,
        Func<float> moveSpeedProvider)
    {
        _rigidbody = rigidbody;
        _transform = transform;
        _moveSpeedProvider = moveSpeedProvider;
        _facing = transform.localScale.x >= 0f ? 1 : -1;
    }

    public void FixedTick(Vector2 moveInput)
    {
        float moveSpeed = _moveSpeedProvider != null
            ? Mathf.Max(0f, _moveSpeedProvider())
            : 0f;
        _rigidbody.velocity = moveInput * moveSpeed;
        UpdateFacing(moveInput.x);
    }

    public void Stop()
    {
        _rigidbody.velocity = Vector2.zero;
    }

    private void UpdateFacing(float moveX)
    {
        if (Mathf.Abs(moveX) < 0.01f)
            return;

        int direction = moveX > 0f ? 1 : -1;
        if (direction == _facing)
            return;

        _facing = direction;
        Vector3 scale = _transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        _transform.localScale = scale;
    }
}
