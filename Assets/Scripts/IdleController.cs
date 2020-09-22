using UnityEngine;
using System.Collections;

// Add this to the AI character
public class IdleController : InputController
{
    public float exploreDistance = 4;
    public float exploreWait = 4;
    
    Vector3? destination;
    float exploreWaitTimer;

    public override void Awake()
    {
        base.Awake();

        exploreWaitTimer = exploreWait;
    }

    void Update()
    {
        if (characterDriver.isGrounded)
        {
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

                    destination = transform.parent.position + randomDirection * exploreDistance;
                    exploreWaitTimer = 0;
                }
            }

            if (destination != null)
            {
                Vector3 direction = (destination.Value - transform.parent.position).normalized;
                float distance = Vector3.Distance(transform.parent.position, destination.Value);

                characterDriver.velocity = direction * characterDriver.walkSpeed;

                transform.parent.eulerAngles = new Vector3(0, Vector3.Angle(Vector3.forward, direction), 0);

                if ((characterDriver.velocity * Time.deltaTime).magnitude >= distance)
                {
                    characterDriver.velocity = Vector3.zero;
                    transform.parent.position = destination.Value;
                    destination = null;
                }
            }

            characterDriver.velocity.y = -characterDriver.groundedVelocity;
        }

        else
        {
            characterDriver.velocity += Physics.gravity * Time.deltaTime;
        }

        characterDriver.Move(characterDriver.velocity * Time.deltaTime);
    }
}
