using UnityEngine;
using System.Collections;

// This is an empty class used just to be able to easily find the inputs
public class InputController : MonoBehaviour
{
    public CharacterDriver characterDriver;

    public virtual void Awake()
    {
        if(!characterDriver)
        {
            Debug.LogWarning(this + " - characterDriver not assigned, attempted to find in parent");

            characterDriver = GetComponentInParent<CharacterDriver>();

            if(!characterDriver)
            {
                Debug.LogError(this + " - No characterDriver found in parent");
            }
        }
    }
}
