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
    public class ProjetoController : ControllerBase
    {
        private readonly ICrudAppService<ProjetoViewModel> _projetoAppService;

        public ProjetoController(ICrudAppService<ProjetoViewModel> projetoAppService)
        {
            _projetoAppService = projetoAppService;
        }

        /// <summary>
        /// Listar projetos
        /// </summary>
        /// <remarks>
        /// # Listar projetos
        /// 
        /// Lista projetos da base de dados.
        /// </remarks>
        /// <param name="getDependencies">Listar dependências do objeto</param>        
        /// <response code="200">Retorna uma lista de projetos</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<ProjetoViewModel> Get(bool getDependencies = false)
        {
            return _projetoAppService.Listar(getDependencies);
        }

        /// <summary>
        /// Consultar projeto
        /// </summary>
        /// <remarks>
        /// # Consultar projeto
        /// 
        /// Consulta um projeto na base de dados.
        /// </remarks>
        /// <param name="id">Id do projeto</param>        
        /// <response code="200">Retorna um projeto</response>
        /// <response code="404">Projeto não encontrado</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpGet("{id}", Name = "GetProjeto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProjetoViewModel> Get(Guid id)
        {
            ProjetoViewModel projeto = _projetoAppService.Consultar(id);

            if (projeto == null)
            {
                return NotFound();
            }

            return Ok(projeto);
        }

        /// <summary>
        /// Incluir projeto
        /// </summary>
        /// <remarks>
        /// # Incluir projeto
        /// 
        /// Inclui um projeto na base de dados.
        /// 
        /// # Sample request:
        ///
        ///     POST /projeto
        ///     {
        ///        "nome": "Novo projeto",
        ///        "idSistema": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91"
        ///     }
        /// </remarks>
        /// <param name="obj">Projeto</param>        
        /// <response code="201">Projeto cadastrado com sucesso</response>
        /// <response code="400">Objetos não preenchidos corretamente</response>
        /// <response code="409">Guid informado já consta na base de dados</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProjetoViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<ProjetoViewModel> Post([FromBody]ProjetoViewModel obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                obj.Id = _projetoAppService.Incluir(obj);
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

            return CreatedAtAction("GetProjeto", new { id = obj.Id }, obj);
        }

        /// <summary>
        /// Alterar projeto
        /// </summary>
        /// <remarks>
        /// # Alterar projeto
        /// 
        /// Altera um projeto na base de dados.
        /// 
        /// # Sample request:
        ///
        ///     PUT /projeto
        ///     {
        ///        "id": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "nome": "Novo projeto - alterado",
        ///        "idSistema": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "dataInclusao": "2019-09-21T19:15:23.519Z"
        ///     }
        /// </remarks>
        /// <param name="id">Id do projeto</param>        
        /// <param name="obj">Projeto</param>        
        /// <response code="204">Projeto alterado com sucesso</response>
        /// <response code="400">ID informado não é válido</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Put(Guid id, [FromBody]ProjetoViewModel obj)
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
                _projetoAppService.Alterar(obj);
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
        /// Remover projeto
        /// </summary>
        /// <remarks>
        /// # Remover projeto
        /// 
        /// Remove um projeto da base de dados.
        /// </remarks>
        /// <param name="id">Id do projeto</param>        
        /// <response code="204">Projeto removido com sucesso</response>
        /// <response code="404">Projeto não encontrado</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            ProjetoViewModel obj = _projetoAppService.Consultar(id);

            if (obj == null)
            {
                return NotFound();
            }

            _projetoAppService.Remover(id);

            return NoContent();
        }

        private bool ObjExists(Guid id)
        {
            return _projetoAppService.Consultar(id) != null;
        }
    }
}
