using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    public bool CanInteract = true;
    public string PromptMessage;
    public Sprite PromptIcon; // <- Tambahan ikon unik
    public UnityEvent OnInteract;

    public virtual void BaseInteract()
    {
        if (!CanInteract)
            return;
        Interact();
    }

    protected virtual void Interact()
    {
        OnInteract?.Invoke();
    }
}
