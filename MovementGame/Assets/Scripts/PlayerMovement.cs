using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float gravityForce = -60f;
    private Vector3 movement;
    private float cameraVerticalAngle;
    private float characterVelocityY;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        PlayerLook();
        PlayerMove();
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

        characterController.Move(movement * Time.deltaTime);
    }
}
