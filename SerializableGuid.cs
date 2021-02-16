using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TotBase
{
    [Serializable]
    public class SerializableGuid : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string suid;

        private Guid guid;

        public Guid Guid
        {
            get { return guid; }
        }

        public SerializableGuid()
        {
            guid = new Guid();
        }

        public SerializableGuid(Guid guid)
        {
            this.guid = guid;
        }

        // Override the Implicit Conversions between Guid and it's serializable version
        public static implicit operator Guid(SerializableGuid o)
        {
            return o.guid;
        }

        public static implicit operator SerializableGuid(Guid o)
        {
            return new SerializableGuid(o);
        }

        public void OnBeforeSerialize()
        {
            suid = guid.ToString();
        }

        public void OnAfterDeserialize()
        {
            if (suid != null)
                guid = new Guid(suid);
        }
    }
}
