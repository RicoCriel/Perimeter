using System.Collections.Generic;
using UnityEngine;
public interface IAimObserver
{
    //define the contract for any class that observes aiming events
    void OnAimStarted();
    void OnAimStopped();
    void OnAimUpdated(Vector3 targetPosition);
}

public class AimSubject 
{
    //manage observers and notify them of aiming-related changes
    private readonly List<IAimObserver> _observers = new List<IAimObserver>();

    public void RegisterObserver(IAimObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void UnregisterObserver(IAimObserver observer)
    {
        if (_observers.Contains(observer))
        {
            _observers.Remove(observer);
        }
    }

    public void NotifyAimStarted()
    {
        foreach (var observer in _observers)
        {
            observer.OnAimStarted();
        }
    }

    public void NotifyAimStopped()
    {
        foreach (var observer in _observers)
        {
            observer.OnAimStopped();
        }
    }

    public void NotifyAimUpdated(Vector3 targetPosition)
    {
        foreach (var observer in _observers)
        {
            observer.OnAimUpdated(targetPosition);
        }
    }
}
