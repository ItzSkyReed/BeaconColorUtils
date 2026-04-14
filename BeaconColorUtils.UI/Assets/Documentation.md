# Beacon Color Utils

**Beacon Color Utils** is a tool designed for Minecraft players and builders to achieve surgical precision with beacon beam colors.

---

## About the Program
**Beacon Color Utils** is a desktop application created to solve the limitations of the default Minecraft beacon palette. 
Since beacon beams change color by averaging values as they pass through layers of stained glass, the number of possible combinations reaches into the millions.
This program handles the heavy mathematical lifting by using pre-made caches, allowing you to find the desired shade using the most efficient sequence of glass blocks or panes.

---

## Key Features

### 1. Glass Sequence Calculator
* **Hex/RGB Search:** Enter any color code, and the program will find a combination of up to 6 glass layers that result in the closest possible match.
* **Economical Mode:** If 3 layers of glass produce the same result as 6, the program will automatically prioritize the more resource-efficient sequence.
* **Beam Preview:** A real-time visualization of how the beacon beam will look in-game.

### 2. Beacon Gradient Generator
* **Multi-stop Gradients:** Create smooth transitions between any number of key colors.
* **Step Configuration:** Define exactly how many beacons should be in your row.
* **Interpolation Modes:** Support for various color blending modes to ensure transitions look natural and vibrant.

### 3. NBT Export
* Export your generated gradients directly into a Structure File (NBT). This allows you to instantly place a complete line of configured beacons into your world using WorldEdit or Litematica.

---

## Why Oklab is Better than RGB

Most graphics software works in **RGB** space. However, RGB was designed for monitors and hardware, not for the human eye.

### The Problem with RGB:
In RGB, a mathematical change (e.g., +10 Green vs. +10 Blue) is not perceived equally by humans. We are much more sensitive to green, making RGB gradients often appear "muddy," uneven, or inconsistent in brightness.

### The Advantage of Oklab:
Oklab is a **perceptually uniform** color space.
* **Linearity:** The distance between colors in Oklab matches how we actually see differences.
* **Perfect Gradients:** When calculating transitions in Oklab, the perceived lightness (L) remains stable. This eliminates the "dark spots" or "neon glows" often found in the middle of standard RGB gradients.

> In this application, all accuracy and transition calculations are performed in Oklab to ensure the result in Minecraft is visually perfect, not just mathematically "correct" on a screen.

---

## What is DeltaE?

**DeltaE** (ΔE) is a metric used to define the "distance" between two colors. Simply put, it tells you how much the color achieved in Minecraft differs from the color you actually wanted.

### Why do we use a simple formula?
While complex standards like CIEDE2000 exist to account for specific human vision quirks, we utilize Euclidean distance since the Oklab space is linear.