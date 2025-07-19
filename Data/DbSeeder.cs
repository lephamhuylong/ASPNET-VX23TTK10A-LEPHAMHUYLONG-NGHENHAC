using Microsoft.AspNetCore.Identity;

namespace MyMusicApp.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        // Lấy các dịch vụ cần thiết
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Tạo các Role (Admin, User) nếu chúng chưa tồn tại
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Tạo tài khoản Admin mặc định
        string adminEmail = "admin@mymusic.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrator",
                EmailConfirmed = true // Xác thực email luôn cho tài khoản admin
            };
            // Thay "Admin@123" bằng một mật khẩu an toàn hơn và lưu vào User Secrets trong môi trường production
            var result = await userManager.CreateAsync(user, "Admin@123");

            if (result.Succeeded)
            {
                // Gán role "Admin" cho tài khoản vừa tạo
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
