using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocater
{
  private static readonly Dictionary<System.Type, object> services = new Dictionary<System.Type, object>();

  /// <summary>
  /// Registers a service instance.
  /// </summary>
  /// <typeparam name="T">The type of the service.</typeparam>
  /// <param name="service">The service instance.</param>
  public static void RegisterService<T>(T service) where T : class
  {
    var type = typeof(T);

    if (services.ContainsKey(type))
    {
      Debug.LogWarning($"Service of type {type} is already registered.");
      return;
    }

    services[type] = service;
  }

  /// <summary>
  /// Gets a registered service instance.
  /// </summary>
  /// <typeparam name="T">The type of the service.</typeparam>
  /// <returns>The service instance.</returns>
  public static T GetService<T>() where T : class
  {
    var type = typeof(T);

    if (services.TryGetValue(type, out var service))
    {
      return service as T;
    }

    Debug.LogError($"Service of type {type} is not registered.");
    return null;
  }

  /// <summary>
  /// Unregisters a service instance.
  /// </summary>
  /// <typeparam name="T">The type of the service.</typeparam>
  public static void UnregisterService<T>() where T : class
  {
    var type = typeof(T);
    if (!services.Remove(type))
    {
      Debug.LogWarning($"Service of type {type} was not registered.");
    }
  }
}
