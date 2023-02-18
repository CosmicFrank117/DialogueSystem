using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue System/Line Table")]
public class DialogueDatabase: ScriptableObject
{
    public DialogueLineData[] data;
}
