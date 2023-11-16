using SoccerQuizApi.Services;

namespace SoccerQuizApi.Helper
{
    public class AdminHelper
    {
        private readonly UserService _usersService;

        public AdminHelper(UserService usersService)
        {
            _usersService = usersService;
        }

        public async Task<bool> NotAdmin(string adminId)
        {
            var user = await _usersService.GetAsync(adminId);

            return user == null || !user.IsAdmin;
        }
    }
}
