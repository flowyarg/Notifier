namespace Notifier.Blazor.Helpers
{
    public class LazyResolver<T> : Lazy<T> 
        where T : class
    {
        public LazyResolver(IServiceProvider provider)
            : base(provider.GetRequiredService<T>)
        {}
    }
}
