using Lamar;

namespace DiscordBot
{
    public static class InversionOfControl
    {
        private static Container container;

        public static Container Container
        {
            get
            {
                return GetOrInitContainer();
            }
        }

        private static Container GetOrInitContainer()
        {
            if(container is null)
            {
                InitializeContainer();
            }

            return container;
        }

        public static void InitializeContainer()
        {
            container = new Container(c =>
            {
                c.For<IConnection>().Use<DiscordConnection>();
                // Add the types you need
                // c.For<YourInterface>().Use<YourConcretion>();
                // c.ForSingletonOf<YourSingletonType>().UseIfNone<YourSingletonType>();
            });
        }
    }
}
