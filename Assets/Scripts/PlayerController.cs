using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Reflection;

// Add this to the player character
public class PlayerController : InputController
{
    public float lookSensitivity = 1;
    
    float lookPitch = 0;

    void Update()
    {
        Vector2 movementInput = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        Vector2 lookInput = new Vector2(CrossPlatformInputManager.GetAxisRaw("Mouse X"), CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

        transform.parent.Rotate(new Vector3(0, lookInput.x));
        lookPitch -= lookInput.y;
        lookPitch = Mathf.Clamp(lookPitch, -90, 90);
        transform.localRotation = Quaternion.Euler(lookPitch, 0, 0);

        if (characterDriver.isGrounded)
        {
            characterDriver.velocity = transform.rotation * new Vector3(movementInput.x, 0, movementInput.y) * characterDriver.walkSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                characterDriver.velocity *= characterDriver.sprintSpeed / characterDriver.walkSpeed;
            }

            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
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
        if (Input.GetKeyDown(KeyCode.E))
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
}
