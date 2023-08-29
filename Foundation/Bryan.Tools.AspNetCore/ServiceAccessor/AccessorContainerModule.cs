namespace Tools.AspNetCore
{
    public class AccessorContainerModule
    {
        public static TService GetService<TService>() where TService : class
        {
            return typeof(TService).GetService() as TService;
        }
    }
}
