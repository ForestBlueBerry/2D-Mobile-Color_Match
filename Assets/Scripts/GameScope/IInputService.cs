using UnityEngine;

public interface IInputService
{
    bool IsPressed { get; }
     Vector2 TouchPoint { get;  }
     Vector2 TouchDelta { get; }
}
