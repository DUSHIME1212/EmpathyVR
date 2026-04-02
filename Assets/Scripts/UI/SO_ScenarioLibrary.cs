using System;
using System.Collections.Generic;
using UnityEngine;
using EmpathyVR.Data;

namespace EmpathyVR.UI
{
    [CreateAssetMenu(fileName = "ScenarioLibrary", menuName = "EmpathyVR/Scenario Library")]
    public class SO_ScenarioLibrary : ScriptableObject
    {
        [Header("Available Scenarios")]
        [Tooltip("The definitive list of all playable scenarios in the game.")]
        public List<SO_Scenario> scenarios;

        /// <summary>
        /// Finds a scenario by its unique ID.
        /// </summary>
        public SO_Scenario GetScenarioById(string id)
        {
            if (scenarios == null) return null;
            return scenarios.Find(s => s.scenarioId == id);
        }
    }
}
