﻿using Si.Engine.Sprite._Superclass;
using Si.GameEngine.AI._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;
namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Finite-state-machine where AI decides the states. "Taunt" keeps an object swooping in and out. Very near and somewhat aggressively.
    /// </summary>
    internal class AIModelHostileEngagement : AIStateMachine
    {
        #region Instance parameters.

        private readonly string _boostResourceName = "AIModelHostileEngagement_Boost";
        private readonly float _idealMaxDistance = SiRandom.Variance(1500, 0.20f);
        private readonly float _idealMinDistance = SiRandom.Variance(400, 0.10f);

        #endregion

        #region AI States.

        private class AIStateApproaching : AIState
        {
            public float VarianceAngle = SiRandom.Variance(1, 0.2f);
        }

        private class AIStateDeparting : AIState
        {
            public float VarianceAngle = SiRandom.Variance(45, 0.2f);
        }

        private class AIStateTransitionToApproach : AIState
        {
            public float VarianceAngle = SiRandom.Variance(1, 0.2f);
            public SiRelativeDirection RotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
        }

        private class AIStateTransitionToDepart : AIState
        {
            public float VarianceAngle = SiRandom.Variance(45, 0.2f);
            public SiRelativeDirection RotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
        }

        private class AIStateTransitionToEvasiveEscape : AIState
        {
            public float VarianceAngle = SiRandom.Variance(45, 0.2f);
            public SiRelativeDirection RotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
            public SiAngle TargetAngle = new();

            public AIStateTransitionToEvasiveEscape(AIStateMachine machine)
            {
                TargetAngle.Degrees = machine.Owner.Velocity.ForwardAngle.Degrees + 180;
            }
        }

        class AIStateEvasiveEscape : AIState
        {
            public float VarianceAngle = SiRandom.Variance(45, 0.2f);
            public SiRelativeDirection RotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
        }

        #endregion

        public AIModelHostileEngagement(EngineCore engine, SpriteShipBase owner, SpriteBase observedObject)
            : base(engine, owner, observedObject)
        {
            owner.OnHit += Owner_OnHit;

            Owner.RenewableResources.Create(_boostResourceName, 800, 0, 10);

            ChangeState(new AIStateDeparting());
            Owner.Velocity.ForwardVelocity = 1.0f;

            OnApplyIntelligence += AIModelHostileEngagement_OnApplyIntelligence;
        }

        private void Owner_OnHit(SpriteBase sender, SiDamageType damageType, int damageAmount)
        {
            if (sender.HullHealth <= 10)
            {
                ChangeState(new AIStateTransitionToEvasiveEscape(this));
            }
        }

        private void AIModelHostileEngagement_OnApplyIntelligence(float epoch, SiPoint displacementVector, AIState state)
        {
            var distanceToObservedObject = Owner.DistanceTo(ObservedObject);

            switch (state)
            {
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                //The object is moving towards the observed object.
                case AIStateApproaching approaching:
                    //Attempt to follow the observed object.
                    Owner.RotateIfNotPointingAt(ObservedObject, 1, approaching.VarianceAngle);

                    if (Owner.DistanceTo(ObservedObject) < _idealMinDistance)
                    {
                        ChangeState(new AIStateTransitionToDepart());
                    }
                    break;
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                //The object is rotating away from the observed object.
                case AIStateTransitionToDepart transitionToDepart:
                    //As we get closer, make the angle more agressive.
                    var rotationRadians = new SiAngle((1 - (distanceToObservedObject / _idealMinDistance)) * 2.0f).Radians;

                    //Rotate as long as we are facing the observed object. If we are no longer facing, then depart.
                    if (Owner.RotateIfPointingAt(ObservedObject, transitionToDepart.RotationDirection, rotationRadians, transitionToDepart.VarianceAngle) == false)
                    {
                        ChangeState(new AIStateDeparting());
                    }

                    //Once we find the correct angle, we go into departing mode.
                    ChangeState(new AIStateDeparting());
                    break;
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                //The object is moving away from the observed object.
                case AIStateDeparting departing:
                    //Onve we are sufficiently far away, we turn back.
                    if (Owner.DistanceTo(ObservedObject) > _idealMaxDistance)
                    {
                        ChangeState(new AIStateTransitionToApproach());
                    }
                    break;
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                //The object is rotating towards the observed object.
                case AIStateTransitionToApproach transitionToApproach:
                    //Once we find the correct angle, we go into approaching mode.
                    if (Owner.RotateIfNotPointingAt(ObservedObject, transitionToApproach.RotationDirection, 1, transitionToApproach.VarianceAngle) == false)
                    {
                        ChangeState(new AIStateApproaching());
                    }
                    break;
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                //The object is rotating agressively away from the observed object.
                case AIStateTransitionToEvasiveEscape transitionToEvasiveEscape:
                    if (Owner.RotateIfNotPointingAt(transitionToEvasiveEscape.TargetAngle.Degrees, transitionToEvasiveEscape.RotationDirection, 1, transitionToEvasiveEscape.VarianceAngle) == false)
                    {
                        ChangeState(new AIStateEvasiveEscape());
                    }
                    break;
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                //The object is quickly moving away from the observed object.
                case AIStateEvasiveEscape evasiveEscape:
                    if (Owner.RenewableResources.Observe(_boostResourceName) > 250)
                    {
                        Owner.Velocity.AvailableBoost = Owner.RenewableResources.Consume(_boostResourceName, SiRandom.Variance(250, 0.5f));
                    }

                    if (distanceToObservedObject > _idealMaxDistance)
                    {
                        //the object got away and is now going to transition back to approach.
                        ChangeState(new AIStateTransitionToApproach());
                    }
                    else if (distanceToObservedObject < _idealMinDistance)
                    {
                        //The observed object got close again, agressively transition away again.
                        ChangeState(new AIStateTransitionToEvasiveEscape(this));
                    }
                    break;
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                default:
                    throw new Exception($"Unknown AI state: {state.GetType()}");
            }
        }
    }
}
