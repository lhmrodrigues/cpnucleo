﻿using Cpnucleo.Application.Interfaces;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Cpnucleo.API.Controllers.V2
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2")]
    [Authorize]
    public class SistemaController : ControllerBase
    {
        private readonly ICrudAppService<SistemaViewModel> _sistemaAppService;

        public SistemaController(ICrudAppService<SistemaViewModel> sistemaAppService)
        {
            _sistemaAppService = sistemaAppService;
        }

        /// <summary>
        /// Listar sistemas
        /// </summary>
        /// <remarks>
        /// # Listar sistemas
        /// 
        /// Lista sistemas da base de dados.
        /// </remarks>
        /// <param name="getDependencies">Listar dependências do objeto</param>        
        /// <response code="200">Retorna uma lista de sistemas</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<SistemaViewModel> Get(bool getDependencies = false)
        {
            return _sistemaAppService.Listar(getDependencies);
        }

        /// <summary>
        /// Consultar sistema
        /// </summary>
        /// <remarks>
        /// # Consultar sistema
        /// 
        /// Consulta um sistema na base de dados.
        /// </remarks>
        /// <param name="id">Id do sistema</param>        
        /// <response code="200">Retorna um sistema</response>
        /// <response code="404">Sistema não encontrado</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpGet("{id}", Name = "GetSistema")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SistemaViewModel> Get(Guid id)
        {
            SistemaViewModel sistema = _sistemaAppService.Consultar(id);

            if (sistema == null)
            {
                return NotFound();
            }

            return Ok(sistema);
        }

        /// <summary>
        /// Incluir sistema
        /// </summary>
        /// <remarks>
        /// # Incluir sistema
        /// 
        /// Inclui um sistema na base de dados.
        /// 
        /// # Sample request:
        ///
        ///     POST /sistema
        ///     {
        ///        "nome": "Novo sistema",
        ///        "descricao": "Descrição do novo sistema"
        ///     }
        /// </remarks>
        /// <param name="obj">Sistema</param>        
        /// <response code="201">Sistema cadastrado com sucesso</response>
        /// <response code="400">Objetos não preenchidos corretamente</response>
        /// <response code="409">Guid informado já consta na base de dados</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpPost]
        [ProducesResponseType(typeof(SistemaViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<SistemaViewModel> Post([FromBody]SistemaViewModel obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                obj.Id = _sistemaAppService.Incluir(obj);
            }
            catch (Exception)
            {
                if (ObjExists(obj.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSistema", new { id = obj.Id }, obj);
        }

        /// <summary>
        /// Alterar sistema
        /// </summary>
        /// <remarks>
        /// # Alterar sistema
        /// 
        /// Altera um sistema na base de dados.
        /// 
        /// # Sample request:
        ///
        ///     PUT /sistema
        ///     {
        ///        "id": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "nome": "Novo sistema - alterado",
        ///        "descricao": "Descrição do novo sistema - alterado",
        ///        "dataInclusao": "2019-09-21T19:15:23.519Z"
        ///     }
        /// </remarks>
        /// <param name="id">Id do sistema</param>        
        /// <param name="obj">Sistema</param>        
        /// <response code="204">Sistema alterado com sucesso</response>
        /// <response code="400">ID informado não é válido</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Put(Guid id, [FromBody]SistemaViewModel obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != obj.Id)
            {
                return BadRequest();
            }

            try
            {
                _sistemaAppService.Alterar(obj);
            }
            catch (Exception)
            {
                if (!ObjExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Remover sistema
        /// </summary>
        /// <remarks>
        /// # Remover sistema
        /// 
        /// Remove um sistema da base de dados.
        /// </remarks>
        /// <param name="id">Id do sistema</param>        
        /// <response code="204">Sistema removido com sucesso</response>
        /// <response code="404">Sistema não encontrado</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            SistemaViewModel obj = _sistemaAppService.Consultar(id);

            if (obj == null)
            {
                return NotFound();
            }

            _sistemaAppService.Remover(id);

            return NoContent();
        }

        private bool ObjExists(Guid id)
        {
            return _sistemaAppService.Consultar(id) != null;
        }
    }
}
