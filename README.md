# 💎 Crystal Castles Unified Ingestion Suite — v0.95 Release README
**Core Data Architecture, Memory Sandboxing, and Viewport Synchronization Complete**

## 🚀 Overview
Version 0.95 marks the absolute conclusion of the data parsing validation phase and functional code freeze for the **Crystal Castles Unified Ingestion Suite**. 

All 37 original game level variants have been meticulously cross-referenced, hand-corrected, and synchronized across the **2D Flat Blueprint view**, **3D Isometric Workspace**, and **Diagnostic Context Canvas**. This version establishes your audited text configurations (`STAGE_NN_*.txt`) within `.\data\unified_data\` as the single, read-only source of truth for the entire software suite.

---

## 🎨 Major Engineering Achievements & Milestone Fixes

### 1. High-Clearance Parser Expansion (`CCUnifiedParser.cs`)
*   **The Issue:** The legacy ingestion pipeline aggressively forced a hardcoded clamping cap of `rawHeightValue > 99 ? (byte)99`. This restriction crushed the custom `099` high-altitude ceiling coordinates used to handle the top courtyard platforms on the Castle and Crossroads maps. Platforms would freeze or clip beneath geometry structures.
*   **The Fix:** Expanded the token scanner array limits to support a broader ceiling capacity of `100` via a secure boundary guard: `(byte)Math.Clamp((int)height, 0, 100);`. High-clearance layouts now flow straight into RAM exactly as written.

### 2. Isolated Laboratory Deep-Cloning Sandbox (`DiagnosticCanvas.cs`)
*   **The Issue:** Toggling view modes frequently triggered cross-viewport data corruption, scrambling platform maps. This was caused by a pass-by-reference memory leak where the diagnostic panel directly latched onto global `RomManager.IsolatedStages` pointers.
*   **The Fix:** Implemented a non-destructive object instantiation sandbox. The diagnostic loop now generates an independent `CityData` container on execution and explicitly deep-copies every structural coordinate cell, layer attribute, and moving elevator register element. Core 3D engine arrays remain 100% pristine.

### 3. Anti-Glare 9-Shade Altitude Heatmap (`MapRenderer.cs`)
*   **The Issue:** The previous flat coloring style turned maps like the *Nasty Tree green waves* and *Crossroads magenta variations* solid black on low-contrast laptop displays. Shading multipliers dropped below screen visibility thresholds, rendering step terraces invisible.
*   **The Fix:** Programmed a local maximum altitude scanner that dynamically maps heights into 9 distinct linear tiers. Added a customized **Contrast Baseline Boost Gate**: if color registers drop near absolute darkness, the engine injects a fixed `0.45f` brightness visibility floor, ensuring stair-step contours are perfectly defined across all 37 maps.

### 4. Input Handler Safety & Overwrite Protections (`InputHandler.cs`)
*   **The Issue:** Pressing the `E` key to tilt the camera in 3D mode accidentally triggered background script macro snapshots, causing heavy disk-write stuttering. Additionally, overlapping file export structures caused viewport sessions to clobber and erase each other's records.
*   **The Fix:** Added an explicit viewport constraint gate (`if (!is3DMode)`) to completely lock out file exports from running inside the 3D viewport. Rewired the main sidebar shortcut text label to mirror this transition (`E : Export`). Single-room files are now uniquely prefixed to isolate `main_export_*.log` passes from `diag_export_*.log` assets.

---

## 📂 Core Folder Structure Reference Guide
The suite layout uses the following standardized relative path mapping schema to maintain version insulation:
*   `.\data\unified_data\` — The master read-only group
