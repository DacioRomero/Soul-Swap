using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using System.Reflection;

// Add this to the player character
public class PlayerInput : CharacterInput
{
    public Camera headCamera;
    public float lookSensitivity = 1;

    CharacterDriver characterDriver;
    float lookPitch = 0;
    bool disableControl = false;

    void Awake()
    {
        characterDriver = GetComponent<CharacterDriver>();

        if (!headCamera)
        {
            Debug.LogError("PlayerController - headCamera not assigned!");
            Debug.Log("PlayerController - Looking for camera in children...");

            headCamera = GetComponentInChildren<Camera>();

            if (!headCamera)
            {
                Debug.LogError("PlayerController - No camera found in children!");
            }

            else
            {
                Debug.Log("PlayerController - Camera found in children, using as headCamera");
            }
        }
    }

    void Update()
    {
        Vector2 movementInput = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        Vector2 lookInput = new Vector2(CrossPlatformInputManager.GetAxisRaw("Mouse X"), CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

        if(Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(headCamera.transform.position, headCamera.transform.forward, out hit))
            {
                CharacterInput character = hit.transform.GetComponent<CharacterInput>();

                if (character)
                {
                    SoulSwap(character);
                }
            }
        }

        transform.Rotate(new Vector3(0, lookInput.x));
        lookPitch -= lookInput.y;
        lookPitch = Mathf.Clamp(lookPitch, -90, 90);
        headCamera.transform.localRotation = Quaternion.Euler(lookPitch, 0, 0);

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

    // Swaps input scripts and uses reflection to copy variable values
    void SoulSwap(CharacterInput character)
    {
        Vector3 cameraOffset = headCamera.transform.localPosition;
        headCamera.transform.parent = character.transform;
        headCamera.transform.localPosition = cameraOffset;
        
        Component thisBase = gameObject.AddComponent(character.GetType());

        foreach (FieldInfo f in character.GetType().GetFields())
        {
            f.SetValue(thisBase, f.GetValue(character));
        }

        Component otherBase = character.gameObject.AddComponent(GetType());
        
        foreach (FieldInfo f in GetType().GetFields())
        {
            if (!f.IsNotSerialized)
            {
                f.SetValue(otherBase, f.GetValue(this));
            }
        }

        Destroy(character);
        Destroy(this);
    }
}
