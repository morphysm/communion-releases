using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public bool IsHolding { get; private set; }

    void Update()
    {
        IsHolding = Keyboard.current.nKey.isPressed;
    }
}