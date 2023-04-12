using FoodieFinderPasswordRecoveryServer.Database;

namespace FoodieFinderPasswordRecoveryServer.BackgroundTasks
{
    public class DeleteExpiredEntries
    {
        private const int SleepMinutes = 60;

        public static void StartThread(AppDbContext dbContext)
        {
            new Thread(() =>
            {
                Thread.Sleep(15 * 1000);
                Console.WriteLine($"[{DateTime.Now}] {nameof(DeleteExpiredEntries)} thread started.");
                while (true)
                {
                    try
                    {
                        foreach (var entry in dbContext.PasswordRecovery)
                        {
                            if (Helpers.IsExpired(entry.CreatedEpoch))
                            {
                                dbContext.PasswordRecovery.Remove(entry);
                            }
                        }

                        Console.WriteLine($"[{DateTime.Now}] Entries deleted: {dbContext.SaveChanges()}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        Console.WriteLine($"Waiting {SleepMinutes} minutes until next cleaning...");
                        Thread.Sleep(SleepMinutes * 60 * 1000);
                    }
                }
            }).Start();
        }
    }
}
