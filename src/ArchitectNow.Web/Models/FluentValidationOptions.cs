using System;
using FluentValidation.AspNetCore;

namespace ArchitectNow.Web.Models
{
    public class FluentValidationOptions
    {
        public Action<FluentValidationMvcConfiguration> Configure;
        public bool Enabled { get; set; }
    }
}