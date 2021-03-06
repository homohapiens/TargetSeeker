using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTarget : Agent
{
	[SerializeField] private List<Transform> targetTransforms;
	[SerializeField] private MeshRenderer platformMeshRenderer;
	[SerializeField] private Material winMaterial;
	[SerializeField] private Material loseMaterial;
	[SerializeField] private Material defaultMaterial;
	
	private RayPerceptionSensorComponent3D rayPerception;
	private Rigidbody rigidBody;
	private ModelManager modelManager;
	private Camera mainCamera;
	private GameObject agentCameraObject;
	private bool hitA;
	private bool enableTargetB;
	private int targetIdx;

	public float moveSpeed;
	public float rotateSpeed;
	public float wallReward;
	public float goalReward;
	public bool useAgentCamera;

    public void Start()
    {
		rayPerception = GetComponent<RayPerceptionSensorComponent3D>();
		rigidBody = GetComponent<Rigidbody>();
		modelManager = GetComponentInParent<ModelManager>();
		
		mainCamera = Camera.main;
		agentCameraObject = transform.GetChild(0).GetChild(0).gameObject;
		agentCameraObject.SetActive(useAgentCamera);
		mainCamera.enabled = !useAgentCamera;
	}

    public override void OnEpisodeBegin()
    {
		hitA = false;
		targetIdx = 0;
		enableTargetB = modelManager.ResetArena(transform, targetTransforms);
    }

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(transform.localPosition);
		sensor.AddObservation(transform.forward);
		sensor.AddObservation(rigidBody.velocity);
		sensor.AddObservation(targetTransforms[targetIdx].localPosition);

        /* 
        // raycast
		RaycastHit hit;
        float maxHitDist = 10f;
        float hitDist = maxHitDist;
        Color rayColor = Color.yellow;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxHitDist))
        {
            hitDist = hit.distance;
            rayColor = Color.red;
        }
        Debug.DrawRay(transform.position, transform.forward * hitDist, rayColor, Time.deltaTime);
        sensor.AddObservation(hitDist);
		*/
    }

	public override void OnActionReceived(ActionBuffers actions)
	{
		float moveX = actions.ContinuousActions[0];
		float moveZ = actions.ContinuousActions[1];
		float rotateH = actions.ContinuousActions[2];

		Vector3 deltaMove = transform.right * moveX + transform.forward * moveZ;
		rigidBody.MovePosition(transform.position + deltaMove * Time.deltaTime * moveSpeed);
		//transform.localPosition += deltaMove * Time.deltaTime * moveSpeed;

		Quaternion deltaRotate = Quaternion.Euler(Vector3.up * rotateH * rotateSpeed * Time.deltaTime);
		rigidBody.MoveRotation(rigidBody.rotation * deltaRotate);
		//transform.Rotate(Vector3.up * rotateH * rotateSpeed * Time.deltaTime);

		// Add penalty for late
		AddReward(-1f / MaxStep);
	}

	private void OnTriggerEnter(Collider other)
	{
		if ( other.tag == "GoalA")
        {
			hitA = true;
			targetIdx = 1;
			targetTransforms[0].gameObject.SetActive(false);
			AddReward(goalReward);
			if (!enableTargetB)
            {
				platformMeshRenderer.material = winMaterial;
				EndEpisode();
            } 
        } 
		else if (other.tag == "GoalB")
		{
			if (!hitA)
            {
				SetReward(wallReward);
				platformMeshRenderer.material = loseMaterial;
				EndEpisode();
			} 
			else
            {
				AddReward(goalReward);
				platformMeshRenderer.material = winMaterial;
				EndEpisode();
            }

		} 
		else if (other.tag == "Wall")
		{
            SetReward(wallReward);
            platformMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
	}

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
		continuousActions[0] = Input.GetAxisRaw("Horizontal");
		continuousActions[1] = Input.GetAxisRaw("Vertical");
		continuousActions[2] = Input.GetAxis("Mouse X");
    }

}
