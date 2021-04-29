using System.Collections.Generic;

namespace code_hunter.Models.Question
{
    public class Question : Body
    {
        public int Votes { get; set; }
        public int AnswersCount { get; set; }
        public bool Solved { get; set; }
        public List<Answer.Answer> Answers { get; set; }
    }
}