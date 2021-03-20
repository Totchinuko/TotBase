using UnityEditor;
using System.Reflection;

namespace TotBaseEditor
{
    public class PCMMenuItem : PCMMenuElement
    {
        private MethodInfo info;

        public PCMMenuItem(MethodInfo info)  {
            this.info = info;
            MenuItem mi = info.GetCustomAttributes(typeof(MenuItem), false)[0] as MenuItem;
            string[] menuPath = mi.menuItem.Split('/');
            name = $"{menuPath[menuPath.Length - 2]}/{menuPath[menuPath.Length - 1]}";
        }

        public override void Execute()
        {
            info.Invoke(null, null);
        }
    }
}