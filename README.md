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
![Ball_in_a_hole-2024-06-06-13-29-46](https://github.com/ikolton/ML-BallCollector/assets/96392714/5dbd106a-50ee-4cd3-afbf-681387965cad)

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
   
1. To see model in action run this command (gif above represents effects of this command):


```sh
mlagents-learn BallCollectorBCGlowCur.yaml --run-id=BCGlowCur --env=BuildsForRewTimePen/Ball_in_a_hole --inference --resume --width=1024 --height=1024 --time-scale=5
```
5. To check the tensorboard run this command:
```sh
tensorboard --logdir results --port 6006
```
and then open in web browser localhost:6006

<!-- Setup Unity Procject-->
### Editing unity project
For playing with project follow [Making a New Learning Environment](https://unity-technologies.github.io/ml-agents/Learning-Environment-Create-New/) but use assets from AssetsFromUnity folder.


**Methodology**
---

### Part in Unity:
First step was to create a good learning environment and agent to learn within it. One training are consists of the orange Agent, Ball, Target Area and Platform.

At the beggining of each episode Target Area and Ball are spawned at random positions to prevent overfitting. Agent is spawned in the middle of the platform.
```code
//ball and Goal spawn in random position on the field
RandomizePositiom();
This.localPosition = new Vector3(0, 1f, 0); 
```

During the episode Agent collects observations inlcuding

It's
- velocity
- rotation
- position

Position of
- Ball
- Target Area
  
```code
public override void CollectObservations(VectorSensor sensor)
{
    // Ball and Goal positions
    sensor.AddObservation(BallTran.localPosition);
    sensor.AddObservation(GoalTran.localPosition);
    // Agent position
    sensor.AddObservation(This.localPosition);
    // Agent velocity
    sensor.AddObservation(rBody.velocity.x);
    sensor.AddObservation(rBody.velocity.z);
    //Agent rotation
    sensor.AddObservation(This.rotation.y);
    //Debug.Log(This.rotation.y);
}
```
  
which sums in total to the space of 12 observations.

After analysing the observations model returns actions - 2 floats which are applied to rotate the Agent and move it forward and backward.

```code
// Actions, size = 2, add forward force and rotate
//forward force
forward = actionBuffers.ContinuousActions[0];
//rotation
float rotate = actionBuffers.ContinuousActions[1];

rBody.AddForce(transform.forward * forward * forceMultiplier);

//rotate game object local y axis
transform.Rotate(transform.up * rotate * rotationSpeed);
```

Model recives penalty for time and reward for moving forward. Reward for moving forward is smaller than penalty for time so the model doesn't try to keep moving forward until the end of the episode:

```code
//reward for moving based on forward velocity
if(rBody.velocity.magnitude > 0.1f && forward > 0.05f)
{
    AddReward(0.001f);
}

//penalty for time
if(GetCumulativeReward() >= -10f)
AddReward(-0.002f);
```

Episode can end after 
- 5000 steps
- ball or agent falls of the platform and penalty is applied

```code
if (This.localPosition.y < 0 || BallTran.localPosition.y < 0)
{
    SetReward(-10.0f);
    EndEpisode();
}
```
- Ball gets to the target area and reward is applied

```code 
if (Vector3.Distance(BallTran.localPosition, GoalTran.localPosition) < 4f)
{
    SetReward(10.0f);
    EndEpisode();   
}
```

### Algorithm configuration
*Files mentioned in this section are located in FinalLearningSetup directory*

Reinforcment learning algorithm chosen for the learning process was PPO due to it's speed (SAC was also tested but was too slow). PPO used rewards showed above for optimalization. To improve learning process to PPO was added [Curiosity](https://pathak22.github.io/noreward-rl/) which encourages the model to take more varied actions. Apart from that two limitation learning methods were applied to jump start the learning process - Behavioral Cloning and Gail. Both LR methods used [BallCollector.demo](https://github.com/ikolton/ML-BallCollector/blob/main/FinalLearningSetup/BallCollector.demo) file with a few recorded succesful runs controlled by player. Whole configuration can be found in [BallCollectorBCGlowCur.yaml](https://github.com/ikolton/ML-BallCollector/blob/main/FinalLearningSetup/BallCollectorBCGlowCur.yaml) file.

### Learning run

![Zrzut ekranu 2024-06-06 131306](https://github.com/ikolton/ML-BallCollector/assets/96392714/ee56455e-3757-4b81-91d5-15a864ce0eef)
![Zrzut ekranu 2024-06-06 131321](https://github.com/ikolton/ML-BallCollector/assets/96392714/b661cd85-2218-4f44-9f32-619d8ea46c5c)


With 10 being a reward for getting ball to the target area, model getting around 9 means very high success rate with most of the penalty being time penalty.


**Future Prospects**
---

To prevent overfitting some ramdomization of the environment was already introduced. However more complicated training areas such as labirynths with adding to the model sensors to collect data by "looking" could lead to intresting more results. 
