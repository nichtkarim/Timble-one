using UnityEngine;

/// <summary>
/// Class <c>Point</c> If a game object is clickable, it implements this interface, so InptHandler can call OnClick on it.
/// </summary>
public interface IClickable
{
    void OnClick();
}