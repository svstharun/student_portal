using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortalDBFirst.Models;

public partial class Students
{
    [Key]
    public int Id { get; set; }

    public string? StudentName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? CollegeName { get; set; }

    public string? Address { get; set; }

    public string? Gender { get; set; }
}



