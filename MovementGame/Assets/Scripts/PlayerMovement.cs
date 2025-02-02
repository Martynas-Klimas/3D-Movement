using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float HOOKSHOT_FOV = 90f;
    private const float NORMAL_FOV = 60f;

    private CharacterController characterController;
    private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float gravityForce = -60f;
    [SerializeField] private Transform hitPointTransform;
    [SerializeField] private float hookShotSpeedMultiplier = 2f;
    [SerializeField] private float hookShotMinSpeed = 10f;
    [SerializeField] private float hookShotMaxSpeed = 40f;
    [SerializeField] private float hookShotRange = 50f;
    [SerializeField] private float hookShotCancelDrag = 3f;
    [SerializeField] private Transform hookShotTransform;

    private float hookShotCancelRange = 1f;
    private float hookShotSize; 
    private Vector3 playerMomentum;
    private Vector3 movement;
    private Vector3 hookShotPosition;
    private float cameraVerticalAngle;
    private float characterVelocity;
    private float characterVelocityY;
    private float cameraFovChangeSpeed = 20f;
    private float cameraFov;
    private State state;

    private enum State
    {
        Normal,
        HookShotThrown,
        HookShotFlying,
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        cameraFov = playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        state = State.Normal;
        hookShotTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case State.Normal:
                PlayerLook();
                PlayerMove();
                HookShotStart();
                break;

            case State.HookShotThrown:
                HookShotThrow();
                PlayerLook();
                PlayerMove();
                break;
                    
            case State.HookShotFlying:
                PlayerLook();
                HookShotMovement();
                break;
        } 
    }

    private void PlayerLook()
    {
        float lookX = Input.GetAxis("Mouse X");
        float lookY = Input.GetAxis("Mouse Y");

        transform.Rotate(new Vector3(0f, lookX * mouseSensitivity, 0f), Space.Self);

        cameraVerticalAngle -= lookY * mouseSensitivity;

        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);

        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0f, 0f);
    }

    private void PlayerMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        movement = transform.right * moveX * moveSpeed + transform.forward * moveZ * moveSpeed;

        if (characterController.isGrounded) { 
            characterVelocityY = 0f;
            if (Input.GetKeyDown(KeyCode.Space)) {
                characterVelocityY = jumpSpeed;
            }
        }

        characterVelocityY += gravityForce * Time.deltaTime;

        movement.y = characterVelocityY;

        movement += playerMomentum;

        characterController.Move(movement * Time.deltaTime);

        //reduce momentum after hookshot cancel mid-air
        if(playerMomentum.magnitude >= 0f)
        {
            playerMomentum -= playerMomentum * hookShotCancelDrag * Time.deltaTime;
            if (playerMomentum.magnitude < .0f) {
                playerMomentum = Vector3.zero;
            }
        }
    }

    private void ResetGravity()
    {
        characterVelocityY = 0f;
    }

    private void HookShotStart()
    {
        RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, hookShotRange))
            {
                hitPointTransform.position = hit.point;
                hookShotPosition = hit.point;
                Debug.Log("Did Hit");
                hookShotSize = 0f;
                hookShotTransform.gameObject.SetActive(true);
                hookShotTransform.localScale = Vector3.zero;
                state = State.HookShotThrown;
            }
        }
    }

    private void HookShotThrow()
    {   
        
        hookShotTransform.LookAt(hookShotPosition);
        float hookShotThrowSpeed = 60f;

        hookShotSize += hookShotThrowSpeed * Time.deltaTime;
        hookShotTransform.localScale = new Vector3(1, 1, hookShotSize);

        if(hookShotSize >= Vector3.Distance(transform.position, hookShotPosition)){
            state = State.HookShotFlying;

        }
    }

    private void HookShotMovement()
    {
        hookShotTransform.LookAt(hookShotPosition);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, HOOKSHOT_FOV, Time.deltaTime * cameraFovChangeSpeed);

        Vector3 hookShotDirection = (hookShotPosition - transform.position).normalized;
        float hookShotSpeed = Mathf.Clamp(Vector3.Distance(hookShotPosition, transform.position), hookShotMinSpeed, hookShotMaxSpeed);
        characterController.Move(hookShotDirection * hookShotSpeed *  hookShotSpeedMultiplier * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //cancelled with jump
            float extraSpeed = 7f;
            playerMomentum = hookShotDirection * hookShotSpeed * extraSpeed;
            playerMomentum += Vector3.up * jumpSpeed;
            CancelHookShot();
        }

        if(Vector3.Distance(transform.position, hookShotPosition) < hookShotCancelRange)
        {
            //reached target
            CancelHookShot();
        }
    }

    private void CancelHookShot()
    {
        state = State.Normal;
        ResetGravity();
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, NORMAL_FOV, Time.deltaTime * cameraFovChangeSpeed);
        hookShotTransform.gameObject.SetActive(false);
    }
}
