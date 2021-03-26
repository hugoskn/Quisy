using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quisy.SqlDbRepository.Entities
{
    [Table("Logs")]
    public class LogsEntity
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public enum LogType
    {
        Warning, Error, Exception, Information
    }
}
