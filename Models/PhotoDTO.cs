﻿namespace DatingApp_API.Models
{
    public class PhotoDTO
    {
        public int Id { get; set; } 
        public string? Url { get; set; }
        public bool? IsMain { get; set; }
    }
}