using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Playermove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private PlayerInputHandler _input;
    private Rigidbody2D _rb;
    private int _facing = 1; 

    void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
        _rb    = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate()
    {
        _rb.velocity = _input.MoveInput * moveSpeed;
        UpdateFacing(_input.MoveInput.x);

    }

    private void UpdateFacing(float moveX)
    {
        if (Mathf.Abs(moveX) < 0.01f) return;

        int dir = moveX > 0f ? 1 : -1;
        if (dir == _facing) return;

        _facing = dir;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
    }

    
    void Update()
    {
        
    }
}
