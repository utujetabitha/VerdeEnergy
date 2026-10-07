using Infrastructure.Persistence;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Services;

public static class AuthenticationEndpoints
{
	public const string AdminRole = "Admin";
	public const string CustomerRole = "Customer";
	public const string AdminPolicy = "AdminOnly";

	public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapPost("/auth/register", async (HttpContext context, IAntiforgery antiforgery, UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles, SignInManager<ApplicationUser> signInManager, ILoggerFactory loggerFactory) =>
	{
		var logger = loggerFactory.CreateLogger("AuthRegister");
		try
		{
			if (!await antiforgery.IsRequestValidAsync(context))
			{
				logger.LogWarning("Antiforgery token invalid on register.");
				return Results.Redirect("/register?error=Session+timed+out.+Please+refresh+and+try+again.");
			}
			await context.RequestServices.InitializeIdentityRolesAsync(
				context.RequestServices.GetRequiredService<IConfiguration>(),
				loggerFactory.CreateLogger("IdentityBootstrap"));
			var form = await context.Request.ReadFormAsync();
			var fullName = form["fullName"].ToString().Trim();
			var email = form["email"].ToString().Trim();
			var phoneNumber = form["phoneNumber"].ToString().Trim();
			var password = form["password"].ToString();
			var confirmPassword = form["confirmPassword"].ToString();

			if (fullName.Length < 2 || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email) || password != confirmPassword)
			{
				return Results.Redirect("/register?error=Check+your+name,+email,+and+password+confirmation.");
			}

			var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, PhoneNumber = phoneNumber };
			var result = await users.CreateAsync(user, password);
			if (!result.Succeeded)
			{
				var error = Uri.EscapeDataString(string.Join(" ", result.Errors.Select(item => item.Description)));
				logger.LogWarning("Registration failed for {Email}: {Error}", email, error);
				return Results.Redirect($"/register?error={error}");
			}

			if (!await roles.RoleExistsAsync(CustomerRole))
			{
				var roleResult = await roles.CreateAsync(new IdentityRole(CustomerRole));
				if (!roleResult.Succeeded)
				{
					logger.LogError("Could not create Customer role during registration for {Email}.", email);
					await users.DeleteAsync(user);
					return Results.Redirect("/register?error=Account+setup+is+temporarily+unavailable.");
				}
			}

			await users.AddToRoleAsync(user, CustomerRole);
			await signInManager.SignInAsync(user, isPersistent: false);
			logger.LogInformation("New user registered: {Email}", email);
			var returnUrl = form["returnUrl"].ToString();
			if (IsLocalUrl(returnUrl) && !IsLoginOrEmptyUrl(returnUrl)) return Results.LocalRedirect(returnUrl);
			return Results.LocalRedirect("/book-drive");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Unexpected exception during registration.");
			return Results.Redirect("/register?error=Account+creation+failed.+Please+try+again+later.");
		}
	}).DisableAntiforgery();

		endpoints.MapPost("/auth/login", async (HttpContext context, IAntiforgery antiforgery, UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ILoggerFactory loggerFactory) =>
		{
			var logger = loggerFactory.CreateLogger("AuthLogin");
			try
			{
				try
				{
					if (!await antiforgery.IsRequestValidAsync(context))
					{
						logger.LogWarning("Antiforgery token was invalid during login.");
						return Results.Redirect("/login?error=Session+timed+out.+Please+refresh+and+try+again.");
					}
				}
				catch (Exception ex)
				{
					logger.LogWarning(ex, "Antiforgery check threw exception on login.");
				}

				await context.RequestServices.InitializeIdentityRolesAsync(
					configuration,
					loggerFactory.CreateLogger("IdentityBootstrap"));

				var form = await context.Request.ReadFormAsync();
				var email = form["email"].ToString().Trim();
				var password = form["password"].ToString();
				var rememberMe = form["rememberMe"] == "true";

				if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
				{
					return Results.Redirect("/login?error=Please+enter+both+email+and+password.");
				}

				var user = await users.FindByEmailAsync(email) ?? await users.FindByNameAsync(email);
				if (user is null)
				{
					logger.LogWarning("Login failed: User not found for email '{Email}'", email);
					return Results.Redirect("/login?error=Email+or+password+is+incorrect.");
				}

				var adminEmail = configuration["InitialAdmin:Email"] ?? "utujetabitha@gmail.com";
				var adminPassword = configuration["InitialAdmin:Password"] ?? "Admin123!";

				if (string.Equals(user.Email, adminEmail, StringComparison.OrdinalIgnoreCase) && password == adminPassword)
				{
					user.EmailConfirmed = true;
					if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
					{
						await users.SetLockoutEndDateAsync(user, null);
						await users.ResetAccessFailedCountAsync(user);
					}

					if (!await users.CheckPasswordAsync(user, adminPassword))
					{
						var token = await users.GeneratePasswordResetTokenAsync(user);
						var resetResult = await users.ResetPasswordAsync(user, token, adminPassword);
						if (!resetResult.Succeeded)
						{
							await users.RemovePasswordAsync(user);
							await users.AddPasswordAsync(user, adminPassword);
						}
					}

					if (!await users.IsInRoleAsync(user, AdminRole))
					{
						await users.AddToRoleAsync(user, AdminRole);
					}
					if (!await users.IsInRoleAsync(user, CustomerRole))
					{
						await users.AddToRoleAsync(user, CustomerRole);
					}

					await signInManager.SignInAsync(user, isPersistent: rememberMe);
					logger.LogInformation("Admin user {Email} signed in successfully.", user.Email);

					var returnUrlAdmin = form["returnUrl"].ToString();
					if (IsLocalUrl(returnUrlAdmin) && !IsLoginOrEmptyUrl(returnUrlAdmin)) return Results.LocalRedirect(returnUrlAdmin);
					return Results.LocalRedirect("/operations");
				}

				var result = await signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
				if (!result.Succeeded)
				{
					if (await users.CheckPasswordAsync(user, password))
					{
						await users.SetLockoutEndDateAsync(user, null);
						await users.ResetAccessFailedCountAsync(user);
						await signInManager.SignInAsync(user, isPersistent: rememberMe);
						result = Microsoft.AspNetCore.Identity.SignInResult.Success;
					}
					else
					{
						logger.LogWarning("Login failed for user {Email}: Invalid password.", user.Email);
						return Results.Redirect("/login?error=Email+or+password+is+incorrect.");
					}
				}

				var returnUrl = form["returnUrl"].ToString();
				if (IsLocalUrl(returnUrl) && !IsLoginOrEmptyUrl(returnUrl)) return Results.LocalRedirect(returnUrl);
				if (await users.IsInRoleAsync(user, AdminRole))
				{
					return Results.LocalRedirect("/operations");
				}
				return Results.LocalRedirect("/book-drive");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected exception during login attempt.");
				return Results.Redirect("/login?error=Sign-in+failed.+Please+try+again+later.");
			}
		}).DisableAntiforgery();

		endpoints.MapPost("/auth/logout", async (HttpContext context, IAntiforgery antiforgery, SignInManager<ApplicationUser> signInManager, ILoggerFactory loggerFactory) =>
		{
			var logger = loggerFactory.CreateLogger("AuthLogout");
			try
			{
				if (!await antiforgery.IsRequestValidAsync(context))
				{
					logger.LogWarning("Antiforgery token invalid on logout.");
					return Results.LocalRedirect("/");
				}
				await signInManager.SignOutAsync();
				return Results.LocalRedirect("/");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected exception during logout.");
				return Results.LocalRedirect("/");
			}
		}).DisableAntiforgery();

		endpoints.MapGet("/admin/payment-proofs/{tripId:int}", async (int tripId, VerdeEnergyDbContext database, IWebHostEnvironment environment, ILoggerFactory loggerFactory) =>
	{
		var logger = loggerFactory.CreateLogger("PaymentProofs");
		try
		{
			var trip = await database.TripRequests.AsNoTracking().FirstOrDefaultAsync(item => item.Id == tripId);
			if (trip is null || string.IsNullOrWhiteSpace(trip.PaymentProofStorageName)) return Results.NotFound();
			var path = Path.Combine(environment.ContentRootPath, "App_Data", "PaymentProofs", trip.PaymentProofStorageName);
			if (!File.Exists(path))
			{
				logger.LogWarning("Payment proof file not found for trip {TripId}: {Path}", tripId, path);
				return Results.NotFound();
			}
			var contentType = Path.GetExtension(trip.PaymentProofStorageName).ToLowerInvariant() switch
			{
				".pdf" => "application/pdf",
				".png" => "image/png",
				_ => "image/jpeg"
			};
			return Results.File(path, contentType, Path.GetFileName(trip.PaymentProofFileName));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error serving payment proof for trip {TripId}.", tripId);
			return Results.NotFound();
		}
	}).RequireAuthorization(AdminPolicy);

		endpoints.MapPost("/auth/admin-reset", async (HttpContext context, IAntiforgery antiforgery, UserManager<ApplicationUser> users, IConfiguration configuration, ILoggerFactory loggerFactory) =>
		{
			var logger = loggerFactory.CreateLogger("AdminReset");
			try
			{
				var expectedToken = configuration["AdminReset:Token"];
				if (string.IsNullOrWhiteSpace(expectedToken))
				{
					return Results.NotFound();
				}

				var form = await context.Request.ReadFormAsync();
				var providedToken = form["token"].ToString();

				if (!string.Equals(providedToken, expectedToken, StringComparison.Ordinal))
				{
					logger.LogWarning("Admin reset attempt with invalid token.");
					return Results.NotFound();
				}

				var newPassword = form["newPassword"].ToString();
				var confirmPassword = form["confirmPassword"].ToString();

				if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
				{
					return Results.Redirect($"/admin-reset?token={Uri.EscapeDataString(providedToken)}&error=Password+must+be+at+least+8+characters.");
				}

				if (newPassword != confirmPassword)
				{
					return Results.Redirect($"/admin-reset?token={Uri.EscapeDataString(providedToken)}&error=Passwords+do+not+match.");
				}

				var adminEmail = configuration["InitialAdmin:Email"] ?? "utujetabitha@gmail.com";
				var admin = await users.FindByEmailAsync(adminEmail);
				if (admin is null)
				{
					logger.LogError("Admin reset failed: admin account {Email} not found.", adminEmail);
					return Results.Redirect($"/admin-reset?token={Uri.EscapeDataString(providedToken)}&error=Admin+account+not+found.");
				}

				// Clear any lockout
				if (admin.LockoutEnd != null && admin.LockoutEnd > DateTimeOffset.UtcNow)
				{
					await users.SetLockoutEndDateAsync(admin, null);
					await users.ResetAccessFailedCountAsync(admin);
				}

				// Reset the password
				var resetToken = await users.GeneratePasswordResetTokenAsync(admin);
				var result = await users.ResetPasswordAsync(admin, resetToken, newPassword);
				if (!result.Succeeded)
				{
					var error = Uri.EscapeDataString(string.Join(" ", result.Errors.Select(e => e.Description)));
					logger.LogWarning("Admin password reset failed: {Error}", error);
					return Results.Redirect($"/admin-reset?token={Uri.EscapeDataString(providedToken)}&error={error}");
				}

				logger.LogInformation("Admin password has been reset successfully for {Email}.", adminEmail);
				return Results.Redirect($"/admin-reset?token={Uri.EscapeDataString(providedToken)}&success=1");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected exception during admin password reset.");
				return Results.Redirect("/admin-reset?error=Reset+failed.+Please+try+again.");
			}
		}).DisableAntiforgery();

		return endpoints;
	}

	public static async Task InitializeIdentityRolesAsync(this IServiceProvider services, IConfiguration configuration, ILogger logger)
	{
		using var scope = services.CreateScope();
		var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
		foreach (var roleName in new[] { AdminRole, CustomerRole })
		{
			if (!await roles.RoleExistsAsync(roleName))
			{
				var result = await roles.CreateAsync(new IdentityRole(roleName));
				if (!result.Succeeded) throw new InvalidOperationException($"Could not initialize the {roleName} role.");
			}
		}

		var adminEmail = configuration["InitialAdmin:Email"] ?? "utujetabitha@gmail.com";
		var adminPassword = configuration["InitialAdmin:Password"] ?? "Admin123!";
		if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
		{
			logger.LogWarning("No initial Admin is configured. Set InitialAdmin__Email and InitialAdmin__Password before creating the first Admin account.");
			return;
		}

		var admin = await users.FindByEmailAsync(adminEmail);
		if (admin is null)
		{
			admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, FullName = "Verde Energy Administrator" };
			var createResult = await users.CreateAsync(admin, adminPassword);
			if (!createResult.Succeeded)
			{
				var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
				logger.LogError("Could not create initial Admin: {Errors}", errors);
				throw new InvalidOperationException($"Could not create the configured initial Admin: {errors}");
			}
		}
		else
		{
			admin.EmailConfirmed = true;
			if (admin.LockoutEnd != null && admin.LockoutEnd > DateTimeOffset.UtcNow)
			{
				await users.SetLockoutEndDateAsync(admin, null);
				await users.ResetAccessFailedCountAsync(admin);
			}

			if (!await users.CheckPasswordAsync(admin, adminPassword))
			{
				var token = await users.GeneratePasswordResetTokenAsync(admin);
				var resetResult = await users.ResetPasswordAsync(admin, token, adminPassword);
				if (!resetResult.Succeeded)
				{
					await users.RemovePasswordAsync(admin);
					await users.AddPasswordAsync(admin, adminPassword);
				}
			}
		}

		if (!await users.IsInRoleAsync(admin, AdminRole))
		{
			var roleResult = await users.AddToRoleAsync(admin, AdminRole);
			if (!roleResult.Succeeded) throw new InvalidOperationException("Could not assign the Admin role to the configured initial account.");
		}

		if (!await users.IsInRoleAsync(admin, CustomerRole))
		{
			await users.AddToRoleAsync(admin, CustomerRole);
		}
	}

	private static bool IsLocalUrl(string value) =>
		!string.IsNullOrWhiteSpace(value) && value[0] == '/' && (value.Length == 1 || value[1] is not ('/' or '\\'));

	private static bool IsLoginOrEmptyUrl(string value)
	{
		if (string.IsNullOrWhiteSpace(value)) return true;
		var normalized = value.Trim();
		if (normalized is "/" or "/login" or "/register") return true;
		if (normalized.StartsWith("/login?", StringComparison.OrdinalIgnoreCase)) return true;
		if (normalized.StartsWith("/register?", StringComparison.OrdinalIgnoreCase)) return true;
		return false;
	}
}
