using UnityEngine;
using UnityEngine.InputSystem;

// Add this to the player character
public class PlayerController : InputController
{
    public float lookSensitivity = 1;

    float lookPitch = 0;

    Vector2 lookInput = Vector2.zero;
    Vector2 movementInput = Vector2.zero;
    bool sprint = false;
    bool jump = false;
    bool soulSwap = false;

    void Update()
    {
        transform.parent.Rotate(new Vector3(0, lookInput.x));
        lookPitch -= lookInput.y;
        lookPitch = Mathf.Clamp(lookPitch, -90, 90);
        transform.localRotation = Quaternion.Euler(lookPitch, 0, 0);

        if (characterDriver.isGrounded)
        {
            characterDriver.velocity = transform.rotation * new Vector3(movementInput.x, 0, movementInput.y) * characterDriver.walkSpeed;

            if (sprint)
            {
                characterDriver.velocity *= characterDriver.sprintSpeed / characterDriver.walkSpeed;
            }

            if (jump)
            {
                jump = false;
                characterDriver.velocity.y = characterDriver.jumpVelocity;
            }

            else
            {
                characterDriver.velocity.y = -characterDriver.groundedVelocity;
            }
        }

        else
        {
            characterDriver.velocity += Physics.gravity * Time.deltaTime;
        }

        characterDriver.Move(characterDriver.velocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (soulSwap)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                InputController character = hit.transform.GetComponentInChildren<InputController>();

                if (character)
                {
                    SoulSwap(character);
                }
            }
        }
    }

    void SoulSwap(InputController character)
    {
        Transform cParent = character.transform.parent;
        Vector3 cPosition = character.transform.localPosition;
        character.transform.parent = transform.parent;
        character.transform.localPosition = transform.localPosition;
        transform.parent = cParent;
        transform.localPosition = cPosition;

        CharacterDriver cDriver = character.characterDriver;
        character.characterDriver = characterDriver;
        characterDriver = cDriver;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jump = context.performed;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.performed;
    }

    public void OnSoulSwap(InputAction.CallbackContext context)
    {
        soulSwap = context.performed;
    }
}
