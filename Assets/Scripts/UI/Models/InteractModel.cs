using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractModel 
{
    //Holds the interaction data
    public string InteractionText {  get; private set; }

    public void SetInteractionText(string interactionText)
    {
        InteractionText = interactionText;
    }
}
