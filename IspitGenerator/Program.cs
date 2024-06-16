using IspitGenerator;
using Newtonsoft.Json;

bool repeatExam = true;
while (repeatExam)
{
    Console.Clear();
    string testName = SelectExam();

    var selectedQuestions = GetQuestions(testName);

    var userAnswers = RunExam(selectedQuestions);

    ProcessAnswers(userAnswers);

    Console.WriteLine("\nPritisni 1 da odes na pocetak.");
    Console.WriteLine("\nPritisni bilo koji drugi taster da ugasis program.");
    int shouldRepeatUserInput = int.Parse(Console.ReadKey().KeyChar.ToString());
    if (shouldRepeatUserInput != 1)
    {
        repeatExam = false;
    }
}

static List<ExamQuestion> GetQuestions(string testName)
{
    Console.Clear();
    string jsonFilePath = $"{testName}.json";

    string jsonData = File.ReadAllText(jsonFilePath);
    ExamQuestions? allQuestions = JsonConvert.DeserializeObject<ExamQuestions>(jsonData);

    Random random = new Random();
    List<ExamQuestion> selectedQuestions = allQuestions.Questions
        .OrderBy(x => random.Next())
        .Take(20)
        .ToList();

    return selectedQuestions;
}

static Answers RunExam(List<ExamQuestion> selectedQuestions)
{
    var userAnswers = new Answers();
    var numberOfQuestions = selectedQuestions.Count;

    for (int i = 0; i < numberOfQuestions; i++)
    {
        var question = selectedQuestions[i];
        Console.Clear();
        Console.WriteLine("Pitanje broj {0} od {1}", i + 1, numberOfQuestions);
        Console.WriteLine("Redni broj pitanja u zbirci: {0}\n", question.Id);
        Console.WriteLine(question.Text);
        foreach (var answer in question.Answers)
        {
            Console.WriteLine($"{answer.Id}. {answer.Text}");
        }

        int userAnswer = int.Parse(Console.ReadKey().KeyChar.ToString());
        if (userAnswer != question.CorrectAnswer)
        {
            userAnswers.IncorrectAnswers.Add((question, userAnswer));
        }
        else
        {
            userAnswers.CorrectAnswers.Add((question, userAnswer));
        }
    }

    return userAnswers;
}

static void ProcessAnswers(Answers userAnswers)
{
    if (userAnswers.IncorrectAnswers.Count == 0)
    {
        Console.Clear();
        Console.WriteLine("Cestitamo. Svi odgovori su tacni.");
    }
    else
    {
        Console.Clear();
        Console.WriteLine("Ukupno netacnih odgovora: {0}", userAnswers.IncorrectAnswers.Count);
        Console.WriteLine("Netacni odgovori:");
        foreach (var incorrectAnswer in userAnswers.IncorrectAnswers)
        {
            var question = incorrectAnswer.question;
            var userAnswer = incorrectAnswer.userAnswer;
            Console.WriteLine($"\nBroj pitanja u zbirci:{question.Id}");
            Console.WriteLine($"Pitanje: {question.Text}");
            if (userAnswer <= question.Answers.Count)
            {
                Console.WriteLine($"Tvoj odgovor: {question.Answers.First(a => a.Id == userAnswer).Text}");
            }
            else
            {
                Console.WriteLine($"Tvoj odgovor: Pogresan unos. Ne postoji odgovor pod rednim brojem {userAnswer}!");
            }
            Console.WriteLine($"Tacan odgovor: {question.Answers.First(a => a.Id == question.CorrectAnswer).Text}");
        }
    }
}

static string SelectExam()
{
    Console.WriteLine("Odaberi test koji zelis da radis:");
    Console.WriteLine("1. Poslovna Ekonomija");
    Console.WriteLine("2. Sociologija");
    int selectedTest = int.Parse(Console.ReadKey().KeyChar.ToString());
    string testName = string.Empty;
    switch (selectedTest)
    {
        case 1:
            testName = "PoslovnaEkonomija";
            break;
        case 2:
            testName = "Sociologija";
            break;
        default:
            break;
    }

    return testName;
}