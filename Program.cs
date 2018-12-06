using System;

namespace Mastermind
{
    class Queue
    {
        public int front = -1;
        public int back = -1;
        public int length = 0;
        public int[][] data = new int[Mastermind.maxGuessesToDisplay][];
    }


    class Mastermind
    {
        //=======================================================//
        //                   Game attributes                     //
        //=======================================================//

        // Amount of latest guesses stored in history
        static public int maxGuessesToDisplay = 5;

        // Length of each guess history element 
        // (length of secret code plus 2 to store result of the guess)
        static public int guessLength;

        // Amount of guesses which depends on secret code length
        // and maximum number that can be used in code
        static public int maxGuesses;

        // Length of secret code
        static public int secretCodeLength;

        // Upper limit of the numbers used in the secret code
        private int maxNumber;

        // Secret code for user to guess
        private int[] secretCode;

        // Guess of the user
        private int[] userGuess;

        // History of user guesses
        private Queue history;

        // Amount of occurences of every number in secret code
        // Used when counting white pegs
        private int[] numberOccurences;

        // Array used to count amount of the numbers that were not guessed by the user
        private int[] numbersNotGuessed;

        // Current number of guesses made by the user
        private int numberOfGuesses;

        // State of the game
        private bool gameWon = false;

        // Random number generator
        private Random numberGenerator = new Random();


        //=======================================================//
        //                    Game methods                       //
        //=======================================================//

        // Start the program
        static void Main(string[] args)
        {
            Mastermind mastermind = new Mastermind();
            mastermind.StartGame();

            Console.WriteLine("End of the game. Press any key to end.");
            Console.ReadKey();
        }

        /// <summary>
        /// Configures and starts the game
        /// </summary>
        public void StartGame()
        {
            // Configure the game
            GetConfiguration();

            // Loop to determine if user wants to play again
            do
            {
                // Reset the game
                InitialiseSettings();

                // Initialise secret code for user to guess
                GenerateSecretCode();

                // Logic of the game
                GameLoop();

            } while (PlayAgain());
        }


        /// <summary>
        /// Gets code length and range of numbers used from the user
        /// </summary>
        private void GetConfiguration()
        {
            // Input validation
            do
            {
                // Get code length from the user
                Console.Write("Set the secret code length (from 3 to 6): ");

                // Exception handling to prevent crashing on wrong input
                try
                {
                    secretCodeLength = Int32.Parse(Console.ReadLine());
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Wrong input. Try again.");
                }

            } while (secretCodeLength < 3 || secretCodeLength > 6);

            do
            {
                // Get number range from the user
                Console.Write("Set the max number used in code (from 3 to 9): ");

                try
                {
                    maxNumber = Int32.Parse(Console.ReadLine());
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Wrong input. Try again.");
                }

            } while (maxNumber < 3 || maxNumber > 9);

            // Formula to calculate the amount of guesses available to the user
            maxGuesses = (int)((maxNumber + 1) / 2 + secretCodeLength / 2) * 2;

            // Length of every individual guess history array: length of code plus results (black and white)
            guessLength = secretCodeLength + 2;

            Console.Clear();
        }

        /// <summary>
        /// Resets game to the initial state
        /// </summary>
        private void InitialiseSettings()
        {
            numberOfGuesses = 0;

            // Create new arrays that hold amount of numbers used in code
            // and amount of numbers not guessed correctly
            numberOccurences = new int[maxNumber + 1];
            numbersNotGuessed = new int[maxNumber + 1];

            // Create history queue
            history = new Queue();
        }


        /// <summary>
        /// Generates random sequence of numbers
        /// </summary>
        private void GenerateSecretCode()
        {
            secretCode = new int[secretCodeLength];

            for (int i = 0; i < secretCodeLength; i++)
            {
                // Fill the secret code array with random numbers from 0 to max number set by user
                secretCode[i] = numberGenerator.Next(0, maxNumber);

                // Count amount of occurences of every number in secret code
                numberOccurences[secretCode[i]] += 1;
            }
        }

        /// <summary>
        /// Asks user to guess the secret code and displays the results
        /// until he either guesses it correctly or runs out of tries
        /// </summary>
        private void GameLoop()
        {
            int black = 0;
            int white = 0;
            int incorrectGuesses;

            // Get the user input and compare it to secret code
            // End the loop if guessed correctly
            do
            {
                // Check if user have not ran out of guesses
                if (numberOfGuesses < maxGuesses)
                {
                    incorrectGuesses = 0;

                    DisplayPreviousGuesses();

                    GetUserGuess();

                    // Get the amount of black pegs
                    black = GetCorrectGuesses();

                    // Sum the amount of numbers that were not guessed
                    for (int i = 0; i <= maxNumber; i++)
                    {
                        incorrectGuesses += numbersNotGuessed[i];
                    }

                    // Calculate the amount of white pegs
                    white = secretCodeLength - incorrectGuesses - black;

                    Console.Clear();
                    // Display the result of the guess to the user
                    Console.WriteLine("Result - Black: {0}, White: {1}\n", black, white);

                    // Update history queue
                    AddGuessToHistory(black, white);

                    // Check if the user have guessed correctly
                    if (black == secretCodeLength)
                    {
                        gameWon = true;
                    }

                    // Reset the values
                    black = 0;
                    white = 0;

                    numberOfGuesses++;
                }
                // Break out of the loop if user has ran out of guesses
                else
                {
                    break;
                }
            } while (gameWon == false);

            // Check if user has won the game or ran out of guesses
            if (gameWon == true)
            {
                Console.WriteLine("Congratulations, you won!\n");
                DisplayPreviousGuesses();
            }
            else
            {
                Console.WriteLine("You ran out of guesses. Game over.");
                Console.WriteLine("The code was: \n");

                for (int i = 0; i < secretCodeLength; i++)
                {
                    Console.Write(secretCode[i] + " ");
                }

                Console.WriteLine();
            }
        }


        /// <summary>
        /// Gets user guess combination
        /// </summary>
        private void GetUserGuess()
        {
            userGuess = new int[secretCodeLength];

            string[] textMessages = { "first", "second", "third", "fourth", "fifth", "sixth" };

            // Get the guess from the user
            Console.WriteLine("\nEnter your guess (number range from 0 (blank) to {0})", maxNumber);

            for (int i = 0; i < secretCodeLength; i++)
            {
                // Input validation
                do
                {
                    Console.Write("Enter {0} number of the combination: ", textMessages[i]);

                    //Exception handling to prevent program from crashing on wrong input
                    try
                    {
                        userGuess[i] = Int32.Parse(Console.ReadLine());
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Wrong input. Try again.");
                        userGuess[i] = -1;
                    }

                } while (userGuess[i] > maxNumber || userGuess[i] < 0);
            }
        }

        /// <summary>
        /// Get the amount of correctly guessed numbers
        /// </summary>
        private int GetCorrectGuesses()
        {
            int correct = 0;

            // Work on the copy of the array of occurences
            for (int i = 0; i <= maxNumber; i++)
            {
                numbersNotGuessed[i] = numberOccurences[i];
            }

            // Get the amount of correctly guessed numbers
            // Compare the guess to the secret combination
            for (int i = 0; i < secretCodeLength; i++)
            {
                // If the places and number match, increment the amount of correct guesses
                if (userGuess[i] == secretCode[i])
                {
                    correct++;
                }

                // If the number in guess is found in secret code, decrement its occurence amount
                if (numbersNotGuessed[userGuess[i]] > 0)
                {
                    numbersNotGuessed[userGuess[i]]--;
                }
            }

            return correct;
        }

        /// <summary>
        /// Adds recent user guess to history queue
        /// </summary>
        private void AddGuessToHistory(int black, int white)
        {
            int[] temp = new int[guessLength];

            // Copy user guess to temporary array which will be stored in history
            for (int i = 0; i < secretCodeLength; i++)
            {
                temp[i] = userGuess[i];
            }

            // Store result of a guess
            temp[secretCodeLength] = black;
            temp[secretCodeLength + 1] = white;

            // Remove oldest guess when queue is full
            if (IsFull(history))
            {
                Remove(history);
            }
            // Add the most recent guess to guess history queue
            Add(history, temp);
        }


        /// <summary>
        /// Display guess history
        /// </summary>
        private void DisplayPreviousGuesses()
        {
            int counter = 0;
            int i = history.front;

            Console.WriteLine("Latest guesses: ");

            // Display previous guesses
            while (counter < numberOfGuesses && counter < maxGuessesToDisplay)
            {
                for (int j = 0; j < secretCodeLength; j++)
                {
                    Console.Write(history.data[i][j] + " ");
                }

                // Display result of guess
                Console.Write(" B: {0} W: {1}", history.data[i][secretCodeLength], history.data[i][secretCodeLength + 1]);
                Console.WriteLine();

                counter++;
                i++;

                // Wrap around if the index is the last one in the queue
                if (i == Size(history))
                {
                    i = 0;
                }
            }

            Console.WriteLine("\nAmount of guesses made: {0} out of {1}", numberOfGuesses, maxGuesses);
        }


        /// <summary>
        /// Ask user if he wants to play again
        /// </summary>
        private bool PlayAgain()
        {
            bool playAgain = false;

            // Get the input from the user
            Console.Write("\nPress y to play again: ");
            char playerChoice;
            playerChoice = (char)Console.ReadKey().KeyChar;

            // If user want to play again, reset states of the game
            if (playerChoice == 'y')
            {
                playAgain = true;
                gameWon = false;
            }
            Console.Write("\n\n");
            Console.Clear();

            return playAgain;
        }


        //=======================================================//
        //              Queue methods implementation             //
        //=======================================================//

        static int Size(Queue q) { return q.length; }

        static bool IsFull(Queue q) { return (q.length >= maxGuessesToDisplay); }

        static bool IsEmpty(Queue q) { return (q.length <= 0); }

        // Return front element of the queue
        static int[] Front(Queue q) { return q.data[q.front]; }

        // Return back element of the queue
        static int[] Back(Queue q) { return q.data[q.back]; }

        // Add new element to the queue to the back
        static void Add(Queue q, int[] array)
        {
            // Determine if there is space to add another element
            if (IsFull(q))
            {
                Console.WriteLine("Error: queue is full!");
            }
            else
            {
                // If the queue is empty, add the element to index 0
                if (IsEmpty(q))
                {
                    q.front = q.back = 0;
                    q.data[q.front] = array;
                }
                // Otherwise add element to next index available
                // Wrap around if the next index is last one
                else
                {
                    q.back = q.back + 1;

                    if (q.back == maxGuessesToDisplay)
                    {
                        q.back = 0;
                    }

                    q.data[q.back] = array;
                }

                q.length++;
            }
        }

        // Remove queue element from the front
        static int[] Remove(Queue q)
        {
            // Determine if there is any element to remove
            if (IsEmpty(q))
            {
                Console.WriteLine("Error: queue is empty");
                return null;
            }
            else
            {
                // Save current front element to another array
                int[] temp = q.data[q.front];
                q.front = q.front + 1;

                // Wrap around if necessary
                if (q.front == maxGuessesToDisplay)
                {
                    q.front = 0;
                }

                q.length--;

                // Return saved element
                return temp;
            }
        }
    }
}
