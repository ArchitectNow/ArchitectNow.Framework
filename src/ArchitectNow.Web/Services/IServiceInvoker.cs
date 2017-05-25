using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ArchitectNow.Web.Services
{
    public interface IServiceInvoker
    {
        Task<IActionResult> AsyncOk(Func<Task> serviceCall);
        Task<IActionResult> AsyncOk<TResult>(Func<Task<TResult>> serviceCall);
        Task<IActionResult> AsyncOkNoContent(Func<Task> serviceCall);
        Task<IActionResult> AsyncOkNotFound<TResult>(Func<Task<TResult>> serviceCall);
    }
}