# ML-BallCollector
Simple ML project for Introduction To AI class made with ML-Agents



**Project Overview**
---
The ML-BallCollector project is a simple machine learning project designed for an Introduction to AI class. The project involves training a model to control an orange square bracket shaped object to collect a ball and put it in a target area using ML-Agents. The goal is to maximize the reward by collecting the ball as fast as possible and putting it in the target area without agent or ball falling of the platform.

*note that project was made while having injured so unclean code or other issues might occur due to minimizing the time to create the project*

**Preview**
---
Green platform means agent successed, red that agent or ball fall down from the platform.
<!-- Video with animation what model does -->
https://github.com/ikolton/ML-BallCollector/assets/96392714/7d1867ef-62bb-4eb1-b904-a2ada5c38d2f

<!-- important links -->
**Useful links for setup**
---

- [ML-Agents Toolkit installation](https://unity-technologies.github.io/ml-agents/Installation/) is needed to run anything.
- [Unity installation](https://docs.unity3d.com/Manual/GettingStartedInstallingUnity.html) is needed to run the project in editor but probably not for running the model with .exe build. 


<!--How to run tought model with inference and comman for it-->
**Getting Started**
---
1. Setup mlagents according to [ML-Agents Toolkit installation](https://unity-technologies.github.io/ml-agents/Installation/)
2. Clone the repository 
```sh
 git clone https://github.com/ikolton/ML-BallCollector
 ```
 3. Navigate to FinalLearningSetup folder
   
4. To see model in action run this command:


```sh
mlagents-learn BallCollectorBCGlowCur.yaml --run-id=BCGlowCur --env=BuildsForRewTimePen/Ball_in_a_hole --inference --resume --width=1024 --height=1024 --time-scale=5
```
5. To check the tensorboard run this command:
```sh
tensorboard --logdir results --port 6006
```
and then open in web browser localhost:6006
<!-- Setup Unity Procject-->

