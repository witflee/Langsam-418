# Langsam-418

## Dependencies
Git LFS - [Installation Instructions](https://docs.github.com/en/repositories/working-with-files/managing-large-files/installing-git-large-file-storage)

## Motivation

The goal of this project was to create a virtual reality simulation of our Langsam classroom, allowing users to explore a digital recreation of the space. Originally, we aimed to produce a highly realistic environment with accurate 3D models of the room's furniture, so that different configurations of tables and chairs could be tested and visualized within VR. As the project progressed, however, animation constraints led us to shift toward more stylized, cartoonish characters, which made model creation and animation significantly more manageable.

---

## Design

We began building the environment by applying textures to an existing Blender model of the Langsam classoom. We added lights to the ceiling and ambient sound to the classroom. From there, we modeled the room's furniture by photographing the real objects with a phone and importing those images into Blender as references. Most furniture models use simple flat materials for color, though the classroom chairs required a more advanced technique. This included UV unwrapping and custom texture creation to accurately capture their appearance.

> <img width="621" height="361" alt="Screenshot from 2026-04-26 21-45-27" src="https://github.com/user-attachments/assets/fe2c2b40-dad3-4e6d-9aae-dddfa27fc81a" />

> <img width="615" height="253" alt="Screenshot from 2026-04-26 21-47-38" src="https://github.com/user-attachments/assets/0278c51b-42f4-4738-a896-1dd81521cb6e" />


The NPC characters were also built and rigged in Blender as shown below. Animations were keyframed using the Dope Sheet editor and then exported into Unity. Interactable objects found throughout the room, such as props and miscellaneous items, were sourced from online. This combination of methods resulted in our final room below.

> <img width="547" height="418" alt="shortened" src="https://github.com/user-attachments/assets/90528f0b-0272-437d-bdda-7c3e498b317f" />


> *finished room*

---

## Process

### Repository & Collaboration

All code was managed through a shared GitHub repository. Each team member worked on their own features independently and pushed contributions to the repo. Because of this structure, the codebase is organized around individual features rather than a single unified architecture. You can access the project files from this repo to download and run the application. 

### 3D Modeling & Animation

All models not sourced from the internet were created in Blender. NPC animations were keyframed in Blender's Dope Sheet, split into named actions, and exported to Unity. Inside Unity, we used the **Animation Controller** to define animation states and set up triggers between them. Scripts attached to character objects handle trigger logic, with raycasts fired from the VR controller detecting interaction events.

### VR Interaction

Storyboards modeled planned interactions with elements of the classroom. Interactable objects (items the player can pick up) were built using components from the **Meta SDK** and 3D models. The in-room screen and eraser use custom scripts that track and record controller movement to simulate a paint/draw application on the screen surface.

### Libraries & Tools

| Tool | Purpose |
|------|---------|
| [Blender](https://www.blender.org/) | 3D modeling, rigging, and animation |
| [Unity](https://unity.com/) | Game engine and scene assembly |
| [Meta SDK](https://developer.oculus.com/) | VR interaction components |
| GitHub | Version control and team collaboration |

## Challenges & Future Work

### Challenges

One learning curve for the project was animating characters from scratch. No one on the team had prior experience with character rigging, so learning to build a mesh, bind a skeleton, and keyframe animations in Blender, and then correctly importing those animations into Unity's Animation Controller, took considerable time and effort.

Setting up the shared repository and getting the Unity project running consistently across all team members' machines also caused early delays, which then compressed the time available for feature development and polish. We also struggled to include a more featureful canvas system and instead had to reduce the system to simple one color drawing and erasing. Our canvas system with more features is pictured below.

> <img width="1800" height="1169" alt="Screenshot_2026-04-24_at_8 30 40_PM" src="https://github.com/user-attachments/assets/6341e305-da58-4833-99ea-5a53075df8df" />


### Future Work

Our biggest takeaway is the importance of hitting milestones and completing features earlier in the development cycle. Finishing core systems ahead of schedule leaves time to iterate, polish, and avoid the kind of last-minute scope cuts we experienced here.

---

## AI & Collaboration

AI tools were used selectively throughout the project, primarily to assist with code. While we had a clear understanding of the logic we wanted to implement, the Meta SDK is large and complex, so an AI helped us navigate its documentation and identify the specific components needed to implement our planned features, saving significant time on library research.
