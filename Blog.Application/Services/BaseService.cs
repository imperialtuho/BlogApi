using Blog.Domain.Common;
using Blog.Domain.Extensions;
using Blog.Domain.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Blog.Application.Services
{
    public class BaseService
    {
        private UserSession? _userSession;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected int? TenantIdentify => _httpContextAccessor.GetTenantIdentify();
        public int? TenantId => LoginSession?.TenantId ?? TenantIdentify;

        public UserSession? LoginSession
        {
            get => _userSession ?? _httpContextAccessor?.GetUserSession();
            set
            {
                _userSession = value;
            }
        }

        protected static bool IsActionPerformByAdmin(UserSession? currentUser = null)
        {
            if (currentUser is null)
            {
                return false;
            }

            if (currentUser.Roles is not null && (currentUser.Roles.Exists(r => r.Contains(ApplicationDefaultRoleValue.SuperAdmin) || r.Contains(ApplicationDefaultRoleValue.Admin))))
            {
                return true;
            }

            return false;
        }
    }
}