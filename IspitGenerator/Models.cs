using System.ComponentModel.DataAnnotations;

namespace IspitGenerator
{
    public class Answers
    {
        public List<(ExamQuestion question, int userAnswer)> IncorrectAnswers { get; set; } = new List<(ExamQuestion, int)>();
        public List<(ExamQuestion question, int userAnswer)> CorrectAnswers { get; set; } = new List<(ExamQuestion, int)>();
    }

    public class ExamQuestions
    {
        public List<ExamQuestion> Questions { get; set; } = [];
    }

    public class ExamQuestion
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public List<Answer> Answers { get; set; } = [];
        public int CorrectAnswer { get; set; } 
    }

    public class Answer
    {
        [AllowedValues(1, 2, 3, 4)]
        public int Id { get; set; }
        public string? Text { get; set; }
    }
}