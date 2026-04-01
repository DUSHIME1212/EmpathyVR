# 🌍 EmpathyVR — Immersive Empathy Through Virtual Reality

> *Step into the life of a Rwandan farmer. Experience the weight of impossible choices.*

![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?logo=unity)
![XR](https://img.shields.io/badge/Platform-Meta%20Quest%20%7C%20PCVR-blue?logo=oculus)
![Language](https://img.shields.io/badge/Language-C%23-purple?logo=csharp)
![License](https://img.shields.io/badge/License-Academic-green)

---

## Table of Contents

1. [Project Overview](#-project-overview)
2. [GDD Alignment](#-gdd-alignment)
3. [Features & Functionality](#-features--functionality)
4. [Architecture & Code Quality](#-architecture--code-quality)
5. [Scene Structure](#-scene-structure)
6. [Performance Optimization](#-performance-optimization)
7. [Setup & Installation](#-setup--installation)
8. [Video Walkthrough](#-video-walkthrough)
9. [Known Issues & Limitations](#-known-issues--limitations)
10. [References & Assets](#-references--assets)

---

## Project Overview

**EmpathyVR** is an immersive, narrative-driven Virtual Reality experience designed to build empathy through embodied perspective-taking. The player inhabits the daily life of *Amara*, a Rwandan subsistence farmer facing the compounding pressures of climate change, water scarcity, and family responsibility.

The experience confronts the player with morally difficult decisions that have real-world parallels — choices that millions of people face every day — presented through:

- **360° photographic skyboxes** that dynamically swap per narrative chapter
- **Narrated chapters** with subtitle display and ambient audio
- **Interactive moral decisions** with timed choice panels
- **Real-world impact statistics** shown after every decision
- **Persistent player choice recording** across the full experience

**Target Platform:** Meta Quest 2/3 & PCVR (OpenXR)
**Target Runtime:** 8–12 minutes per scenario
**Engine:** Unity 2022.3 LTS

---

## GDD Alignment

The vertical slice implements the following systems from the Game Design Document:

| GDD Feature | Status | Notes |
|---|---|---|
| Scene flow: Menu → Briefing → Story Select → Experience → Reflection | ✅ Implemented | Managed by `SceneLoader` singleton |
| Data-driven narrative via ScriptableObjects | ✅ Implemented | `SO_Scenario`, `SO_NarrativeChapter`, `SO_DecisionData` |
| Chapter-based skybox environment | ✅ Implemented | `EnvironmentManager` + `SkyboxRotator` |
| Decision system with countdown timer | ✅ Implemented | `DecisionManager` + `DecisionUI` + `TimerUI` |
| Consequence display with real-world statistics | ✅ Implemented | `ConsequenceUI` + `ConsequenceResolver` |
| Ambient audio persistence across scenes | ✅ Implemented | `AudioManager` (DontDestroyOnLoad) |
| Player choice recording for reflection scene | ✅ Implemented | `SO_PlayerChoiceRecord` |
| VR Gaze interaction for headset input | ✅ Implemented | `GazeInteractor` + `GazeReticleUI` |
| World-space UI following headset camera | ✅ Implemented | `WorldCanvasFollower` |

---

## Features & Functionality

### Narrative Engine
- Chapters are driven by `SO_NarrativeChapter` ScriptableObjects containing: chapter ID, narration text, narration audio clip, ambient audio tag, skybox material, and decision trigger flag.
- `NarrativeEngine` sequences text and audio playback, then raises an event when the chapter is complete.
- `GameManager` acts as the central orchestrator — registering all scene-local managers and coordinating chapter progression without coupling individual systems.

### Dynamic Skybox Environments
- Each chapter has a unique 360° panoramic skybox (farm morning, midday heat, harvest evening).
- `EnvironmentManager` swaps `RenderSettings.skybox` on chapter load and calls `DynamicGI.UpdateEnvironment()` for correct ambient lighting.
- `SkyboxRotator` applies a configurable slow rotation to the skybox material's `_Rotation` property each frame, adding life and immersion to otherwise static photos.

### Decision System
- When a chapter is marked `Trigger Decision After`, the `DecisionManager` presents a `SO_DecisionData` asset containing: prompt text, two options (each with label, consequence preview, and outcome headline), and a time limit.
- A countdown timer pressures the player; if time expires, the default option is chosen automatically.
- After selecting, `DecisionUI` is instantly hidden (`ForceHide()`) and `ConsequenceUI` fades in showing the player's choice, outcome headline, real-world statistic, and the next chapter title.

### Scene Initialization System
- `SceneInitializer` wires all scene-local components (NarrativeEngine, DecisionManager, EnvironmentManager, HUDController) explicitly to the persistent `GameManager` on `Awake()`.
- This solves the race-condition problem where `GameManager` tried to call chapter logic before scene objects were registered.
- `StartExperience()` is only called after a 0.1s deferred invoke, ensuring all `OnEnable`/`Start` calls have completed.

---

## Architecture & Code Quality

EmpathyVR follows a **modular, SOLID-principled architecture** using a clear namespace hierarchy:

```
EmpathyVR/
├── EmpathyVR.Core          → GameManager, SceneLoader, SceneInitializer, SceneBootstrapper
├── EmpathyVR.Data          → All ScriptableObject definitions (SO_Scenario, SO_NarrativeChapter, SO_DecisionData, SO_PlayerChoiceRecord)
├── EmpathyVR.Narrative     → NarrativeEngine, NarrativeSequencer, NarrativeNode
├── EmpathyVR.Decisions     → DecisionManager, ConsequenceResolver
├── EmpathyVR.Environment   → EnvironmentManager, WeatherController, SkyboxRotator, InteractableObject
├── EmpathyVR.UI            → All UI controllers (DecisionUI, ConsequenceUI, HUDController, BriefingUI, StorySelectionUI, GazeReticleUI, WorldCanvasFollower...)
├── EmpathyVR.Audio         → AudioManager, AmbientAudioTrigger
└── EmpathyVR.Player        → GazeInteractor, PlayerLocomotion, PlayerState
```

### Design Patterns Used

| Pattern | Where Applied |
|---|---|
| **Singleton** | `GameManager`, `SceneLoader`, `AudioManager`, `DecisionManager` |
| **Observer / Event** | `GameManager` raises chapter/decision events; systems subscribe |
| **Strategy** | `ConsequenceResolver` resolves outcomes based on choice data |
| **Data Object (ScriptableObject)** | All game content decoupled from runtime logic via SO assets |
| **Facade** | `GameManager` provides a single interface to all subsystems |
| **DontDestroyOnLoad** | `GameManager`, `AudioManager`, `SceneLoader` persist across scene loads |

### SOLID Principles
- **S** — Each class has a single responsibility (`EnvironmentManager` only manages skyboxes and environments, not narrative or decisions).
- **O** — New scenarios are added by creating new `SO_Scenario` assets — no code changes needed.
- **L** — All panels implement consistent `Show()` / `Hide()` / `ForceHide()` interfaces.
- **I** — UI components are registered through narrow `Register___()` interfaces on `GameManager`.
- **D** — `NarrativeEngine`, `DecisionManager` and `EnvironmentManager` depend on `GameManager` abstractions, not on each other.

---

## Scene Structure

```
Build Index | Scene Name                      | Purpose
------------|----------------------------------|-------------------------------
0           | 00_Boot                         | Bootstraps persistent managers
1           | 01_MainMenu                     | Title screen & navigation
2           | 02_Briefing                     | Context briefing before experience
3           | 03_StorySelection               | Choose a scenario (The Farmer's Morning)
4           | 04_Experience_FarmersMorning    | Main VR narrative experience
5           | 05_Reflection                   | Post-experience reflection & choices review
```

### Farmers Morning Hierarchy
```
04_Experience_FarmersMorning
├── Directional Light
├── XR Interaction Manager
├── Managers                    ← SkyboxRotator
├── XR Origin (VR)              ← Tracked camera rig
├── Scene_Managers              ← WeatherController, EnvironmentManager
└── SceneInitializer            ← Wires all managers to GameManager
    ├── DecisionManager
    ├── [Environment]           ← EnvironmentManager
    └── Experience_UI           ← World Space Canvas (WorldCanvasFollower)
        ├── Decision_Panel      ← DecisionUI (starts inactive)
        ├── Consequence_Panel   ← ConsequenceUI (starts inactive)
        └── HUD_Panel           ← Chapter title, subtitle, timer (always visible)
```

---

## Performance Optimization

The following optimizations have been implemented and profiled targeting **72 FPS** on Meta Quest 2:

### 1. Static Skybox Material Swapping (Draw Call Reduction)
- Instead of instantiating 3D environment assets, chapters use skybox material swaps.
- This reduces scene draw calls from ~200+ (with 3D models) to under **40 draw calls** per scene.
- Materials are referenced directly on the `SO_NarrativeChapter` ScriptableObject — no `Resources.Load()` or runtime asset loading.

### 2. DontDestroyOnLoad Singleton Reuse (Scene Load Optimization)
- `AudioManager`, `GameManager`, and `SceneLoader` are instantiated once and reused across all scenes.
- Eliminates repeated instantiation costs and prevents garbage collection spikes on scene transitions.

### 3. UI Activation Control (Rendering Cost Reduction)
- `Decision_Panel` and `Consequence_Panel` start as `SetActive(false)`.
- Only the visible panel is active at any time, eliminating GPU overdraw from layered transparent canvases.
- `DOKill()` is called before every panel transition to prevent tween accumulation.

### 4. Shader Property ID Caching (CPU Overhead Reduction)
- `SkyboxRotator` caches the `_Rotation` shader property ID with `Shader.PropertyToID()` in a static field, avoiding string-hash lookups every frame.

### 5. Camera Reference Caching
- `WorldCanvasFollower` caches `Camera.main` in `Start()` and accesses the cached transform in `LateUpdate()`, avoiding the expensive `GameObject.FindWithTag("MainCamera")` call per frame.

**FPS Benchmark:** Profiled at **72–90 FPS** in Unity Editor (Play Mode) on PC. Quest 2 target is **72 FPS stable**.

---

## Setup & Installation

### Prerequisites
- Unity **2022.3 LTS** or newer
- XR Plugin Management package
- OpenXR or Oculus XR Plugin
- DOTween (Demigiant) — free version from Asset Store
- TextMeshPro (included with Unity)

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/DUSHIME1212/EmpathyVR.git
   cd EmpathyVR
   ```

2. **Open in Unity Hub**
   - Open Unity Hub → Add project → select the cloned folder.

3. **Install missing packages** (if prompted)
   - Window → Package Manager → install any missing packages shown in console.

4. **Configure XR**
   - Edit → Project Settings → XR Plug-in Management
   - Enable **OpenXR** (PC) or **Oculus** (Android/Quest)

5. **Build for Quest (Android)**
   - File → Build Settings → Switch Platform → Android
   - Ensure `04_Experience_FarmersMorning` scene is in the build list
   - Build & Run with Quest connected via USB

6. **Play in Editor (Desktop)**
   - Open scene `04_Experience_FarmersMorning`
   - Assign your `Scenario_FarmersMorning` ScriptableObject to `SceneInitializer → Test Scenario`
   - Press ▶ Play

---

## Video Walkthrough

> **[Watch Full Walkthrough on YouTube / Google Drive](#)**  *(https://drive.google.com/drive/folders/1odjf1t1w8F0l8pv_gwfrThc3Z2wILJ5N?usp=sharing)*

The video demonstrates:
- [ ] Scene flow from Main Menu → Briefing → Story Selection → Experience
- [ ] Skybox swap between chapters
- [ ] Decision panel appearing with countdown timer
- [ ] Consequence panel showing real-world impact statistics
- [ ] Chapter progression through all Farmers Morning chapters
- [ ] Ambient audio playing throughout
- [ ] Full experience ending and transition to Reflection scene

---

## Known Issues & Limitations

| Issue | Status | Notes |
|---|---|---|
| Gaze interaction requires calibration per device | Minor | Adjust `GazeInteractor` dwell time in Inspector for headset |
| `AudioSource` components must be manually assigned to `AudioManager` in prefab | Minor | See walkthrough for setup steps |
| Timer display resets if decision is selected in the last frame | Low | Cosmetic only, does not affect outcome recording |
| Narration audio clips are placeholder audio | Roadmap | Full Kinyarwanda/English voice-over planned |

---

## References & Assets

| Asset / Tool | Source | Usage |
|---|---|---|
| **DOTween** | [Demigiant](http://dotween.demigiant.com/) | UI fade animations |
| **TextMeshPro** | Unity Built-in | All in-game text rendering |
| **Farm Skybox Materials** | [Polyhaven](https://polyhaven.com/) / Custom | 360° environment backgrounds |
| **XR Interaction Toolkit** | Unity Registry Package | XR rig, input, and ray interaction |
| **Ambient Audio** | Royalty-free / Custom | Farm morning soundscapes |
| **SOLID Principles Reference** | Robert C. Martin — *Clean Code* | Architecture design |
| **VR UI Best Practices** | [Unity XR Documentation](https://docs.unity3d.com/Manual/xr_reference.html) | World space canvas setup |

---

## Author

**DUSHIME** — AR/VR Development  
*Vertical Slice Submission — [Course Name]*  
*[Institution Name] — [Semester/Year]*

---

> "Empathy is seeing with the eyes of another, listening with the ears of another, and feeling with the heart of another." — Alfred Adler
