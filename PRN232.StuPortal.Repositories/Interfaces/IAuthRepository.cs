using PRN232.StuPortal.Repositories.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.StuPortal.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserBusinessModel?> GetUserByUsernameAsync(string username);

        Task AddRefreshTokenAsync(RefreshTokenBusinessModel refreshToken);

        Task<RefreshTokenBusinessModel?> GetRefreshTokenByTokenAsync(string token);

        Task RevokeRefreshTokenAsync(string token);
    }
}
