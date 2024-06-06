using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


//rewards for the agent are from getting closer to the ball and scoring a goal

public class PlayerAgent : Agent
{
    Rigidbody rBody;
    [SerializeField] Material winMaterial;
    [SerializeField] Material loseMaterial;
    [SerializeField] MeshRenderer groundMesh;
    private Transform This;
    public Transform GroundTransform;
    public GameObject Ball;
    public GameObject Goal;

    public Transform BallTran;
    public Transform GoalTran;

    public float maxSpeed = 10;
    public float rotationSpeed = 1;
    public float forceMultiplier = 10;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        This = GetComponent<Transform>();
        BallTran = Ball.GetComponent<Transform>();
        GoalTran = Goal.GetComponent<Transform>();
       
    }

    void FixedUpdate()
    {
        //max speed
        if (rBody.velocity.magnitude > maxSpeed)
        {
            rBody.velocity = rBody.velocity.normalized * maxSpeed;
        }

        //if velocity is too low penalize the agent
        /*if (rBody.velocity.magnitude < 0.1)
        {
            //AddReward(-0.00005f);
            //debug log out reward
            Debug.Log(GetCumulativeReward());
        }*/
        //log out velocity
        //Debug.Log(rBody.velocity);

    }

    void Update()
    {
        //if ball is in the goal area then reward the agent
        //goal is 4 by 4
        //ball is 1 by 1
        //check by distance
        if (Vector3.Distance(BallTran.localPosition, GoalTran.localPosition) < 4f)
        {
            //1st training reward
            SetReward(10.0f);
            //2nd training reward
            //AddReward(10.0f);


            //set win Material
            groundMesh.material = winMaterial;
            //log out reward
            //Debug.Log("Goal");
            //Debug.Log(GetCumulativeReward());

           
            EndEpisode();
           
        }

        //if agent or ball falls off the field then end the episode with penalty
        if (This.localPosition.y < 0 || BallTran.localPosition.y < 0)
        {
            SetReward(-10.0f);
            //set lose Material
            groundMesh.material = loseMaterial;
            //log out reward
            //Debug.Log("Fell off");
            //Debug.Log(GetCumulativeReward());
            EndEpisode();
        }


    }

    


    

    private void RandomizePositiom()
    {
        int option = Random.Range(0, 4);
        if (option == 0)
        {
            BallTran.localPosition = new Vector3(Random.Range(-10f, -4f), 1f, Random.Range(-10f, 10f));
            GoalTran.localPosition = new Vector3(Random.Range(4f, 10f), 1f, Random.Range(-10f, 10f));
        }
        else if(option == 1)
        {
            BallTran.localPosition = new Vector3(Random.Range(4f, 10f), 1f, Random.Range(-10f, 10f)); 
            GoalTran.localPosition = new Vector3(Random.Range(-10f, -4f), 1f, Random.Range(-10f, 10f));
        }
        else if (option == 2)
        {
            BallTran.localPosition = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, -4f));
            GoalTran.localPosition = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(4f, 10f));
        }
        else
        {
            BallTran.localPosition = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(4f, 10f));
            GoalTran.localPosition = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, -4f));
        }
    }


    public override void OnEpisodeBegin()
    {
        //ball and Goal spawn in random position on the field
        RandomizePositiom();
        This.localPosition = new Vector3(0, 1f, 0);        
        //stop ball velocity
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Ball.GetComponent<Rigidbody>().Sleep();
        //zero agent velocity
        rBody.velocity = Vector3.zero;
    }

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

    private float forward;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2, add forward force and rotate
        //forward force
        //float
        forward = actionBuffers.ContinuousActions[0];
        //rotate
        float rotate = actionBuffers.ContinuousActions[1];

        rBody.AddForce(transform.forward * forward * forceMultiplier);


        //alternative movement based on position and not force
        //move game object forward
        //transform.position += transform.forward * forward * forceMultiplier * Time.deltaTime;



        //rotate game object local y axis
        transform.Rotate(transform.up * rotate * rotationSpeed);



        //Reward for getting closer to the ball
        //AddReward(-0.00003f * Vector3.Distance(This.localPosition, BallTran.localPosition));


        /*//Penalty for not moving
        //Debug.Log(forward);
        if (forward >= 0.2 && forward <= -0.2)
        {
            AddReward(-0.0001f);
        }*/

        //reward for moving based on forward velocity
        if(rBody.velocity.magnitude > 0.1f && forward > 0.05f)
        {
            //AddReward(0.0025f);
            //changed reward
            AddReward(0.001f);
        }

        //penalty for time
        if(GetCumulativeReward() >= -10f)
        AddReward(-0.002f);

        //log out reward
        //Debug.Log(GetCumulativeReward());


    }



    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        //forward
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        //rotate
        continuousActionsOut[1] = Input.GetAxis("Horizontal");

    }
}
