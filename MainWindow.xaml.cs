using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BlackjackWPF
{
    public partial class MainWindow : Window
    {
        // karty
        private List<string> deck;

        // player
        private List<string> playerHand;

        // Dealer
        private List<string> dealerHand;

        // Random Number Generator
        private Random rng = new Random();

        public MainWindow()
        {
            InitializeComponent(); // Inicjalizcja okna
        }


        private void StartGame_Click(object sender, RoutedEventArgs e) // Funkcja na przycisk Start
        {
            // Nowa talia kart
            deck = CreateDeck();
            playerHand = new List<string>();
            dealerHand = new List<string>();

            // Odnawiamy status gry
            PlayerPanel.Children.Clear();
            DealerPanel.Children.Clear();
            GameStatus.Text = "";
            PlayerScore.Text = "";
            DealerScore.Text = "";

            // Pokazanie przycisków gry
            HitButton.Visibility = Visibility.Visible;
            StandButton.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Collapsed;
            ExitButton.Visibility = Visibility.Collapsed;

            // Dwie karty dla gracza
            playerHand.Add(DealCard(PlayerPanel));
            playerHand.Add(DealCard(PlayerPanel));

            // Dwie karty dla dealera
            dealerHand.Add(DealCard(DealerPanel));
            dealerHand.Add(DealCard(DealerPanel));

            // Aktualizacja punktów
            UpdateScores();

            int playerScore = GetHandValue(playerHand); // Zmienne z punktami
            int dealerScore = GetHandValue(dealerHand);

            // Sprawdzamy zwycięstwo lub przegraną przy blackjacku
            if (playerScore == 21 && dealerScore == 21)
            {
                EndGame("Draw");
            }
            else if (playerScore == 21)
            {
                EndGame("Blackjack Win");
            }
            else if (dealerScore == 21)
            {
                EndGame("Blackjack Lose");
            }
        }

        private void Hit_Click(object sender, RoutedEventArgs e) // Funkcja na przycisk Hit - dobranie karty
        {
            playerHand.Add(DealCard(PlayerPanel)); // Dodanie karty do ręki
            UpdateScores(); // Aktualizacja punktów

            int playerScore = GetHandValue(playerHand);

            if (playerScore == 21) // Blackjack gracza
            {
                EndGame("Blackjack Win");
            }
            else if (playerScore > 21) // Więcej niż 21 — przegrana
            {
                EndGame("Lose");
            }
        }

        private void Stand_Click(object sender, RoutedEventArgs e) // Funkcja na przycisk Stand - zakończenie tury gracza
        {
            while (GetHandValue(dealerHand) < 17) // Dealer dobiera karty, dopóki jego suma punktów jest mniejsza niż 17
            {
                dealerHand.Add(DealCard(DealerPanel));
            }

            UpdateScores(); // Aktualizacja punktów

            int playerScore = GetHandValue(playerHand); // Zmienne z punktami
            int dealerScore = GetHandValue(dealerHand);


            // Sprawdzanie końcowego wyniku
            if (dealerScore > 21 || playerScore > dealerScore)
            {
                EndGame("Win");
            }
            else if (playerScore == 21)
            {
                EndGame("Blackjack Win");
            }
            else if (dealerScore == 21)
            {
                EndGame("Blackjack Lose");
            }
            else if (dealerScore == playerScore)
            {
                EndGame("Draw");
            }
            else
            {
                EndGame("Lose");
            }
        }

        private void EndGame(string message) // Funkcja końca gry
        {
            GameStatus.Text = message;
            HitButton.Visibility = Visibility.Collapsed;
            StandButton.Visibility = Visibility.Collapsed;
            StartButton.Content = "Play again";
            StartButton.Visibility = Visibility.Visible;
            ExitButton.Visibility = Visibility.Visible;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) // Funkcja na przycisk Exit - zamknięcie gry
        {
            Application.Current.Shutdown(); // Zamykanie programu
        }

        private void UpdateScores() // Funkcja aktualizacji punktów gracza i dealera
        {
            PlayerScore.Text = $"Score: {GetHandValue(playerHand)}";
            DealerScore.Text = $"Score: {GetHandValue(dealerHand)}";
        }

        private string DealCard(StackPanel panel) // Funkcja wizualizacji kart
        {
            if (deck.Count == 0)
                deck = CreateDeck(); // Ponowne tworzenie talii, jeśli skończyły się karty

            string card = deck[rng.Next(deck.Count)]; // Losowa karta
            deck.Remove(card); // Zapobieganie powtórzenia karty

            Image img = new Image // Obraz karty
            {
                Width = 133,
                Height = 400,
                Margin = new Thickness(2), // Marginesy
                Source = new BitmapImage(new Uri($"C:/Projects/C#/BlackJack/BlackJack/cards/{card}.png")) // Ścieżka do obrazów kart
            };

            panel.Children.Add(img); // Dodajemy obraz karty do panelu
            return card;
        }


        private List<string> CreateDeck() // Funkcja tworzenia talii kart
        {
            string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "j", "q", "k", "1" }; // wartości kart
            List<string> newDeck = new List<string>(); // Nowa talia kart

            foreach (var v in values)
            {
                newDeck.Add(v); 
            }

            return newDeck; // Zwracamy nową talię kart
        }

        private int GetHandValue(List<string> hand) // Funkcja obliczania wartości ręki gracza lub dealera
        {
            int total = 0; // Suma punktów w ręce
            int ace = 0; // Ilość asów w ręce

            foreach (var card in hand) // Iteracja przez karty w ręce
            {
                if (card == "j" || card == "q" || card == "k") // Karty o wartości 10
                    total += 10;
                else if (card == "1") // As traktowany jako 11
                {
                    total += 11;
                    ace++;
                }
                else if (int.TryParse(card, out int cardValue)) // Konwertowanie wartości karty
                    total += cardValue;
                else
                    throw new FormatException($"Invalid card value: {card}"); // Błąd karty
            }

            while (total > 21 && ace > 0) // Jeśli suma przekracza 21 i mamy asy, traktujemy asa jako 1
            {
                total -= 10;
                ace--;
            }

            return total; // Zwracamy sumę punktów
        }
    }
}
