// See https://aka.ms/new-console-template for more information
using BugSouls.FontMapGenerator;

Console.WriteLine("Hello!");
Console.WriteLine("Type: Gen <ttf file> <size> to generate a fontmap!");
Console.WriteLine("Type: 'Quit' to exit!");

//get a command
bool isRunning = true;

while (isRunning)
{
    Console.Write("Enter command > ");
    string[] input = Console.ReadLine().Split(' ');

    switch (input[0].ToLower())
    {
        case "gen":
            int size;
            if (int.TryParse(input[2], out size))
            {
                FontMapGenerator.GenerateFontMap(input[1], size);
            }
            else
            {
                Console.WriteLine("Invalid font size!");
            }    
            break;
        case "quit":
            isRunning = false;
            break;
        default:
            Console.WriteLine("Invalid Command!");
            break;
    }
}