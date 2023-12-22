using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionType : MonoBehaviour
{
    public enum InteractionTypes { Dialogue, Observe, Reaction, PickUp, DropDown, Collectible, GetIn }
    public InteractionTypes InteractionMode;

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
    public bool IsDialogue()
    {
        if (InteractionMode == InteractionTypes.Dialogue)
        {
            if(transform.name == "RobotArmInteraction") InteractionName = "Interactuar";
            else InteractionName = "Hablar";
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
            InteractionName = "Recoger";
            return true;
        }
        else if (InteractionMode == InteractionTypes.DropDown)
        {
            InteractionName = "Dejar";
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
            InteractionName = "Observar";
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
            InteractionName = "Coleccionable";
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
            InteractionName = "Entrar";
            return true;
        }
        else
        {
            return false;
        }
    }
}
