﻿namespace WebAPIAutoresResourceManipulation;

public class AuthorDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<BookDTO> Books { get; set; }
}
