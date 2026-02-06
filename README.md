# 🌵 Space Station Cactus

**Space Station Cactus** is a Unity-based VR game about keeping a cactus alive inside a space station orbiting a star.  
The player must manage temperature, sunlight, and upgrades while reacting to orbital cycles and system failures.

The project focuses on **clean architecture**, **event-driven design**, and **XR-ready gameplay systems**, rather than visual complexity.

---

## 🎮 Core Gameplay

- The space station alternates between **Sunlit** and **Eclipse** orbit phases
- Windows automatically open at sunrise (with a chance to fail)
- Temperature rises or falls based on window state
- Keeping the cactus within a safe temperature range builds a **score multiplier**
- If the cactus remains outside its alive range for too long — **game over**
- Upgrades improve automation and introduce active abilities (AC pulse)

---

## 🧱 Architecture Overview

- **Observer pattern** used extensively via a centralized event hub
- **ScriptableObject-based configuration** for all gameplay tuning
- Clear separation between:
  - Gameplay logic
  - Visual presentation
  - UI
  - Debug tooling
- No hard-coded cross-system references
- Designed to be easy to extend, test, and debug

---

## 🛠 Tools & Technologies

- **Unity 6.3**
- **OpenXR / XR Interaction Toolkit**
- **Visual Studio 2026**
- **Blender** (model adjustments & animation testing)

---

## 📦 External Assets & Packages

The following third-party assets were used in the project:

### 3D Models
- Philips TV model  
  https://free3d.com/3d-model/-philips-379589.html
- Air Conditioner model  
  https://free3d.com/3d-model/airconditioner-v4--671561.html

### Textures & Materials
- Blue Metal Plate  
  https://polyhaven.com/a/blue_metal_plate
- Corrugated Iron  
  https://polyhaven.com/a/corrugated_iron
- Spaceship Panels  
  https://freepbr.com/product/spaceship-panels/

### Skybox
- Deep Space Skybox Pack (Unity Asset Store)  
  https://assetstore.unity.com/packages/2d/textures-materials/deep-space-skybox-pack-11056

### Packages
- glTFast (for glTF model importing)  
  https://github.com/atteneder/glTFast

All assets are used for **educational and non-commercial purposes** as part of this project.
