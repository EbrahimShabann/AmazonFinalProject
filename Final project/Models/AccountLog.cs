using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_project.Models
{
    public class AccountLog
    {
        [Key]
        public int LogID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        [StringLength(20)]
        [Unicode(false)]
        public string ActionType { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? TimeStamp { get; set; }

        [Column(TypeName = "text")]
        public string AdditionalInfo { get; set; }
    }
}
