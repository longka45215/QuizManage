using System;
using System.Collections.Generic;

namespace Project_PRN.Models;

public partial class Subject
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? TeacherId { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Question> Questions { get; } = new List<Question>();
}
