using System;

namespace SmorcIRL.TempMail.Models
{
    public class DomainInfo
    {
        public string Id { get; set; }

        public string Domain { get; set; }

        public string IsActive { get; set; }

        public string IsPrivate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}