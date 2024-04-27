using Model;
using System.Collections;
using System.Collections.Generic;
using UnitBrains.Pathfinding;
using UnitBrains.Player;
using UnityEditor;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    enum state
    {
        move,
        attack
    }

    public override string TargetUnitName => "Ironclad Behemoth";
    private state unitState = state.move;
    private float transitionTime = 1f;
    private bool stateChange = true;

    public override Vector2Int GetNextStep()
    {
        Vector2Int position = base.GetNextStep();
        bool checkTrue = (position == unit.Pos);

        if (checkTrue)
            unitState = state.attack;
        else
            unitState = state.move;

        stateChange = checkTrue;
        return stateChange ? unit.Pos : position;
    }

    public override void Update(float deltaTime, float time)
    {
        {
            if (stateChange)
            {
                transitionTime -= deltaTime;

                if (transitionTime <= 0)
                {
                    transitionTime = 1f;
                    stateChange = false;
                }
            }
            base.Update(deltaTime, time);
        }
    }

    protected override List<Vector2Int> SelectTargets()
    {
        if (stateChange)
            return new List<Vector2Int>();
        if (unitState == state.attack)
            return base.SelectTargets();
        return new List<Vector2Int>();
    }
}
