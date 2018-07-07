namespace Captura.Tests
{
    public class FakePropertyStore : PropertyStore
    {
        public const string DefaultPropertyValue = "Default Value";
        
        public string Property
        {
            get => Get(DefaultPropertyValue);
            set => Set(value);
        }
    }
}