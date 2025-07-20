## 📌 About This Project

This Unity case study was completed as part of the Unity Marketing Developer application at Voodoo.

The objective:
- Recreate the core mechanic of Mob Control in Unity
- Benchmark an ad from a competitor
- Enhance the prototype with a creative, market-focused twist
- Deliver everything with clarity and marketability

## 🔧 Prototype Overview

I built a basic version of Mob Control from scratch using the provided assets.

Features:
- Tap, hold and move cannon to spawn mobs
- Mobs pass through multiplier gates (x2, x5, x10)
- Spawn Enemy from EnemyBase
- Enemy base destruction logic

I focused on getting the **core feedback loop** right: satisfying spawn → gate boost → mob rush → base destruction.

## 🔍 Benchmark Ad & Key Learnings

**Ad Link**: https://youtube.com/shorts/Q8M6Sca-Cf0?si=qhkowhrfTFtUZl6I

Why this ad stood out:
- Hooked me within the first 1.5 seconds with fast gate action
- Bright neon visuals and an explosive, satisfying ending
- Introduced a milestone to destroy a boss gate, creating a goal-driven moment
- Added camera shake and a subtle bump effect on the cannon while shooting to increase tactile feedback
- Clear and simple gameplay made it instantly understandable
- In-game obstacles added challenge and made the progression more engaging

I borrowed the **instant feedback loop and the bright gate VFX** idea to improve retention in the first few seconds of my marketed version.

## 🚀 Marketed Version (Enhanced Concept)

I introduced a **"Futuristic Neon Factory"** theme to make the level feel unique and visually catchy.

New additions:
- Complete medieval castle theme set on a lava river, creating a bold and memorable atmosphere
- Hovering numbers and custom VFX for gate multipliers
- Camera shake effect added when releasing the big enemy for dramatic impact
- Subtle bump effect on the cannon while shooting to increase realism
- Custom shader flicker effect on the enemy base when under attack
- A fun teleport pipe mechanic to surprise the player and add variation on the entrance

This version is made to **grab attention within 3 seconds**, ideal for TikTok/Facebook-style marketing videos.

## 🧗 Challenges & Learnings

- Fine-tuned the timing between clone spawning and gate scaling to ensure smooth gameplay flow
- Gained hands-on experience with Unity’s Recorder
- Deepened my understanding of URP and post-processing
- Adopted a brute-force approach in early development to get the core mechanics functional quickly under time constraints
- Experimented with a theme system architecture, allowing for easy integration and swapping of different visual themes
- Learned how to build a modular theming setup, making it possible to apply and preview new environments (e.g., lava castle) for rapid iteration and visual testing

I kept code quality reasonable but focused heavily on visual feedback and prototyping speed, as instructed.

## 📈 Next Steps & Improvements

Given more time, I would:
- Add dynamic obstacle gates to create meaningful risk/reward decisions during gameplay
- Polish clone spawn animations and VFX for more impactful visual feedback
- Implement a simple UI showing clone count, score, and wave progress
- Explore alternative visual themes (e.g., Candy World, Lava Zone) to broaden audience appeal
- Build a modular theme injection system to allow for applying complete visual overhauls within hours
- Develop custom Unity tools to streamline level design, theme switching, and content iteration
- Introduce architectural patterns such as Dependency Injection (DI), Model-View-Presenter (MVP), and Reactive Programming for better scalability and maintainability
- Experiment with Unity’s Job System, multithreading, and async/await to improve performance and prepare the project for large-scale content

I’m also curious to experiment with adding short reward animations at the end to simulate monetization potential.

## 📦 Deliverables

- ✅ Unity Project (Mob Control Clone)
- ✅ Video 1: Base Prototype
- ✅ Video 2: Marketed Version
- ✅ Benchmark Ad Link & Learnings (this doc)
- ✅ Design & Technical Reflections (this doc)
