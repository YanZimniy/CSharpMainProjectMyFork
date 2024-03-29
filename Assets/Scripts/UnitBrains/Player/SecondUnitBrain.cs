﻿using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private List<Vector2Int> outOfRangeTargets = new List<Vector2Int>();
        private static int counter = 0;
        private int unitCounter;
        private int maxTargets = 3;



        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {

            float overheatTemperature = OverheatTemperature;
            float currenttemp = GetTemperature();

            if (currenttemp >= overheatTemperature)
            {
                return;
            }

            IncreaseTemperature();

            for (int i = 0; i <= currenttemp; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

        }


        protected override List<Vector2Int> SelectTargets()
        {
            Vector2Int enemyBase = runtimeModel.RoMap.Bases[
            IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];

            Vector2Int minTarget = Vector2Int.zero;
            float min = float.MaxValue;

            List<Vector2Int> result = new List<Vector2Int>();
            result.Clear();

            foreach (Vector2Int target in GetAllTargets())
            {

                float DistanceToBase = DistanceToOwnBase(target);

                if (DistanceToBase < min)
                {
                    min = DistanceToBase;
                    minTarget = target;
                }

            }

            result.Add(minTarget);

            if (result.Count == 0)
            {
                result.Add(enemyBase);
            }

            SortByDistanceToOwnBase(result);

            for (int counter = 0; counter < maxTargets; counter++)
            {
                if (min < float.MaxValue)
                {
                    if (IsTargetInRange(minTarget))
                    {
                        result.Add(minTarget);
                    }
                    outOfRangeTargets.Add(minTarget);
                }
            }

            return result;

        }

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep();
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}