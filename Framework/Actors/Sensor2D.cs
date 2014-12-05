using CraftingLegends.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CraftingLegends.Framework
{
    public class Sensor2D : MonoBehaviour
    {
        // ================================================================================
        //  Sensor Events
        // --------------------------------------------------------------------------------

        public delegate void SensorEventDelegate(SensorEvent type, IActor actor);
        public event SensorEventDelegate sensorEvent;

        // ================================================================================
        //  public
        // --------------------------------------------------------------------------------

        public string searchTag = "";

        public List<IActor> actors = new List<IActor>();

        public bool hasActorsDetected
        {
            get
            {
                return actors.Count > 0;
            }
        }

        // ================================================================================
        //  private
        // --------------------------------------------------------------------------------

        protected Transform _transform;

        protected IActor _self;

        // ================================================================================
        //  unity methods
        // --------------------------------------------------------------------------------

        void Awake()
        {
            _transform = transform;

            _self = _transform.GetInterface<IActor>();
            if (_self == null && _transform.parent != null)
                _self = _transform.parent.GetInterface<IActor>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (searchTag == "" || other.tag == searchTag)
            {                
                IActor actor = other.transform.GetInterface<IActor>();
                if (actor != null && actor != _self && !actors.Contains(actor) && actor.isAlive)
                {
                    actors.Add(actor);

                    actor.stateChanged += Actor_StateChanged;

                    if (sensorEvent != null)
                        sensorEvent(SensorEvent.ActorDetected, actor);
                }
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (searchTag == "" || other.tag == searchTag)
            {
                IActor actor = other.transform.GetInterface<IActor>();
                Remove(actor);
            }
        }

        // ================================================================================
        //  private methods
        // --------------------------------------------------------------------------------

        void Actor_StateChanged(IActor actor, ActorState state)
        {
            if (!actor.isAlive)
            {
                if (actors.Contains(actor))
                {
                    Remove(actor);
                }
                else
                {
                    actor.stateChanged -= Actor_StateChanged;
                }
            }
        }

        private void Remove(IActor actor)
        {
            if (actor != null && actors.Contains(actor))
            {
                actors.Remove(actor);

                actor.stateChanged -= Actor_StateChanged;

                if (sensorEvent != null)
                    sensorEvent(SensorEvent.ActorLeft, actor);
            }
        }

        private void UpdateList()
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i] == null)
                {
                    actors.RemoveAt(i);
                    i--;
                }
            }
        }

    }
}
