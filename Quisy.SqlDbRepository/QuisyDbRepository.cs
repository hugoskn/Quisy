using Quisy.SqlDbRepository.Entities;
using System;

namespace Quisy.SqlDbRepository
{
    public class QuisyDbRepository
    {
        public static int AddLog(LogType type, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return 0;

            var logEntity = new LogsEntity { Message = message, Type = type, CreatedDate = DateTime.UtcNow };

            try
            {
                using (var ctx = new QuisyDbContext())
                {
                    var entity = ctx.Logs.Add(logEntity);
                    return ctx.SaveChanges();
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
