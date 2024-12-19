﻿using System.Security.Claims;
using flashcard.model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using flashcard.model.Entities;
using Microsoft.JSInterop;

namespace flashcard.Components.Pages
{
    public partial class Edit : ComponentBase
    {
        [Parameter]
        public string Slug { get; set; } = string.Empty;
        
        private bool deckVisibility = true;
        private bool isSubmitting = false;
        private string tempQuestion = string.Empty;
        private string tempAnswer = string.Empty;
        private List<FlashCard> flashCardProblems = [];
        
        // For editing existing cards
        private string editingQuestion = string.Empty;
        private string editingAnswer = string.Empty;
        private int editingIndex = -1;

        private string deckName
        {
            get => FlashCardService.DeckName;
            set => FlashCardService.DeckName = value;
        }

        private string selectedCategory
        {
            get => FlashCardService.SelectedCategory;
            set => FlashCardService.SelectedCategory = value;
        }

        private string deckDescription
        {
            get => FlashCardService.DeckDescription;
            set => FlashCardService.DeckDescription = value;
        }

        private string? userEmail;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (!authState.User.Identity!.IsAuthenticated)
            {
                Navigation.NavigateTo("/auth/signin");
                return;
            }

            var user = authState.User;
            userEmail = user.FindFirst(ClaimTypes.Email)?.Value;

            var canEdit = await FlashCardService.IsCanEditDeck(userEmail!, Slug);
            if (!canEdit)
            {
                Navigation.NavigateTo("/");
                return;
            }

            // Load existing deck data
            try
            {
                var deck = await FlashCardService.GetDeckBySlug(Slug);
                if (deck == null)
                {
                    Navigation.NavigateTo("/");
                    return;
                }

                // Initialize the form with existing data
                deckName = deck.Title!;
                deckDescription = deck.Description!;
                selectedCategory = deck.Category!;
                deckVisibility = deck.IsPublic;

                // Load flashcard problems
                flashCardProblems = await FlashCardService.GetFlashcardByDeckSlug(deck.Slug!);
                // flashCardProblems = problems.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading deck: {ex}");
                Navigation.NavigateTo("/");
            }
        }

        private void HandleQuestionChange(ChangeEventArgs e)
        {
            tempQuestion = e.Value?.ToString() ?? string.Empty;
        }

        private void HandleAnswerChange(ChangeEventArgs e)
        {
            tempAnswer = e.Value?.ToString() ?? string.Empty;
        }

        private async Task HandleDelete()
        {
            await FlashCardService.DeleteDeck(userEmail!, Slug);
            Navigation.NavigateTo("/");
        }

        private void HandleDiscard()
        {
            tempQuestion = string.Empty;
            tempAnswer = string.Empty;
        }

        private void HandleSave()
        {
            if (!string.IsNullOrWhiteSpace(tempQuestion) && !string.IsNullOrWhiteSpace(tempAnswer))
            {
                AddFlashCardProblem(tempQuestion, tempAnswer);
                tempQuestion = string.Empty;
                tempAnswer = string.Empty;
            }
        }

        private void OpenEditModal(int index)
        {
            editingIndex = index;
            var problem = flashCardProblems[index];
            editingQuestion = problem.Question!;
            editingAnswer = problem.Answer!;
            JSRuntime.InvokeVoidAsync("edit_card_modal.showModal");
        }

        private void HandleEditSave()
        {
            if (editingIndex >= 0 && !string.IsNullOrWhiteSpace(editingQuestion) && !string.IsNullOrWhiteSpace(editingAnswer))
            {
                flashCardProblems[editingIndex] = new FlashCard 
                { 
                    Question = editingQuestion, 
                    Answer = editingAnswer 
                };
                editingIndex = -1;
                editingQuestion = string.Empty;
                editingAnswer = string.Empty;
            }
        }

        private void HandleEditDiscard()
        {
            editingIndex = -1;
            editingQuestion = string.Empty;
            editingAnswer = string.Empty;
        }

        private void AddFlashCardProblem(string question, string answer)
        {
            flashCardProblems.Add(new FlashCard { Question = question, Answer = answer });
        }

        private void DeleteFlashCardProblem(int index)
        {
            flashCardProblems.RemoveAt(index);
        }

        private async Task HandleFinalize()
        {
            if (string.IsNullOrWhiteSpace(deckName) || string.IsNullOrWhiteSpace(selectedCategory) ||
                string.IsNullOrWhiteSpace(deckDescription))
            {
                Console.WriteLine("Please fill all the required fields");
                return;
            }

            if (flashCardProblems.Count == 0)
            {
                Console.WriteLine("Please add at least one flashcard problem");
                return;
            }

            isSubmitting = true;

            try
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                var googleId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(googleId))
                {
                    Console.WriteLine("GoogleId claim is missing or empty.");
                    return;
                }

                var updatedDeck = new DeckBase
                {
                    Title = deckName,
                    Description = deckDescription,
                    Category = selectedCategory,
                    TotalQuestion = flashCardProblems.Count,
                    IsPublic = deckVisibility,
                    GoogleId = googleId,
                };

                await FlashCardService.UpdateFlashCard(Slug, updatedDeck, flashCardProblems);
                Navigation.NavigateTo("/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating deck: {ex}");
            }
            finally
            {
                isSubmitting = false;
            }
        }
    }
}