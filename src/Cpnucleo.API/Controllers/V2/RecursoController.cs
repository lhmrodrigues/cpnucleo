﻿using Cpnucleo.API.Services;
using Cpnucleo.Application.Interfaces;
using Cpnucleo.Infra.CrossCutting.Util.Interfaces;
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
    public class RecursoController : ControllerBase
    {
        private readonly IRecursoAppService _recursoAppService;
        private readonly ISystemConfiguration _systemConfiguration;

        public RecursoController(IRecursoAppService recursoAppService, ISystemConfiguration systemConfiguration)
        {
            _recursoAppService = recursoAppService;
            _systemConfiguration = systemConfiguration;
        }

        /// <summary>
        /// Listar recursos
        /// </summary>
        /// <remarks>
        /// # Listar recursos
        /// 
        /// Lista recursos da base de dados.
        /// </remarks>
        /// <param name="getDependencies">Listar dependências do objeto</param>        
        /// <response code="200">Retorna uma lista de recursos</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public IEnumerable<RecursoViewModel> Get(bool getDependencies = false)
        {
            return _recursoAppService.Listar(getDependencies);
        }

        /// <summary>
        /// Consultar recurso
        /// </summary>
        /// <remarks>
        /// # Consultar recurso
        /// 
        /// Consulta um recurso na base de dados.
        /// </remarks>
        /// <param name="id">Id do recurso</param>        
        /// <response code="200">Retorna um recurso</response>
        /// <response code="404">Recurso não encontrado</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpGet("{id}", Name = "GetRecurso")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public ActionResult<RecursoViewModel> Get(Guid id)
        {
            RecursoViewModel recurso = _recursoAppService.Consultar(id);

            if (recurso == null)
            {
                return NotFound();
            }

            return Ok(recurso);
        }

        /// <summary>
        /// Incluir recurso
        /// </summary>
        /// <remarks>
        /// # Incluir recurso
        /// 
        /// Inclui um recurso na base de dados.
        /// 
        /// # Sample request:
        ///
        ///     POST /recurso
        ///     {
        ///        "nome": "Novo recurso",
        ///        "login": "usuario.teste",
        ///        "senha": "12345678",
        ///        "confirmarSenha": "12345678"
        ///     }
        /// </remarks>
        /// <param name="obj">Recurso</param>        
        /// <response code="201">Recurso cadastrado com sucesso</response>
        /// <response code="400">Objetos não preenchidos corretamente</response>
        /// <response code="409">Guid informado já consta na base de dados</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpPost]
        [ProducesResponseType(typeof(RecursoViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Authorize]
        public ActionResult<RecursoViewModel> Post([FromBody]RecursoViewModel obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                obj.Id = _recursoAppService.Incluir(obj);
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

            return CreatedAtAction("GetRecurso", new { id = obj.Id }, obj);
        }

        /// <summary>
        /// Autenticar recurso
        /// </summary>
        /// <remarks>
        /// # Autenticar recurso
        /// 
        /// Autentica o recurso e devolve um token válido por 60 minutos para utilização na API.
        /// </remarks>
        /// <response code="200">Retorna um recurso</response>
        /// <response code="404">Recurso não encontrado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpGet("Autenticar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<RecursoViewModel> Autenticar(string login, string senha)
        {
            RecursoViewModel recurso = _recursoAppService.Autenticar(login, senha, out bool valido);

            if (recurso == null)
            {
                return NotFound();
            }

            if (!valido)
            {
                return NotFound();
            }
            else
            {
                recurso.Token = TokenService.GenerateToken(recurso.Id.ToString(), _systemConfiguration.JwtKey, _systemConfiguration.JwtIssuer, _systemConfiguration.JwtExpires);

                return Ok(recurso);
            }
        }

        /// <summary>
        /// Alterar recurso
        /// </summary>
        /// <remarks>
        /// # Alterar recurso
        /// 
        /// Altera um recurso na base de dados.
        /// 
        /// # Sample request:
        ///
        ///     PUT /recurso
        ///     {
        ///        "id": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "nome": "Novo recurso - alterado",
        ///        "login": "usuario.teste",
        ///        "senha": "12345678",
        ///        "confirmarSenha": "12345678",
        ///        "dataInclusao": "2019-09-21T19:15:23.519Z"
        ///     }
        /// </remarks>
        /// <param name="id">Id do recurso</param>        
        /// <param name="obj">Recurso</param>        
        /// <response code="204">Recurso alterado com sucesso</response>
        /// <response code="400">ID informado não é válido</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public IActionResult Put(Guid id, [FromBody]RecursoViewModel obj)
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
                _recursoAppService.Alterar(obj);
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
        /// Remover recurso
        /// </summary>
        /// <remarks>
        /// # Remover recurso
        /// 
        /// Remove um recurso da base de dados.
        /// </remarks>
        /// <param name="id">Id do recurso</param>        
        /// <response code="204">Recurso removido com sucesso</response>
        /// <response code="404">Recurso não encontrado</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            RecursoViewModel obj = _recursoAppService.Consultar(id);

            if (obj == null)
            {
                return NotFound();
            }

            _recursoAppService.Remover(id);

            return NoContent();
        }

        private bool ObjExists(Guid id)
        {
            return _recursoAppService.Consultar(id) != null;
        }
    }
}
