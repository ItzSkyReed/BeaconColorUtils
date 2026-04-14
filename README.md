# Beacon Color Utils

**Beacon Color Utils** is a cross-platform desktop tool designed for Minecraft players and builders to achieve surgical precision with beacon beam colors.

The default [Minecraft beacon](https://minecraft.wiki/w/Beacon) palette is limited, 
but because beacon beams change color by averaging values as they pass through layers of stained glass, 
the number of possible combinations reaches into the millions. This application handles the heavy mathematical lifting, 
using pre-made caches to find the exact glass sequences you need efficiently.

---

## Key Features

* **Glass Sequence Calculator**
  Enter any Hex/RGB code, and the program finds the closest possible match using up to 6 glass layers. It includes a real-time beam preview and an **Economical Mode** that automatically prioritizes shorter glass sequences if they yield the same visual result.
* **Beacon Gradient Generator**
  Create smooth, multi-stop transitions between key colors. You can define exactly how many beacons should be in your row and choose from various interpolation modes to ensure vibrant, natural blends.
* **NBT Export**
  Export your generated gradients directly into a Structure File (NBT). This allows you to instantly place a complete line of configured beacons into your world using tools like WorldEdit or Litematica.

---