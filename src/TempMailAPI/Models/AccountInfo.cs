using System;

namespace SmorcIRL.TempMail.Models
{
    public class AccountInfo
    {
        public string Id { get; set; }

        public string Address { get; set; }

        public int Quota { get; set; }

        public int Used { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}