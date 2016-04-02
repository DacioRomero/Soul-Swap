using UnityEngine;
using System.Collections;

// Add this to the AI character
[RequireComponent(typeof(CharacterDriver))]
public class IdleInput : CharacterInput
{
    public float exploreDistance = 4;
    public float exploreWait = 4;

    CharacterDriver characterDriver;
    Vector3? destination;
    float exploreWaitTimer;

    void Awake()
    {
        characterDriver = GetComponent<CharacterDriver>();

        exploreWaitTimer = exploreWait;
    }

    void Update()
    {
        if (characterDriver.isGrounded)
        {
            Debug.Log("Grounded");

            if (destination == null)
            {
                if (exploreWaitTimer < exploreWait)
                {
                    exploreWaitTimer += Time.deltaTime;
                }

                if (exploreWaitTimer >= exploreWait)
                {
                    Vector3 randomDirection = Random.insideUnitCircle;
                    randomDirection.z = randomDirection.y;
                    randomDirection.y = 0;

                    destination = transform.position + randomDirection * exploreDistance;
                    exploreWaitTimer = 0;
                }
            }

            if (destination != null)
            {
                Vector3 direction = (destination.Value - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, destination.Value);

                characterDriver.velocity = direction * characterDriver.walkSpeed;

                transform.eulerAngles = new Vector3(0, Vector3.Angle(Vector3.forward, direction), 0);

                if ((characterDriver.velocity * Time.deltaTime).magnitude >= distance)
                {
                    characterDriver.velocity = Vector3.zero;
                    transform.position = destination.Value;
                    destination = null;
                }
            }

            characterDriver.velocity.y = -characterDriver.groundedVelocity;
        }

        else
        {
            Debug.Log("Not grounded");
            characterDriver.velocity += Physics.gravity * Time.deltaTime;
        }
        Debug.Log("Velocity " + characterDriver.velocity.y);

        characterDriver.Move(characterDriver.velocity * Time.deltaTime);
    }
}
