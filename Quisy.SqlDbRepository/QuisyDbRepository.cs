using Quisy.SqlDbRepository.Entities;
using System;
using System.Threading.Tasks;

namespace Quisy.SqlDbRepository
{
    public class QuisyDbRepository
    {
        public static async Task<int> AddLogAsync(LogType type, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return 0;

            var logEntity = new LogsEntity { Message = message, Type = type.ToString(), CreatedDate = DateTime.UtcNow };

            try
            {
                using (var ctx = new QuisyDbContext())
                {
                    var entity = ctx.Logs.Add(logEntity);
                    return await ctx.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
    }
}
