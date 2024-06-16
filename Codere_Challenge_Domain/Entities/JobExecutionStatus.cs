using Codere_Challenge_Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codere_Challenge_Core.Entities
{
    [Table("JobExecutionProcess")]
    public class JobExecutionStatus : BaseEntity
    {
        public JobStatus JobStatus { get; set; }

        public DateTime ExecutionDate { get; set; }

        public int LastIndexProcessed { get; set; }
    }

    public enum JobStatus
    {
        INPROCESS = 0, 
        FINISHED = 1,
        FAILED = 2,
    }
}
