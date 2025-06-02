using System;
using System.Collections.Generic;

namespace StudentPortalDBFirst.Models;

public partial class UsersMaster
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? PasswordHash { get; set; }
    

}
