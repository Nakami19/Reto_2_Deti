using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    #region Variables

    Camera mainCamera;

    [Header("Variables de movimiento")]
    public float speed = 5f;
    public float currentSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 5f;
    Vector3 currentMoveVelocity = Vector3.zero;
    [SerializeField] float acceleration = 10f;


    [Header("Variables para checkear wallrun")]
    public CollisionDetectorRaycast leftCollider;
    public CollisionDetectorRaycast rightCollider;

    [Header("Variables del Wallrun")]
    [SerializeField] private float wallJumpCooldown = 2f;
    private float wallJumpCooldownTimer = 0f;
    Vector3 wallRunDirection;
    public bool fallWhileWallrunning = true;
    public float wallRunJumpHeight = 3f;
    public float wallRunSpeedTreshold = 3.5f;
    public float wallRunGravity = -20f;
    public float maxFallSpeed = -2f;
    public float wallJumpForceMultiplier = 1.5f;
    bool isWallRunning = false;
    bool isTouchingWallLeft;
    bool isTouchingWallRight;
    bool isInAir;
    bool onRightWall;


    CharacterController controller;

    PlayerControls playerControls;
    Vector2 moveInput;

    float verticalVelocity;
    bool wantJump;
    bool wantWallJump;

    #endregion


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerControls = new PlayerControls();
        playerControls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); //Lee el movimiento
        playerControls.Player.Move.canceled += ctx => moveInput = Vector2.zero; //Captura fin del movimiento
        playerControls.Player.Jump.performed += ctx =>
        {
            if (controller.isGrounded)
                wantJump = true;
            else if (isWallRunning)
                wantWallJump = true;
        }; //Lee el salto
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
        mainCamera = Camera.main; //Obtiene la camara principal
    }

    // Update is called once per frame
    void Update()
    {
        WallRun();
        if (wallJumpCooldownTimer > 0f)
        {
            wallJumpCooldownTimer -= Time.deltaTime;
        }
        Jump();
        PlayerMove();      
    }

    void PlayerMove()
    {
        if (isWallRunning) return;

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


        //Aplica movimiento y gravedad
        Vector3 velocity = currentMoveVelocity + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime); //Aplica el movimiento al CharacterController
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            if (wantJump)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); //Salto                
            }
            else
            {
                verticalVelocity = -1f; //Mantiene al jugador en el suelo
            }
            wantJump = false; //Reinicia el salto
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; //Aplica gravedad
        }
    }

    #region WallRun
    void WallRun()
    {

        if (wallJumpCooldownTimer > 0f)
        {
            isWallRunning = false;
            return;
        }

        float planarSpeed = new Vector2(currentMoveVelocity.x, currentMoveVelocity.z).magnitude;
        bool isSpeedEnough = planarSpeed >= wallRunSpeedTreshold;
        bool isMovingForward = moveInput.y > 0.1f;

        isTouchingWallLeft = leftCollider.isColliding;
        isTouchingWallRight = rightCollider.isColliding;
        isInAir = !controller.isGrounded;

        if ((isTouchingWallLeft || isTouchingWallRight) && isInAir && isSpeedEnough && isMovingForward)
        {
            isWallRunning = true;
            onRightWall = isTouchingWallRight;

            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            wallRunDirection = camForward;

            if (wantWallJump)
            {
                wantWallJump = false;
                isWallRunning = false;
                wallJumpCooldownTimer = wallJumpCooldown;

                Vector3 awayFromWall = onRightWall ? -Camera.main.transform.right : Camera.main.transform.right;
                awayFromWall.y = 0f;
                awayFromWall.Normalize();

                Vector3 jumpDirection = (awayFromWall + Vector3.up).normalized;

                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                currentMoveVelocity = jumpDirection * currentSpeed * wallJumpForceMultiplier;

                return;
            }

            if (fallWhileWallrunning)
            {
                verticalVelocity = Mathf.Max(verticalVelocity + wallRunGravity * Time.deltaTime, maxFallSpeed);
            }
            else
            {
                verticalVelocity = 0f;
            }

            Vector3 velocity = wallRunDirection * currentSpeed + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime);

        }
        else
        {
            isWallRunning = false;
        }
    }

    #endregion


}
