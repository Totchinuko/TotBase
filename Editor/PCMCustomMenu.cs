using System;

namespace TotBaseEditor
{
    public class PCMCustomMenu : PCMMenuElement
    {
        private Action action;
        public PCMCustomMenu(string name, Action action) {
            this.action = action;
            this.name = name;
        }
        public override void Execute()
        {
            action?.Invoke();
        }
    }
}