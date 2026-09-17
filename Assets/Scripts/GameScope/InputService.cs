using UnityEngine;
using VContainer.Unity;
using UnityEngine.InputSystem;

public class InputService : ITickable, IInputService
{
    public Vector2 TouchPoint { get; private set;  }
    public Vector2 TouchDelta { get; private set; }

    public bool IsPressed { get; private set; }

    public void Tick()
    {
       if(Pointer.current == null ) return;
        IsPressed = Pointer.current.press.isPressed;
        if (IsPressed)
        {
            TouchPoint = Pointer.current.position.ReadValue();
            TouchDelta = Pointer.current.delta.ReadValue();

        }
      
    }
}
