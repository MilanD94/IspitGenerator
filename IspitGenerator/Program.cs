using IspitGenerator;
using Newtonsoft.Json;

bool repeatExam = true;
while (repeatExam)
{
    Console.Clear();
    string examName = SelectExam();

    var selectedQuestions = GetQuestions(examName);

    var userAnswers = RunExam(selectedQuestions);

    ShowExamResults(userAnswers);

    Console.WriteLine("\nPritisni 1 da odes na pocetak.");
    Console.WriteLine("\nPritisni bilo koji drugi taster da ugasis program.");
    int shouldRepeatUserInput = int.Parse(Console.ReadKey().KeyChar.ToString());
    if (shouldRepeatUserInput != 1)
    {
        repeatExam = false;
    }
}

static string SelectExam()
{
    Console.WriteLine("Odaberi test koji zelis da radis:");
    Console.WriteLine("1. Poslovna Ekonomija");
    Console.WriteLine("2. Sociologija");
    int selectedTest = int.Parse(Console.ReadKey().KeyChar.ToString());
    string examName = string.Empty;
    switch (selectedTest)
    {
        case 1:
            examName = "PoslovnaEkonomija";
            break;
        case 2:
            examName = "Sociologija";
            break;
        default:
            break;
    }

    return examName;
}

static List<ExamQuestion> GetQuestions(string testName)
{
    Console.Clear();
    Console.WriteLine("Koliko pitanja zelis da imas na testu?");
    var questionsAmount = int.Parse(Console.ReadLine().ToString());
    string jsonFilePath = $"{testName}.json";

    string jsonData = File.ReadAllText(jsonFilePath);
    ExamQuestions? allQuestions = JsonConvert.DeserializeObject<ExamQuestions>(jsonData);

    Random random = new Random();
    List<ExamQuestion> selectedQuestions = allQuestions.Questions
        .OrderBy(x => random.Next())
        .Take(questionsAmount)
        .Select(q =>
        {
            q.Answers = [.. q.Answers.OrderBy(a => random.Next())];
            return q;
        })
        .ToList();

    return selectedQuestions;
}

static Answers RunExam(List<ExamQuestion> selectedQuestions)
{
    var userAnswers = new Answers();
    var numberOfQuestions = selectedQuestions.Count;

    for (int i = 0; i < numberOfQuestions; i++)
    {
        //SETS THE QUESTION
        ExamQuestion question = SetQuestion(selectedQuestions, numberOfQuestions, i);

        //HANDLES ANSWER
        HandleAnswer(userAnswers, question);
    }

    return userAnswers;
}

static ExamQuestion SetQuestion(List<ExamQuestion> selectedQuestions, int numberOfQuestions, int i)
{
    var question = selectedQuestions[i];
    Console.Clear();
    Console.WriteLine("Pitanje broj {0} od {1}", i + 1, numberOfQuestions);
    Console.WriteLine("Redni broj pitanja u zbirci: {0}\n", question.Id);
    Console.WriteLine(question.Text);
    for (int j = 0; j < question.Answers.Count; j++)
    {
        Console.WriteLine($"{j + 1}. {question.Answers[j].Text}");
    }

    return question;
}

static void HandleAnswer(Answers userAnswers, ExamQuestion question)
{
    int userAnswer = int.Parse(Console.ReadKey().KeyChar.ToString());
    var parsedAnswer = question.Answers[userAnswer - 1].Id;

    if (parsedAnswer != question.CorrectAnswer)
    {
        userAnswers.IncorrectAnswers.Add((question, parsedAnswer));
    }
    else
    {
        userAnswers.CorrectAnswers.Add((question, parsedAnswer));
    }
}

static void ShowExamResults(Answers userAnswers)
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