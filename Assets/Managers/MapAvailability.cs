using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Assets.Managers
{
    public class MapAvailability : MonoBehaviour
    {
        public int SizeX;
        public int SizeY;
        private MapFields Map;

        void Start()
        {
            if (Map == null)
            {
                Map = new MapFields();
                for (int y = -SizeY / 2; y < SizeY / 2; y++)
                    for (int x = -SizeX / 2; x < SizeX / 2; x++)
                        Map.Set(x, y, Field.Availability.Free);
            }
        }


        void OnGUI()
        {
            Map.MapName = GUI.TextField(new Rect(20, 20, 100, 20), Map.MapName);
            if (GUI.Button(new Rect(20, 45, 60, 20), "Save"))
                Save();
            if (GUI.Button(new Rect(20, 70, 60, 20), "Load"))
                Load();

            for (int y = -SizeY / 2; y < SizeY / 2; y++)
            {
                for (int x = -SizeX / 2; x < SizeX / 2; x++)
                {
                    var world = new Vector3(x, 0, y);
                    var screen = Camera.main.WorldToScreenPoint(world);
                    var field = Map.Fields.FirstOrDefault(a => a.X == x && a.Y == y);
                    if (field == null)
                        field = Map.Set(x, y, Field.Availability.Free);
                    field.Available =
                    GUI.Toggle(new Rect(screen.x - 10, Screen.height - screen.y - 10, 40, 40),
                        field.Available == Field.Availability.Free, "") ? Field.Availability.Free : Field.Availability.Impossible;
                }
            }
            {

            }
        }

        void Save()
        {
            Managers.Save.SaveMapFields(Map);
        }

        private void Load()
        {
            Map = Game.Map.MapFields;
        }
    }
    [Serializable]
    public class MapFields
    {
        [SerializeField]
        public string MapName = "map0";

        [SerializeField]
        private List<Field> _fields;

        public MapFields()
        {
            _fields = new List<Field>();
        }

        public ReadOnlyCollection<Field> Fields
        {
            get { return _fields.AsReadOnly(); }
        }

        public Field Set(int x, int y, Field.Availability value)
        {
            var field = _fields.FirstOrDefault(a => a.X == x && a.Y == y);
            if (field == null)
                _fields.Add(new Field(value, x, y));
            else
                field.Available = value;
            return field;
        }
        public bool Get(int x, int y)
        {
            var field = _fields.FirstOrDefault(a => a.X == x && a.Y == y);
            if (field != null)
                return field.Available == Field.Availability.Free;
            return false;
        }
    }

    [Serializable]
    public class Field
    {
        public enum Availability
        {
            Free,
            Blocked,
            Impossible
        }

        public Availability Available;
        [SerializeField]
        private int _x;
        [SerializeField]
        private int _y;

        public int X { get { return _x; } }
        public int Y { get { return _y; } }

        public Field(Availability available, int x, int y)
        {
            Available = available;
            _x = x;
            _y = y;
        }
    }
}
