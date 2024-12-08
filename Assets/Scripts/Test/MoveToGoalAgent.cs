using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Test
{
    public class MoveToGoalAgent : Agent
    {
        public override void OnActionReceived(ActionBuffers actions)
        {
            Debug.Log(actions.DiscreteActions[0]);
        }
    }
}