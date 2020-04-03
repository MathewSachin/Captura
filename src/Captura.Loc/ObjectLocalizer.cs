namespace Captura.Loc
{
    public class ObjectLocalizer<T> : TextLocalizer
    {
        public ObjectLocalizer(T Source, string LocalizationKey) : base(LocalizationKey)
        {
            this.Source = Source;            
        }
        
        public T Source { get; }
    }
}
