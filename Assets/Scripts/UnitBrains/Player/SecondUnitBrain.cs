using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

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
        List<Vector2Int> DontReachTarget = new List<Vector2Int>();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////  
            ///

            if (GetTemperature() >= overheatTemperature)
            {
                return; 
            }
            
            for (float i = 0 ; i < GetTemperature () ; i++) 
            {
          
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                   
            }
            IncreaseTemperature();


            ///////////////////////////////////////
        }   

        public override Vector2Int GetNextStep()
        {
            Vector2Int target = DontReachTarget[0];
            if (DontReachTarget.Count > 0 && !IsTargetInRange(target))
            {
                return unit.Pos.CalcNextStepTowards(target);
            }
            else
            {
                return unit.Pos;
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            ///

            List<Vector2Int> result = GetReachableTargets();
            result = new List<Vector2Int>();
            DontReachTarget.Clear();

            float minDistance = float.MaxValue;
            Vector2Int nearestTarget = Vector2Int.zero;
           
            foreach (var target in GetAllTargets())
            {
                float distance = DistanceToOwnBase(target);

                if (minDistance >= distance)
                {

                    minDistance = distance;
                    nearestTarget = target;
                }
                result.Add(target);
            }

            if (IsTargetInRange(nearestTarget))
            {
                result.Add(nearestTarget);
            }
            else
            {
                DontReachTarget.Add(nearestTarget);
            }

            if (result.Count == 0 && DontReachTarget.Count == 0)
            {
                
                if (IsTargetInRange(runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId]))
                {
                    result.Add(runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId]);
                    return result;
                }
                DontReachTarget.Add(runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId]);
                return result;
            }
            else
            {
                return result;
            }
        }
                           
        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
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
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}