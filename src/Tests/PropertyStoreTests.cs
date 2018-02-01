using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captura.Tests
{
    [TestClass]
    public class PropertyStoreTests
    {
        [TestMethod]
        public void TestPropertyChanged()
        {
            var obj = new FakePropertyStore();

            var propertyName = "Unknown";

            obj.PropertyChanged += (S, E) => propertyName = E.PropertyName;

            const string newValue = "New Value";

            obj.Property = newValue;

            Assert.AreEqual(obj.Property, newValue);
            Assert.AreEqual(propertyName, nameof(obj.Property));
        }

        [TestMethod]
        public void CheckDefaultValue()
        {
            var obj = new FakePropertyStore();
            
            Assert.AreEqual(obj.Property, FakePropertyStore.DefaultPropertyValue);
        }
    }
}