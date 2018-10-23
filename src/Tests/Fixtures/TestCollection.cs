using Xunit;

namespace Captura.Tests.Fixtures
{
    [CollectionDefinition(nameof(Tests))]
    public class TestCollection : ICollectionFixture<TestManagerFixture>, ICollectionFixture<MoqFixture> { }
}