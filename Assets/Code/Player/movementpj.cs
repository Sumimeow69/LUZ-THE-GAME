using UnityEngine;
using UnityEngine.InputSystem;


public class movementpj : MonoBehaviour
{
    [SerializeField] private float movspeed = 5.0f;
    [SerializeField] private Rigidbody2D rb;

    Vector2 movement;

    
    void Update()
    {
        movement.x = 0;
        movement.y = 0;

        if (Keyboard.current.aKey.isPressed)
            movement.x = -1;

        if (Keyboard.current.dKey.isPressed)
            movement.x = 1;

        if (Keyboard.current.sKey.isPressed)
            movement.y = -1;

        if (Keyboard.current.wKey.isPressed)
            movement.y = 1;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position + movement * movspeed * Time.fixedDeltaTime
        );
    }
}
