using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class CharacterMovement1 : MonoBehaviour
{
    #region Variables
    [Header("Variables de movimiento")]
    public float speed = 5f;
    public float currentSpeed = 5f;
    public float gravity = -9.81f;

    Vector3 currentMoveVelocity = Vector3.zero;
    [SerializeField] float acceleration = 10f;

    [HideInInspector] bool isDashing = false;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashTimer = 0f;
    [SerializeField] float dashSpeed = 15f;

    CharacterController controller;

    PlayerControls playerControls;
    Vector2 moveInput;

    float verticalVelocity;

    #endregion


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerControls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); //Lee el movimiento
        playerControls.Player.Move.canceled += ctx => moveInput = Vector2.zero; //Captura fin del movimiento
    }

    void OnEnable()
    {
        playerControls.Enable(); //Habilita el Input Action
    }

    void OnDisable()
    {
        playerControls.Disable(); //Deshabilita el Input Action
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Bloquea el cursor en el centro de la pantalla
        Cursor.visible = false; //Oculta el cursor
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -1f; //Mantiene al jugador en el suelo
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; //Aplica gravedad
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = 2 * speed; //Aumenta la velocidad al correr
        }
        else
        {
            currentSpeed = speed; //Vuelve a la velocidad normal
        }

        //Movimiento plano XZ relativo a la camara

        Vector3 camForward = Camera.main.transform.forward; camForward.y = 0f; camForward.Normalize();
        Vector3 camRight = Camera.main.transform.right; camRight.y = 0f; camRight.Normalize();

        Vector3 targetMove = (camRight * moveInput.x + camForward * moveInput.y) * currentSpeed;
        currentMoveVelocity = Vector3.Lerp(currentMoveVelocity, targetMove, acceleration * Time.deltaTime);


        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            isDashing = true;
            dashTimer = dashDuration;

        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            controller.Move(camForward * dashSpeed * Time.deltaTime);
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }

        //Aplica movimiento y gravedad
        Vector3 velocity = currentMoveVelocity + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime); //Aplica el movimiento al CharacterController
    }

   

}
