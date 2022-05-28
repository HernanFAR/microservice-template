using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Answers.RazorViews.ViewModels
{
    public record AnswerNotificationViewModel( 
        string Answer, DateTime AnswerDate, string AnswerCreatorName, 
        Guid QuestionId, string Question, DateTime QuestionDate, string QuestionCreatorName);
}
