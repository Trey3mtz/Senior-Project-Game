using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestScrictableObj", menuName = "ScriptableObj/TestScriptableObj")]
public class TestScriptable : ScriptableObject
{
    public string myString;

}

//[CreateAssetMenu(fileName = "Sub_TestScrictableObj", menuName = "ScriptableObj/TestScriptableObj/Sub_TestScrictableObj")]
public class Sub_TestScriptable : TestScriptable
{
    public string myString_2;

}