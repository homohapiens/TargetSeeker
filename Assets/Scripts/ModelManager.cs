using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using System.Linq;

public class ModelManager : MonoBehaviour
{
    // 5 Levels
    [Range(0, 4)]
    public int defaultLevel;
    public Transform obstacle;
    
    private float obstacleLength;
    private List<Vector2> lengthRange = new List<Vector2>();
    private List<bool> enableTargetB = new List<bool>();
    private List<Quad> quads = new List<Quad>();
    private int[] quadNums = new int[] { 0, 1, 2, 3 };
 
    private System.Random rnd = new System.Random();

    public struct Axis
    {
        public float min;
        public float max;
        public Axis(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public struct Quad
    {
        public Axis x;
        public Axis z;
        public Quad(Axis x, Axis z)
        {
            this.x = x;
            this.z = z;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Obstacle Length Ranges
        lengthRange.Add(new Vector2(0.01f, 1f)); // level 0
        lengthRange.Add(new Vector2(0.01f, 1f)); // level 1
        lengthRange.Add(new Vector2(1f, 4f));    // level 2
        lengthRange.Add(new Vector2(4f, 8f));    // level 3
        lengthRange.Add(new Vector2(9f, 9f));    // level 4

        enableTargetB.Add(false);  // level 0
        enableTargetB.Add(true);  // level 1
        enableTargetB.Add(true);  // level 2
        enableTargetB.Add(true);  // level 3
        enableTargetB.Add(true);  // level 4

        // Create quadrants
        quads.Add(new Quad(new Axis(-7.5f, -1.5f), new Axis(-7.5f, -1.5f)));
        quads.Add(new Quad(new Axis(-7.5f, -1.5f), new Axis(1.5f, 7.5f)));
        quads.Add(new Quad(new Axis(1.5f, 7.5f), new Axis(1.5f, 7.5f)));
        quads.Add(new Quad(new Axis(1.5f, 7.5f), new Axis(-7.5f, -1.5f)));
    }

    public bool ResetArena(Transform agent, List<Transform> targets)
    {
        int agentLevel = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("agent_level", (float)defaultLevel);
        int[] ranQuadNum = quadNums.OrderBy(x => rnd.Next()).ToArray();

        // Agent poistion
        int I = ranQuadNum[0];
        agent.localPosition = new Vector3(Random.Range(quads[I].x.min, quads[I].x.max), 0, Random.Range(quads[I].z.min, quads[I].z.max));
        agent.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        
        // Goal positions
        I = ranQuadNum[1];
        targets[0].localPosition = new Vector3(Random.Range(quads[I].x.min, quads[I].x.max), 0, Random.Range(quads[I].z.min, quads[I].z.max));
        targets[0].gameObject.SetActive(true);
        I = ranQuadNum[2];
        targets[1].localPosition = new Vector3(Random.Range(quads[I].x.min, quads[I].x.max), 0, Random.Range(quads[I].z.min, quads[I].z.max));
        targets[1].gameObject.SetActive(enableTargetB[agentLevel]);

        // Set obstacle length and rotation
        obstacleLength = Random.Range(lengthRange[agentLevel].x, lengthRange[agentLevel].y);
        obstacle.localScale = new Vector3(0.5f, 1.5f, obstacleLength);
        obstacle.localRotation = Quaternion.Euler(0, Random.Range(0, 2)*90, 0);

        return enableTargetB[agentLevel];
    }

}
