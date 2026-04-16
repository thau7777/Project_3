<!-- BANNER -->

<p align="center">
  <img src="YOUR_BANNER" width="100%" />
</p>

<h1 align="center">🎮 Project 3</h1>

<p align="center">
  <b>A Unity-based gameplay prototype focusing on combat and player interaction</b>
</p>

<p align="center">
  <a href="YOUR_BUILD_LINK">▶️ Play Game</a> •
  <a href="YOUR_TRAILER">🎥 Trailer</a>
</p>

---

## 📌 About The Project

This project was developed as part of my game development learning journey using Unity.
The goal was to build a playable prototype while practicing gameplay programming and system design.

* 🎯 Genre: Action / Combat Prototype
* ⏱ Development Time: (YOUR TIME)
* 👥 Team Size: (NUMBER OF MEMBERS)
* 👨‍💻 Role: Gameplay Programmer

---

## 🖼️ Gameplay Preview

<p align="center">
  <img src="SCREENSHOT_1" width="45%" />
  <img src="SCREENSHOT_2" width="45%" />
</p>

<p align="center">
  <img src="GAMEPLAY_GIF" width="70%" />
</p>

---

## ✨ Key Features

* ⚔️ Combo-based attack system
* 🎥 Camera rotation with player control
* 🎮 Responsive player input handling
* 🧠 Basic gameplay interaction systems

---

## 👥 My Contribution

As a gameplay programmer in this team project, my main responsibilities included:

* Implemented player combat system (combo attack logic)
* Handled player input and interaction flow
* Developed camera control system (rotation & usability)
* Focused on improving gameplay responsiveness and feel

---

## 🧠 Technical Challenges & Observations

During development, I identified several technical challenges and architectural limitations:

### 1. Script Responsibility Issues

Some scripts handled multiple responsibilities (e.g., input, movement, and combat in a single class), making them harder to maintain and extend.

### 2. Tight Coupling Between Systems

Certain systems were tightly coupled, reducing flexibility and making future changes more difficult.

### 3. Scalability Limitations

The current structure would become difficult to scale if more features (e.g., skills, enemies, UI systems) were added.

---

## 🚀 Proposed Improvements

If I were to continue developing this project, I would improve the architecture as follows:

### 🔧 Apply SOLID Principles

* Separate responsibilities into smaller components
* Ensure each system handles only one core function

### 🧱 Refactor System Structure

From:

* One script handling multiple gameplay systems

To:

* InputHandler → handles player input
* PlayerController → handles movement
* PlayerCombat → handles attack logic
* CameraController → handles camera behavior

### ⚙️ Improve Maintainability

* Reduce dependencies between systems
* Make code easier to extend and debug

---

## 🛠️ Tech Stack

* 🎮 Engine: Unity
* 💻 Language: C#
* 🧱 Programming Concepts:

  * Object-Oriented Programming (OOP)
  * Basic SOLID principles

---

## 🧠 What I Learned

* Designing and implementing gameplay systems in Unity
* Handling player input and animation flow
* Understanding the importance of clean architecture
* Identifying and analyzing code structure issues in team projects
* Thinking about scalability and maintainability in game development

---

## 🎮 Controls

| Action        | Input        |
| ------------- | ------------ |
| Attack        | Left Mouse   |
| Rotate Camera | Middle Mouse |

---

## 📂 Project Structure

```
Assets/
 ├── Scripts/
 ├── Animations/
 ├── Prefabs/
 ├── Scenes/
```

---

## 📈 Future Improvements

* Improve combat system with better state management
* Add enemy AI behavior
* Enhance visual feedback (VFX, screen shake, sound)
* Optimize performance and code structure

---

## 👤 Author

* Trần Hậu
* GitHub: https://github.com/thau7777

---

## ⭐ Support

If you find this project interesting, feel free to give it a ⭐!
