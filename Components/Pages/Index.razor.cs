﻿using flashcard.model;
using flashcard.model.Entities;
using flashcard.z_dummydata;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using flashcard.utils;

namespace flashcard.Components.Pages
{
	public partial class Index : ComponentBase
	{

		private string searchText = string.Empty;
		private string selectedCategory = string.Empty;
		private List<Flashcard> flashCards = [];
		private List<Flashcard> filteredFlashCards = [];
		private string? userEmail;

		protected override async Task OnInitializedAsync()
		{
			//flashCards = DummyDataCardBasic.GetFlashCards();
			flashCards = await FlashCardService.GetAllFlashCards();
			//Console.WriteLine(flashCards[0].Slug);
            filteredFlashCards = flashCards;
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;

			if (user.Identity?.IsAuthenticated ?? false)
			{
				userEmail = user.FindFirst(ClaimTypes.Email)?.Value;
			}
		}

		private void ApplyFilters()
		{
			var searchTextLower = searchText.ToLowerInvariant();
			var selectedCategoryLower = selectedCategory.ToLowerInvariant();

			filteredFlashCards = flashCards
			.Where(fc => (string.IsNullOrEmpty(searchText) || fc.Title.Contains(searchTextLower, StringComparison.InvariantCultureIgnoreCase)) &&
			(selectedCategory == "all" || string.IsNullOrEmpty(selectedCategory) || fc.Category.ToLowerInvariant().Equals(selectedCategoryLower)))
			.ToList();
		}

		private void HandleSearch(ChangeEventArgs e)
		{
			searchText = e.Value?.ToString() ?? string.Empty;
			ApplyFilters();
		}

		private void SelectCategory(ChangeEventArgs e)
		{
			selectedCategory = e.Value?.ToString() ?? string.Empty;
			ApplyFilters();
		}
	}
}
