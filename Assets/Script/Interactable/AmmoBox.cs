using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : Interactable
{
    [SerializeField] private Pistol pistol;
    [SerializeField] private int ammoAmount;


    protected override void Interact()
    {
        base.Interact();
        pistol.AddAmmo(ammoAmount);
    }
}