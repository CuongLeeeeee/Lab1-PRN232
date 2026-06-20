using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Repositories.BusinessModels
{
    public class RefreshTokenBusinessModel
    {
        public int RefreshTokenId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public int UserId { get; set; }

        public UserBusinessModel? User { get; set; }
    }
}
