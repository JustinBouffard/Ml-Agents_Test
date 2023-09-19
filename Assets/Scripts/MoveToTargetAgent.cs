using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Tilemaps;

public class MoveToTargetAgent : Agent
{
    [Tooltip("Movement speed of the agent")]
    [SerializeField]
    private float movementSpeed = 5f;

    [SerializeField] private Transform target;
    [SerializeField] private Transform env;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;

    private void Update()
    {
        Vector3 pos = transform.localPosition;
        pos.z = -0.1f;
        transform.localPosition = pos;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.8f, 1.15f), UnityEngine.Random.Range(-2.5f, 2.5f));
        target.localPosition = new Vector3(UnityEngine.Random.Range(2.5f, 0.5f), UnityEngine.Random.Range(-2.5f, 2.5f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition.normalized);
        sensor.AddObservation((Vector2)target.localPosition.normalized);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        // Always use the local position of the agent 
        transform.localPosition += new Vector3(moveX, moveY, movementSpeed) * Time.deltaTime * movementSpeed;

        AddReward(-1f / MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Target target))
        {
            AddReward(10f);
            backgroundSpriteRenderer.color = Color.green;
            EndEpisode();
        }
        else if(collision.TryGetComponent(out Wall wall))
        {
            AddReward(-2f);
            backgroundSpriteRenderer.color = Color.red;
            EndEpisode();
        }
    }
}
