using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionType : MonoBehaviour
{
    public enum InteractionTypes { Dialogue, Observe, Reaction, PickUp, DropDown, Collectible, GetIn }
    public InteractionTypes InteractionMode;

    public string SpecialInteractionText;
    [HideInInspector]
    public string InteractionName;

    public void Start()
    {
        IsDialogue();
        IsPickUpOrDropDown();
        IsObserve();
        IsCollectible();
        IsGetIn();
    }
    string SpecialInteraction(string InteractionText)
    {
        if (SpecialInteractionText != InteractionText) return SpecialInteractionText;
        else return InteractionText;
    }
    public bool IsDialogue()
    {
        if (InteractionMode == InteractionTypes.Dialogue)
        {
            InteractionName = SpecialInteraction("Hablar");
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsPickUpOrDropDown()
    {
        if (InteractionMode == InteractionTypes.PickUp)
        {
            InteractionName = SpecialInteraction("Recoger");
            return true;
        }
        else if (InteractionMode == InteractionTypes.DropDown)
        {
            InteractionName = SpecialInteraction("Dejar");
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsObserve()
    {
        if (InteractionMode == InteractionTypes.Observe)
        {
            InteractionName = SpecialInteraction("Observar");
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsReaction()
    {
        if (InteractionMode == InteractionTypes.Reaction)
        {
            InteractionName = "";
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsCollectible()
    {
        if (InteractionMode == InteractionTypes.Collectible)
        {
            InteractionName = SpecialInteraction("Coleccionable");
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsGetIn()
    {
        if (InteractionMode == InteractionTypes.GetIn)
        {
            InteractionName = SpecialInteraction("Entrar");
            return true;
        }
        else
        {
            return false;
        }
    }
}
