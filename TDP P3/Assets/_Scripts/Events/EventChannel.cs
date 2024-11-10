using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public abstract class EventChannel<T> : ScriptableObject
{
  readonly HashSet<EventListener<T>> observers = new();

  public void Register(EventListener<T> observer) => observers.Add(observer);

  public void Unregister(EventListener<T> observer) => observers.Remove(observer);

  public void Invoke(T value)
  {
    foreach(var observer in observers)
    {
      observer.Raise(value);
    }
  }
}

public readonly struct Empty { }

[CreateAssetMenu(menuName = "Events/Event Channel/Empty")]
public class EventChannel : EventChannel<Empty> {  }
