using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Хранилище идентификаторов шейдерных свойств
/// </summary>
public class ShaderParams
{
    public static int offset = Shader.PropertyToID("_Offset");
    public static int destructionAmount = Shader.PropertyToID("_DestructionAmount");
    public static int intensity = Shader.PropertyToID("_Intensity");
}
