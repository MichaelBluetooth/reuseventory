using AutoMapper;
using ReuseventoryApi.Profiles;

namespace ReuseventoryApi.Test.TestUtils
{
    public class MapperConfigFactory
    {
        public static IMapper GetMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ListingProfile());
                cfg.AddProfile(new UserProfile());

                //It'd be nice if I could do this automatically...
                // List<Profile> profiles = new List<Profile>();
                // foreach (Type type in Assembly.GetAssembly(typeof(Profile)).GetTypes()
                //             .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Profile))))
                // {
                //     cfg.AddProfile((Profile)Activator.CreateInstance(type));
                // }
            });
            return new Mapper(config);
        }
    }
}