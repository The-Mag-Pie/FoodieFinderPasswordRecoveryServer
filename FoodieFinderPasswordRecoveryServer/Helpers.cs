namespace FoodieFinderPasswordRecoveryServer
{
    public class Helpers
    {
        private const int MinutesOfValidity = 15;

        public static bool IsExpired(long createdEpoch)
        {
            var epochDiff = DateTimeOffset.Now.ToUnixTimeSeconds() - createdEpoch;
            if (epochDiff > (MinutesOfValidity * 60))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
