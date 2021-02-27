using UnityEngine;
using System;

namespace TotBase
{
    [Serializable]
    public class InputDefinition
    {
        public string name;        
        public KeyCode primaryKey;
        public KeyCode alternatKey;

        public bool GetKeyDown()
        {
            return Input.GetKeyDown(primaryKey) || Input.GetKeyDown(alternatKey);
        }

        public bool GetKey()
        {
            return Input.GetKey(primaryKey) || Input.GetKey(alternatKey);
        }

        public bool GetKeyUp()
        {
            return Input.GetKeyUp(primaryKey) || Input.GetKeyUp(alternatKey);
        }
    }
}
