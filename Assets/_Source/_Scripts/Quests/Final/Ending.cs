using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ending", menuName = "Final/Ending")]

public class Ending : ScriptableObject
{
    [TextArea(4, 10)]
    public List<string> content = new List<string>();
}
