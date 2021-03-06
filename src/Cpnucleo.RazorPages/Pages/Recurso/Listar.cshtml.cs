﻿using Cpnucleo.Infra.CrossCutting.Communication.API.Interfaces;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cpnucleo.RazorPages.Pages.Recurso
{
    [Authorize]
    public class ListarModel : PageBase
    {
        private readonly IRecursoApiService _recursoApiService;

        public ListarModel(IRecursoApiService recursoApiService)
        {
            _recursoApiService = recursoApiService;
        }

        public RecursoViewModel Recurso { get; set; }

        public IEnumerable<RecursoViewModel> Lista { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Lista = await _recursoApiService.ListarAsync(Token);

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}