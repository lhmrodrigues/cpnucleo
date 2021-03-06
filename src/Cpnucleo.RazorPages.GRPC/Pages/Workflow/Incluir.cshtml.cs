﻿using Cpnucleo.Infra.CrossCutting.Communication.GRPC.Interfaces;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Cpnucleo.RazorPages.GRPC.Pages.Workflow
{
    [Authorize]
    public class IncluirModel : PageModel
    {
        private readonly ICrudGrpcService<WorkflowViewModel> _workflowGrpcService;

        public IncluirModel(ICrudGrpcService<WorkflowViewModel> workflowGrpcService)
        {
            _workflowGrpcService = workflowGrpcService;
        }

        [BindProperty]
        public WorkflowViewModel Workflow { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _workflowGrpcService.IncluirAsync(Workflow);

            return RedirectToPage("Listar");
        }
    }
}