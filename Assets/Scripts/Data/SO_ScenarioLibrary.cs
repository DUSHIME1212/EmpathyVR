// Create this file: Assets/Scripts/Data/SO_ScenarioLibrary.cs
using UnityEngine;
using System.Collections.Generic;
using EmpathyVR.Data;

namespace EmpathyVR.Data
{
    [CreateAssetMenu(fileName = "ScenarioLibrary",
                     menuName = "EmpathyVR/Scenario Library")]
    public class SO_ScenarioLibrary : ScriptableObject
    {
        public List<SO_Scenario> scenarios;
    }
}