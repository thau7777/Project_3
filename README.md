<a name="readme-top"></a>

<!-- BANNER -->

<p align="center">
  <img src="YOUR_BANNER_URL" width="100%" />
</p>

<h1 align="center">🎮 Project 3 – Roguelike Gameplay Prototype</h1>

<p align="center">
  <b>A Unity-based roguelike prototype featuring multiple gameplay modes</b>
</p>

<p align="center">
  <a href="YOUR_BUILD_LINK">▶️ Play Game</a> •
  <a href="YOUR_TRAILER_LINK">🎥 Trailer</a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Engine-Unity-black" />
  <img src="https://img.shields.io/badge/Language-C%23-blue" />
  <img src="https://img.shields.io/badge/Focus-Gameplay%20Programming-orange" />
  <img src="https://img.shields.io/badge/Genre-Roguelike-purple" />
</p>

---

## 👋 Introduction

This repository is a fork of a team-developed Unity project, adapted to highlight my individual contributions and technical perspective as a gameplay programmer.

The project is a **roguelike prototype** featuring two gameplay modes: **top-down action** and **turn-based mechanics**.

My work focuses specifically on the **top-down gameplay mode**, where I was responsible for implementing core gameplay systems, designing interactions, and enhancing overall game feel through visual and audio feedback.

This repository reflects both my implementation work and my approach to gameplay system design, responsiveness, and real-time interaction.

> 🔍 This repository is intended as a portfolio piece to demonstrate my gameplay programming and game feel design skills.

---

## 📌 About The Project

This is a **roguelike game prototype** developed in Unity as part of a team project.

The game includes two main gameplay modes:

* ⚔️ **Top-down action mode** (real-time combat)
* ♟️ **Turn-based mode** (strategic gameplay)

My contribution is focused entirely on the **top-down gameplay experience**.

* 🎯 Genre: Roguelike
* 👨‍💻 Role: Gameplay Programmer (Top-down mode)
* 🎮 Focus: Combat, interaction, and game feel

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## ✨ Key Features (Top-Down Mode)

* ⚔️ Combo-based combat system
* 🎮 Responsive player input handling
* 🎥 Player-controlled camera system
* 🌿 Dynamic environmental interaction
* 💥 VFX-driven feedback (VFX Graph & Particle System)
* 🔊 Integrated sound effects for gameplay feedback
* 🧩 UI elements tailored for top-down gameplay

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 👤 My Contribution

I was responsible for the **design and implementation of the top-down gameplay mode**, including both technical systems and player experience.

### 🎮 Gameplay Programming

* Built core combat system (combo-based attacks)
* Implemented player input handling
* Designed gameplay responsiveness and control flow

### 🌍 Environment Interaction

* Grass reacts dynamically to wind and nearby objects
* Leaves are displaced when objects move through them
* Snow surface deforms based on player movement

### 💥 Visual Effects (VFX)

* Designed and implemented VFX using Unity VFX Graph
* Created particle systems for combat and environment feedback

### 🔊 Audio Design

* Integrated sound effects to enhance gameplay feedback

### 🧩 UI (Top-Down Mode)

* Designed and implemented gameplay UI

### 📁 Code Ownership

* All scripts in:

  * `Assets/Scripts/`
  * `Assets/LokiInspector/`

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🧠 Technical Challenges & Observations

### 1. Script Responsibility

Some gameplay scripts (including my own) handle multiple responsibilities, which can reduce maintainability.

### 2. System Coupling

Gameplay systems are somewhat tightly coupled, making them harder to extend independently.

### 3. Combat Flow Management

The combat system could benefit from a more structured approach such as a state machine.

### 4. Separation of Effects and Logic

VFX and gameplay logic are sometimes closely linked, which may limit flexibility.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🚀 Proposed Improvements

* Apply SOLID principles more consistently
* Introduce a state machine for combat and player states
* Decouple VFX, audio, and gameplay logic
* Improve modularity for better scalability

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🛠️ Tech Stack

* 🎮 Unity
* 💻 C#
* 💥 VFX Graph
* 🎨 Particle System

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---
## 🧩 Design Approach & Patterns

During development, I explored and applied several design patterns to structure gameplay systems and improve code organization:

* **Hierarchical State Machine (HSM)**
  Used to structure player behavior and combat flow, allowing more flexible transitions between states compared to simple state handling.

* **Singleton**
  Applied for managing global systems such as input or core managers.

* **Object Pooling**
  Used to optimize performance for frequently spawned objects such as VFX and gameplay effects.

* **Flyweight / Factory-style approach**
  Used to reuse shared data and reduce duplication in certain gameplay elements.

* **Event Bus (event-driven communication)**
  Helped decouple systems such as input, gameplay logic, and feedback systems.

* **Strategy Pattern**
  Applied to separate interchangeable gameplay behaviors and logic variations.

* **Builder-style approach**
  Used in constructing more complex objects or configurations in a controlled manner.

While these patterns were implemented based on my learning from various resources, I recognize that my implementations are still at an early stage and not fully optimized.

Some systems could be further improved in terms of:

* clearer separation of responsibilities
* better abstraction and reusability
* more consistent application of design principles

This project represents my effort to move from writing functional code to designing more structured and maintainable systems.

## 🧠 What I Learned

* Designing gameplay systems within a multi-mode game
* Building responsive real-time combat systems
* Enhancing game feel through VFX and audio
* Identifying architectural limitations in team projects
* Thinking about scalability and maintainability

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## ⭐ Support

If you find this project interesting, feel free to give it a ⭐!
