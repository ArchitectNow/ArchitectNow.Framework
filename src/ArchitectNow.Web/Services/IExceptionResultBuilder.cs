using System;
using Microsoft.AspNetCore.Mvc;

namespace ArchitectNow.Web.Services
{
    public interface IExceptionResultBuilder
    {
        IActionResult Build(Exception exception);
    }
}