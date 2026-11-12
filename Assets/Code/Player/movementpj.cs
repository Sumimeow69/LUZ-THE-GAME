using UnityEngine;
using UnityEngine.InputSystem;


public class movementpj : MonoBehaviour
{
    [SerializeField] private float movspeed = 5.0f;
    [SerializeField] private Rigidbody2D rb;
    private Animator animator;

    Vector2 movement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        
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

        if (movement != Vector2.zero)
        {
            animator.SetFloat("VelocidadX", movement.x);
            animator.SetFloat("VelocidadY", movement.y);
        }
        
        animator.SetBool("Caminando", movement != Vector2.zero);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position + movement * movspeed * Time.fixedDeltaTime
        );
    }
}
