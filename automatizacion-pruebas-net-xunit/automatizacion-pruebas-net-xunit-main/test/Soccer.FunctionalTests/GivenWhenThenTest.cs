using System.Threading.Tasks;
using Xunit;

namespace Soccer.FunctionalTests;

public abstract class GivenWhenThenTest
    : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await Given();
        await When();
    }

    public async Task DisposeAsync()
    {
        await Cleanup();
    }

    protected virtual Task Cleanup()
    {
        return Task.CompletedTask;
    }

    protected abstract Task Given();

    protected abstract Task When();
}
