using System;
using Microsoft.AspNetCore.Mvc;

namespace ArchitectNow.Web.Services
{
    public interface IExceptionResultBuilder
    {
        ObjectResult Build(Exception exception);
    }
}