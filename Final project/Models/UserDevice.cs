using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_project.Models
{
    public class UserDevice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string DeviceFingerprint { get; set; }

        [StringLength(200)]
        public string DeviceName { get; set; }

        [StringLength(100)]
        public string DeviceType { get; set; } // "Desktop", "Mobile", "Tablet"

        [StringLength(200)]
        public string Browser { get; set; }

        [StringLength(200)]
        public string OperatingSystem { get; set; }

        // Add the missing UserAgent property
        [StringLength(1000)]
        public string UserAgent { get; set; }

        [StringLength(45)]
        public string IpAddress { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        public bool IsTrusted { get; set; } = false;

        public DateTime FirstSeen { get; set; } = DateTime.UtcNow;

        public DateTime LastSeen { get; set; } = DateTime.UtcNow;

        public DateTime? TrustedAt { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}