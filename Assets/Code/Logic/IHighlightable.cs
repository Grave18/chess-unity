namespace Logic
{
    public interface IHighlightable
    {
        void Highlight();
        void DisableHighlight();

        public bool IsEqual(IHighlightable other);
    }
}
