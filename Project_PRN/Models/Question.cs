using System;
using System.Collections.Generic;

namespace Project_PRN.Models;

public partial class Question
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;
    public string QuestionAnswer { get; set; } = null!;

    public string SubjectId { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public Question()
    {
    }

    public Question(int id, string content, string questionAnswer, string subjectId)
    {
        Id = id;
        Content = content;
        QuestionAnswer = questionAnswer;
        SubjectId = subjectId;
    }
}
