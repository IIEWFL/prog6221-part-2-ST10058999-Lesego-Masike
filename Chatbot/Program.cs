using System;
using System.Media;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
/*
 * References:
 * 1. Microsoft Documentation - C# Programming Guide
 *    URL: https://learn.microsoft.com/en-us/dotnet/csharp/
 *    Description: Official documentation for C# namespaces, classes, and methods, icluding System, System.Media, and System.Collections.Generic .
 * 2. GeekforGeeks - C# Programming Language
 *    URL: https://www.geeksforgeeks.org/c-sharp-programming-language/
 *    Description: Tutorials on C# classes, properties, and collections used in this program.
 * 3. Stack Overflow
 *    URL: https://stackoverflow.com/
 *    Description: Community-driven Q&A site where many C# programming issues are discussed, including error handling and debugging techniques.
 * 4. TutorialsPoint - C# Tutorial
 *    URL: https://www.tutorialspoint.com/csharp/
 *    Description: Guidance on file handling and console applications in C#.
 */
public class Chatbot
{
    private string UserName { get; set; }
    private string FavoriteTopic { get; set; }
    private string LastTopic { get; set; }
    private Random RandomGenerator { get; set; }
    private Dictionary<string, string[]> Responses { get; set; }

    public Chatbot()
    {
        UserName = "User";
        FavoriteTopic = "";
        LastTopic = "";
        RandomGenerator = new Random();
        InitializeResponses();
    }

    private void InitializeResponses()
    {
        Responses = new Dictionary<string, string[]>
        {
            { "password", new string[]
                {
                    "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
                    "A strong password should be over 12 characters long, with letters, numbers, and special characters. Never reuse passwords!",
                    "Consider using a password manager to generate and store complex passwords securely."
                }
            },
            { "scam", new string[]
                {
                    "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organizations.",
                    "Scammers can be very convincing. Always verify the sender’s email and avoid clicking on suspicious links.",
                    "If something seems too good to be true, it might be a scam. Double-check before sharing any sensitive details."
                }
            },
            { "privacy", new string[]
                {
                    "It’s crucial to stay safe online! Use strong privacy settings and be mindful of what you share.",
                    "Protect your privacy by using two-factor authentication and reviewing your account security settings.",
                    "Privacy is key! Avoid sharing personal details on public platforms and use secure connections."
                }
            },
            { "default", new string[]
                {
                    "I’m not sure I understand. Can you try rephrasing?",
                    "I didn’t catch that. Could you explain it another way?",
                    "Hmm, I’m not sure about that. Can you ask differently?"
                }
            }
        };
    }

    public void PlayWelcomeAudio()
    {
        try
        {
            string audioFile = "welcome.wav";
            string fullPath = System.IO.Path.GetFullPath(audioFile);
            Console.WriteLine($"Attempting to play: {fullPath}");

            if (!System.IO.File.Exists(audioFile))
            {
                Console.WriteLine($"Error: {audioFile} not found in the application directory.");
                return;
            }
            SoundPlayer player = new SoundPlayer(audioFile);
            player.Play();
            Console.WriteLine("Audio playback started.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing audio: {ex.Message}\nStack Trace: {ex.StackTrace}");
        }
    }

    public void DisplayLogo()
    {
        try
        {
            Console.WriteLine("Displaying logo.");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string logo = @"
       ==============================
             Cybersecurity Bot
        ==============================
          ___          _ _       
         | _ \__ _ _ _| | | ___  
         |  _/ _` | '_| | |/ _ \ 
         |_| \__,_|_| |_|_|\___/
        ==============================";
            TypeResponse(logo);
            Console.WriteLine("Logo displayed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing logo: {ex.Message}");
        }
    }

    public void GreetUser()
    {
        try
        {
            Console.WriteLine("Starting GreetUser.");
            Console.WriteLine(new string('-', 50));
            TypeResponse("Please enter your name:");
            UserName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(UserName))
                UserName = "User";

            Console.ForegroundColor = ConsoleColor.Green;
            string welcomeMessage = $"Hello, {UserName}! Welcome to the Cybersecurity Awareness Bot!";
            Console.WriteLine("Starting welcome message.");
            PlayWelcomeAudio();
            TypeResponse(welcomeMessage);
            Console.WriteLine("Welcome message completed.");
            TypeResponse("I'm here to help you learn about staying safe online.");
            TypeResponse("What’s your favorite cybersecurity topic? (e.g., password, scam, privacy)");
            FavoriteTopic = Console.ReadLine()?.ToLower().Trim() ?? "";
            if (Responses.ContainsKey(FavoriteTopic))
            {
                TypeResponse($"Great choice, {UserName}! I’ll keep that in mind. Let’s talk about {FavoriteTopic} or anything else!");
            }
            Console.WriteLine("GreetUser completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GreetUser: {ex.Message}\nStack Trace: {ex.StackTrace}");
        }
    }

    private string DetectSentiment(string input)
    {
        input = input.ToLower();
        if (input.Contains("worried") || input.Contains("scared"))
            return "worried";
        if (input.Contains("curious") || input.Contains("wondering"))
            return "curious";
        if (input.Contains("frustrated") || input.Contains("annoyed"))
            return "frustrated";
        return "neutral";
    }

    private string AdjustResponseForSentiment(string response, string sentiment)
    {
        switch (sentiment)
        {
            case "worried":
                return "I understand your concern. " + response + " Let me share some tips to help you stay safe.";
            case "curious":
                return "That’s a great question! " + response + " Want to know more?";
            case "frustrated":
                return "I’m sorry you’re feeling that way. " + response + " Let’s work through this together.";
            default:
                return response;
        }
    }

    public void ProcessUserInput(string input)
    {
        try
        {
            input = input.ToLower().Trim();
            string sentiment = DetectSentiment(input);
            string response;
            string topic = LastTopic;

            if (input == "what can i ask you about?")
            {
                response = "You can ask about password security, scams, privacy, or just chat!";
                TypeResponse(response);
                return;
            }

            if (input.Contains("password"))
                topic = "password";
            else if (input.Contains("scam") || input.Contains("phishing"))
                topic = "scam";
            else if (input.Contains("privacy"))
                topic = "privacy";
            else if (LastTopic != "" && (input.Contains("more") || input.Contains("tell me more") || input.Contains("continue")))
            {
                topic = LastTopic;
            }
            else
            {
                topic = "default";
            }

            LastTopic = topic == "default" ? LastTopic : topic;
            response = Responses[topic][RandomGenerator.Next(Responses[topic].Length)];

            if (FavoriteTopic != "" && topic == FavoriteTopic)
            {
                response = $"{UserName}, since you’re interested in {FavoriteTopic}, here’s a tip: " + response;
            }

            response = AdjustResponseForSentiment(response, sentiment);
            TypeResponse(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing input: {ex.Message}");
            TypeResponse("Something went wrong. Let’s try again!");
        }
    }

    public void TypeResponse(string message)
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(50);
            }
            Console.WriteLine();
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in TypeResponse: {ex.Message}");
        }
    }

    public void Run()
    {
        try
        {
            Console.WriteLine("Starting Run.");
            DisplayLogo();
            GreetUser();

            while (true)
            {
                Console.WriteLine(new string('-', 50));
                TypeResponse("What would you like to know? (Type 'exit' to quit)");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    TypeResponse("Please enter a valid question or command.");
                    continue;
                }

                if (input.ToLower() == "exit")
                    break;

                ProcessUserInput(input);
            }
            Console.WriteLine("Run completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Run: {ex.Message}\nStack Trace: {ex.StackTrace}");
        }
    }

    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Program started.");
            Chatbot bot = new Chatbot();
            bot.Run();
            Console.WriteLine("Program ended.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Main: {ex.Message}\nStack Trace: {ex.StackTrace}");
        }
    }
}
