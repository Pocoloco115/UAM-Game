# Jaguarcín Game

![Unity](https://img.shields.io/badge/Engine-Unity-black?logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/Framework-.NET-512BD4?logo=.net&logoColor=white)
![2D Platformer](https://img.shields.io/badge/Genre-2D_Platformer-blue)
![Status](https://img.shields.io/badge/Status-In_Development-yellow)
![License](https://img.shields.io/badge/License-Custom-lightgrey)

**Jaguarcín Game** is a 2D platformer inspired by the official mascot of the **Universidad Americana (UAM)**.  
The project is developed using the **Unity** engine and sprite creation tools such as **Piskel**.

---

<h1 align="center">🧁 Developed by <b>Muffin Studios</b></h1>

<div align="center">
  <img src="Media/Idle.png" alt="A Game About Killing Goats or Something Logo" width="150" style="display:inline-block; margin-right:20px;"/>
  <img src="Media/Muffin studios png logo.png" alt="Muffin Studio Logo" width="200" style="display:inline-block;"/>
</div>

---

## 🕹 Key Features

### Gameplay
The objective is to **reach the highest score possible** by jumping onto platforms.  
Each time Jaguarcín touches a platform, the score increases by **1**.

The run ends if:
- You touch a spike  
- You leave the camera bounds  

In both cases, Jaguarcín **explodes**.

### Character Abilities
- **Horizontal dash**  
- **Double jump**  
- **Wall climbing**  

---

## 🎮 Main Menu
![Badge](https://img.shields.io/badge/Preview-Main_Menu-green)

![Main Menu](Media/main_menu.gif)

---

## 🏃 Gameplay Preview
![Badge](https://img.shields.io/badge/Preview-Gameplay-blue)

![Gameplay](Media/gameplay.gif)

---

## ⚙️ Settings Menu (Keybinds)
![Badge](https://img.shields.io/badge/Preview-Settings-orange)

![Settings](Media/settings_menu.gif)

---

## 💻 Technologies Used
- **Unity** (core engine)  
- **C#** (scripting)  
- **Piskel** (sprite and animation design)   

---

## 🚀 Running the Game
### Download the build (Release)

1. Go to the **Releases** section of the repository
2. Download the `Builds.zip`
3. Unzip the `.zip`
4. Run the game (UAM game.exe)

---

## 🗂 Project Structure
```text
JaguarcinGame/
│
├── Assets/
│   ├── Animation/
│   ├── Audio/
│   ├── Material/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   ├── Settings/
│   ├── Simple 2D Platformer BE2/
│   ├── Sprites/
│   ├── TextMeshPro/
│   ├── Tile/
│   ├── Tilemap/
│   └── UI/
│
├── Media/
│   └── gifs/
│       ├── main_menu.gif
│       ├── gameplay.gif
│       └── settings_menu.gif
│
├── README.md
└── ProjectSettings/
