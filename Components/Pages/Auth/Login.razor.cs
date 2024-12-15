﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace flashcard.Components.Pages.Auth
{
	public partial class Login : ComponentBase
	{
		[CascadingParameter]
		public required HttpContext HttpContext { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override async Task OnInitializedAsync()
		{
            if (HttpContext.User.Identity!.IsAuthenticated)
            {
                Console.WriteLine("User is already authenticated");
                Navigation.NavigateTo("/");
                return;
            }

            var authProperties = new AuthenticationProperties
			{
				RedirectUri = "/google/callback",
			};
			var result = TypedResults.Challenge(authProperties, [GoogleDefaults.AuthenticationScheme]);
            await result.ExecuteAsync(HttpContext);
		}
	}
}
