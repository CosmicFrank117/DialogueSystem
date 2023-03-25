using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Dialogue System/Line Table")]
public class DialogueDatabase: ScriptableObject
{
    [FormerlySerializedAs("data")]
    public List<DialogueLineData> dataList;
}
