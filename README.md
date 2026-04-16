<a name="readme-top"></a>

<!-- BANNER -->

<p align="center">
  <img src="YOUR_BANNER_URL" alt="Banner" width="100%" />
</p>

<h1 align="center">🎮 Project 3</h1>

<p align="center">
  <b>A Unity gameplay prototype focusing on combat and player interaction</b>
</p>

<p align="center">
  <a href="YOUR_BUILD_LINK">▶️ Play Game</a> •
  <a href="YOUR_TRAILER_LINK">🎥 Trailer</a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Engine-Unity-black" />
  <img src="https://img.shields.io/badge/Language-C%23-blue" />
  <img src="https://img.shields.io/badge/Type-Team%20Project-orange" />
  <img src="https://img.shields.io/badge/Status-Completed-green" />
</p>

---

## 📚 Table of Contents

* [📌 About The Project](#-about-the-project)
* [🖼️ Gameplay Preview](#-gameplay-preview)
* [✨ Features](#-features)
* [👥 My Contribution](#-my-contribution)
* [🧠 Technical Challenges & Observations](#-technical-challenges--observations)
* [🚀 Proposed Improvements](#-proposed-improvements)
* [🛠️ Tech Stack](#️-tech-stack)
* [🧠 What I Learned](#-what-i-learned)
* [🎮 Controls](#-controls)
* [📂 Project Structure](#-project-structure)
* [👥 Contributors](#-contributors)

---

## 📌 About The Project

This project was developed as part of my game development learning journey using Unity.
The goal was to build a playable prototype while practicing gameplay programming and system design.

* 🎯 Genre: Action / Combat Prototype
* ⏱ Development Time: (YOUR TIME)
* 👥 Team Size: (NUMBER OF MEMBERS)
* 👨‍💻 Role: Gameplay Programmer

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🖼️ Gameplay Preview

<p align="center">
  <img src="SCREENSHOT_1" width="45%" />
  <img src="SCREENSHOT_2" width="45%" />
</p>

<p align="center">
  <img src="GAMEPLAY_GIF" width="70%" />
</p>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## ✨ Features

* ⚔️ Combo-based attack system
* 🎥 Camera rotation control
* 🎮 Responsive input handling
* 🧠 Basic gameplay interaction

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 👥 My Contribution

As a gameplay programmer in this team project, my main responsibilities included:

* Implemented player combat system (combo attack logic)
* Handled player input system
* Developed camera control system
* Improved gameplay responsiveness and feel

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🧠 Technical Challenges & Observations

During development, I identified several technical and architectural limitations:

### 1. Script Responsibility Issues

Some scripts handled multiple responsibilities (input, movement, combat in a single class), which reduced maintainability.

### 2. Tight Coupling Between Systems

Systems were tightly connected, making it harder to extend or modify individual components.

### 3. Scalability Concerns

The current structure may become difficult to scale when adding more features like AI, skills, or UI systems.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🚀 Proposed Improvements

If I were to continue developing this project, I would:

### 🔧 Apply SOLID Principles

* Separate logic into smaller, focused components
* Ensure each class has a single responsibility

### 🧱 Refactor Architecture

From:

* One script handling multiple systems

To:

* InputHandler
* PlayerController
* PlayerCombat
* CameraController

### ⚙️ Improve Maintainability

* Reduce dependencies between systems
* Improve scalability and readability

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🛠️ Tech Stack

* 🎮 Engine: Unity
* 💻 Language: C#
* 🧱 Concepts:

  * OOP
  * Basic SOLID principles

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🧠 What I Learned

* Implementing gameplay systems in Unity
* Handling input and animation flow
* Identifying code structure issues in team projects
* Thinking in terms of scalability and maintainability
* Understanding the importance of clean architecture

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 🎮 Controls

| Action        | Input        |
| ------------- | ------------ |
| Attack        | Left Mouse   |
| Rotate Camera | Middle Mouse |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 📂 Project Structure

```
Assets/
 ├── Scripts/
 ├── Animations/
 ├── Prefabs/
 ├── Scenes/
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## 👥 Contributors

<a href="https://github.com/thau7777/Project_3/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=thau7777/Project_3" />
</a>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## ⭐ Support

If you find this project interesting, feel free to give it a ⭐!

<p align="right">(<a href="#readme-top">back to top</a>)</p>
