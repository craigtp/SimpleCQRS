using SimpleCQRS.Core;

namespace SimpleCQRS.Web
{
    public static class ServiceLocator
    {
        public static FakeBus Bus { get; set; }
    }
}
