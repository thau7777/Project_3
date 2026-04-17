<a name="readme-top"></a>

<!-- BANNER -->

<p align="center">
  <img src="README_assets/GameTitle.png" width="80%" />
</p>

<h1 align="center">A Graduation Game Project developed at VTC Academy</h1>

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

This repository contains a team-developed Unity project.

The project was developed collaboratively, and this documentation isolates my technical contributions for clarity.

This README focuses specifically on my individual contributions within the project, highlighting my work as a gameplay programmer, particularly in the top-down gameplay mode.

The project is a **roguelike prototype** featuring two gameplay modes: **top-down action** and **turn-based mechanics**.

My work focuses on implementing core gameplay systems, designing interactions, and enhancing overall game feel through visual and audio feedback.

I aimed to build **scalable gameplay systems using event-driven and pattern-based design**, while also improving player experience through responsive controls and immersive feedback.

> 🔍 This README is intended as a portfolio piece to demonstrate my gameplay programming and system design skills.
> 
> ⚠️ This project was developed as part of a graduation project with a focus on learning and experimentation. Some systems may not be fully polished, but the implementation demonstrates my understanding of gameplay architecture, scalability, and real-world development practices in Unity.

## 📅 Development Timeline

- Development period: [Month Year] – [Month Year]
- Team size: X members

The project was developed within a constrained timeframe, which influenced prioritization decisions and iterative development. Despite these constraints, I focused on building scalable gameplay systems and maintaining code structure where possible.

---

## 📚 Table of Contents

* [About The Project](#-about-the-project)
* [Gameplay Preview](#-gameplay-preview)
* [Key Features](#-key-features-top-down-mode)
* [My Contribution](#-my-contribution)
* [Technical Challenges & Observations](#-technical-challenges--observations)
* [Proposed Improvements](#-proposed-improvements)
* [Tech Stack](#️-tech-stack)
* [Design Approach & Patterns](#-design-approach--patterns)
* [What I Learned](#-what-i-learned)

---

## 📌 About The Project

This is a **roguelike game prototype** developed in Unity as part of a team project.

The game includes two main gameplay modes:

* **Top-down action mode** (real-time combat)
* **Turn-based mode** (strategic gameplay)

My contribution is focused entirely on the **top-down gameplay experience**.

* Genre: Roguelike
* Role: Gameplay Programmer (Top-down mode)
* Focus: Combat systems, interaction, and game feel

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## 🤝 Team & Contributors

<p align="center">
  <a href="https://github.com/thau7777">
    <img src="https://github.com/thau7777.png" width="80px;" alt="thau7777"/>
    <br />
    <sub><b>Trần Hậu</b></sub>
    <br />
    <sub>Top-down Gameplay Programmer & Designer</sub>
    <br />
    <sub>Combat, AI(HSM), Skills/Items, Enviroment, VFX & Game Feel</sub>
  </a>

  <a href="https://github.com/TriLau">
    <img src="https://github.com/TriLau.png" width="80px;" alt="TriLau"/>
    <br />
    <sub><b>Tri Lau</b></sub>
    <br />
    <sub>Main Menu, Lobby & World Map</sub>
    <br />
    <sub>Main System Programmer(Save/Load, Settings), VFX, Balance</sub>
  </a>

  <a href="https://github.com/Luhieunghia2001">
    <img src="https://github.com/Luhieunghia2001.png" width="80px;" alt="Luhieunghia2001"/>
    <br />
    <sub><b>Lu Hieu Nghia</b></sub>
    <br />
    <sub>Turn-based Gameplay Programmer</sub>
    <br />
    <sub>Turn-based System Design(Combat, AI, Skills/Items)</sub>
  </a>

  <a href="https://github.com/Ridotakarin">
    <img src="https://github.com/Ridotakarin.png" width="80px;" alt="Ridotakarin"/>
    <br />
    <sub><b>Ridotakarin</b></sub>
    <br />
    <sub>Minigame Programmer & Designer</sub>
    <br />
    <sub>Publishing & Game Balance Support</sub>
  </a>
</p>

<p align="center">
  This project was developed collaboratively. This README highlights my individual contributions,
  primarily focused on gameplay programming and system design in the top-down mode.
</p>
---

## 🖼️ Gameplay Preview
<p align="center"><b>** ComboAttack **</b></p>
<p align="center">
  <img src="README_assets/ComboAttack.gif" width="97%" />
</p>
<p align="center"><b>** Skills, Weather And Maps **</b></p>
<p align="center">
  <img src="README_assets/Bubble.gif" width="32%" />
  <img src="README_assets/IceLance.gif" width="32%" />
  <img src="README_assets/FlameTornado.gif" width="32%" />
</p>
<p align="center"><b>** Enviroment Details **</b></p>
<p align="center">
  <img src="README_assets/GrassInteract.gif" width="48.5%" />
  <img src="README_assets/SnowInteract.gif" width="48.5%" />
</p>
<p align="center"><b>** Boss Fight **</b></p>
<p align="center">
  <img src="README_assets/stingray.gif" width="32%" />
  <img src="README_assets/blackknight.gif" width="32%" />
  <img src="README_assets/beholder.gif" width="32%" />
</p>
<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## ✨ Key Features (Top-Down Mode)

* Combo-based combat system

* Responsive player input handling

* Player-controlled camera system

* **Hierarchical State Machine (Player & Enemy AI)**
  Structured and scalable state transitions for gameplay behaviors

* **Enemy AI System**

  * Pathfinding & obstacle avoidance using NavMesh + CharacterController
  * Dynamic separation to prevent clustering
  * Context-aware move selection based on distance
  * Ranged enemy positioning (kiting behavior)
  * Boss phase-based abilities

* **Skill System (30+ skills)**
  Strategy-based design for scalable and extendable behaviors

* **Item System (~10 types)**
  Reusable and flexible item behavior structure

* **Event-driven gameplay (Event Bus)**
  Decoupled communication between systems

* **Object management (Factory + Object Pooling)**
  Efficient spawning and reuse of gameplay objects

* Dynamic environmental interaction

  * Grass reacts to nearby objects
  * Leaves are displaced on movement
  * Snow deforms based on position

* VFX-driven feedback (VFX Graph & Particle System)

* Integrated sound effects

* Gameplay UI for top-down mode
<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 👤 My Contribution

I was responsible for the design and implementation of the top-down gameplay mode.

### Gameplay Programming

* Combat system (combo-based attacks)
* Input handling and gameplay flow
* Responsive control system

### System Design

* HSM for player and enemy behaviors
* Strategy-based skill and item systems
* Event-driven communication (Event Bus)

### Environment Interaction

* Grass, leaves, and snow interaction systems

### Visual Effects

* VFX Graph and particle-based feedback

### Audio

* Integrated gameplay sound effects

### UI

* Gameplay UI for top-down mode

### Code Ownership

* `Assets/Scripts/`
* `Assets/LokiInspector/`
<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🧠 Technical Challenges & Observations

### 1. Script Responsibility

Some scripts (including my own) handle multiple responsibilities, reducing maintainability.

### 2. System Coupling

Certain systems are tightly coupled, making them harder to extend or modify independently.

### 3. Combat Flow Complexity

Combat logic could benefit from a more structured and consistent state management approach.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🚀 Proposed Improvements

* Apply SOLID principles more consistently
* Improve separation between systems
* Refactor combat logic into a clearer state structure
* Improve modularity and scalability

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🛠️ Tech Stack

* Unity
* C#
* VFX Graph
* Particle System

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🧩 Design Approach & Patterns

During development, I explored and applied multiple design patterns to improve system structure, scalability, and performance.

### 🧠 Core Architecture

* **Hierarchical State Machine (HSM)**
  Used to drive both player and enemy behaviors, enabling scalable and structured state transitions.

---

### 🧩 Gameplay Systems

* **Strategy Pattern (Skills & Items)**
  Used to implement flexible systems where behaviors are encapsulated and easily extendable.

---

### 📡 System Communication

* **Event Bus (Event-Driven Architecture)**
  Used extensively to decouple systems and allow flexible communication between gameplay components.

---

### ♻️ Performance & Resource Management

* **Object Pooling**
  Used to reuse frequently spawned objects such as VFX.

* **Flyweight + Factory-style approach**
  Used to manage shared data and centralize object creation across multiple pools.

---

### ⚠️ Reflection

While these patterns were applied based on my learning, I recognize that my implementations are still evolving and not fully optimized.

Areas for improvement include:

* clearer abstraction between systems
* more consistent use of design principles
* improving flexibility and reusability

This project represents my transition from writing functional code to designing more structured and scalable gameplay systems.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🧠 What I Learned

* Designing scalable gameplay systems using patterns
* Building responsive real-time combat systems
* Using event-driven architecture to decouple systems
* Optimizing performance with object pooling
* Enhancing game feel through VFX and audio
* Thinking in terms of system design, not just implementation

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## ⭐ Support

If you find this project interesting, feel free to give it a ⭐!
