namespace TotBaseEditor
{
    public abstract class PCMMenuElement
    {
        public string name {get; protected set;}

        public abstract void Execute();
    }
}