using System;
using Assets.Buildings.Interfaces;
using UnityEngine;

namespace Assets.Buildings
{
    [Serializable]
    public class Building : ISerializationCallbackReceiver, IDebugable
    {
        [SerializeField]
        public string Id;
        [SerializeField]
        private Vector3 _position;

        public DateTime ConstructionTime { get; set; }
        [SerializeField]
        private long _constructionTimeSerialized;

        public Vector3 Position { get { return _position; } }

        public Building(string id, Vector3 position, DateTime constructionTime)
        {
            Id = id;
            _position = position;
            ConstructionTime = constructionTime;
        }

        protected virtual Sprite GetSprite()
        {
            return null;
        }

        public virtual string GetDebugData()
        {
            return "";
        }

        public virtual void OnBeforeSerialize()
        {
            try
            {
                _constructionTimeSerialized = ConstructionTime.ToFileTimeUtc();
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        public virtual void OnAfterDeserialize()
        {
            ConstructionTime = DateTime.FromFileTimeUtc(_constructionTimeSerialized);
        }
    }
}
