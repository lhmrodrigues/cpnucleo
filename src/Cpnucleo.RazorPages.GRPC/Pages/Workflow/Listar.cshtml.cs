﻿using Cpnucleo.Infra.CrossCutting.Communication.GRPC.Interfaces;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cpnucleo.RazorPages.GRPC.Pages.Workflow
{
    [Authorize]
    public class ListarModel : PageModel
    {
        private readonly ICrudGrpcService<WorkflowViewModel> _workflowGrpcService;

        public ListarModel(ICrudGrpcService<WorkflowViewModel> workflowGrpcService)
        {
            _workflowGrpcService = workflowGrpcService;
        }

        public WorkflowViewModel Workflow { get; set; }

        public IEnumerable<WorkflowViewModel> Lista { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Lista = await _workflowGrpcService.ListarAsync();

            return Page();
        }
    }
}