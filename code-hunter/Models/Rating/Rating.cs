using System.Buffers;

namespace code_hunter.Models.Rating
{
    public class QuestionsRating : Question.Question
    {
        public QuestionsRating(Question.Question question)
        {
            Id = question.Id;
            Votes = question.Votes;
            Solved = question.Solved;
            AnswersCount = question.AnswersCount;
            Title = question.Title;
            Description = question.Description;
            CreatedAt = question.CreatedAt;
            UpdatedAt = question.UpdatedAt;
            Useful = question.Useful;
            NotUseful = question.NotUseful;
            UserId = question.UserId;
            Username = question.Username;
        }
    }

    public class AnswersRating : Answer.Answer
    {
        public AnswersRating(Body answer)
        {
            Id = answer.Id;
            Title = answer.Title;
            Description = answer.Description;
            CreatedAt = answer.CreatedAt;
            UpdatedAt = answer.UpdatedAt;
            Useful = answer.Useful;
            NotUseful = answer.NotUseful;
            UserId = answer.UserId;
            Username = answer.Username;
        }
    }
}