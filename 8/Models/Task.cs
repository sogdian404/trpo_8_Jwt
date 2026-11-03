using System;
using System.Collections.Generic;

namespace _8.Models;

public partial class Task
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? OwnerLogin { get; set; }

    public bool? IsCompleted { get; set; }
}
