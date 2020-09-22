using UnityEngine;
using System.Collections;

// This class is automatically added when any input is added
[RequireComponent(typeof(CharacterController))]
public class CharacterDriver : MonoBehaviour
{
    public float jumpVelocity = 4;
    public float walkSpeed = 8;
    public float sprintSpeed = 16;
    public float groundedVelocity = 0.125f;

    public bool isGrounded
    {
        get
        {
            return characterController.isGrounded;
        }
    }
    
    [HideInInspector]
    public Vector3 velocity;
    CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector3 motion)
    {
        characterController.Move(motion);
    }
}
