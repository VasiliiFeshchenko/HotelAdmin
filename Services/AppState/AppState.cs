namespace HotelAdmin.Services.IsProductionChecker
{
    public class AppState
    {
        public static bool IsProduction
        {
            get
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json")
                  .Build();
                if (configuration["State"] == "Production")
                {
                    return true;
                }
                return false;
            }
        }
    }
}
