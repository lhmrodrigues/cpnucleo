﻿using Cpnucleo.Application.Interfaces;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Cpnucleo.API.Controllers.V1
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1", Deprecated = true)]
    [Authorize]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaAppService _tarefaAppService;

        public TarefaController(ITarefaAppService tarefaAppService)
        {
            _tarefaAppService = tarefaAppService;
        }

        /// <summary>
        /// Listar tarefas
        /// </summary>
        /// <remarks>
        /// # Listar tarefas
        /// 
        /// Lista tarefas da base de dados.
        /// </remarks>
        /// <response code="200">Retorna uma lista de tarefas</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public IEnumerable<TarefaViewModel> Get()
        {
            return _tarefaAppService.Listar();
        }

        /// <summary>
        /// Consultar tarefa
        /// </summary>
        /// <remarks>
        /// # Consultar tarefa
        /// 
        /// Consulta uma tarefa na base de dados.
        /// </remarks>
        /// <param name="id">Id do tarefa</param>        
        /// <response code="200">Retorna uma tarefa</response>
        /// <response code="404">Tarefa não encontrada</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<TarefaViewModel> Get(Guid id)
        {
            TarefaViewModel tarefa = _tarefaAppService.Consultar(id);

            if (tarefa == null)
            {
                return NotFound();
            }

            return Ok(tarefa);
        }

        /// <summary>
        /// Incluir tarefa
        /// </summary>
        /// <remarks>
        /// # Incluir tarefa
        /// 
        /// Inclui uma tarefa na base de dados.
        /// 
        /// # Sample request:
        ///
        ///     POST /tarefa
        ///     {
        ///        "nome": "Nova tarefa",
        ///        "dataInicio": "2019-09-21T15:24:35.117Z",
        ///        "dataTermino": "2019-09-21T15:24:35.117Z",
        ///        "qtdHoras": 8,
        ///        "idProjeto": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "idWorkflow": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "idRecurso": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "idTipoTarefa": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91"
        ///     }
        /// </remarks>
        /// <param name="obj">Tarefa</param>        
        /// <response code="201">Tarefa cadastrada com sucesso</response>
        /// <response code="400">Objetos não preenchidos corretamente</response>
        /// <response code="409">Guid informado já consta na base de dados</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public ActionResult<TarefaViewModel> Post([FromBody]TarefaViewModel obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _tarefaAppService.Incluir(obj);
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

            return CreatedAtAction(nameof(Get), new { id = obj.Id }, obj);
        }

        /// <summary>
        /// Alterar tarefa
        /// </summary>
        /// <remarks>
        /// # Alterar tarefa
        /// 
        /// Altera uma tarefa na base de dados.
        /// 
        /// # Sample request:
        ///
        ///     PUT /tarefa
        ///     {
        ///        "id": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "nome": "Nova tarefa - alterada",
        ///        "dataInicio": "2019-09-21T15:24:35.117Z",
        ///        "dataTermino": "2019-09-21T15:24:35.117Z",
        ///        "qtdHoras": 8,
        ///        "idProjeto": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "idWorkflow": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "idRecurso": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "idTipoTarefa": "fffc0a28-b9e9-4ffd-0053-08d73d64fb91",
        ///        "dataInclusao": "2019-09-21T19:15:23.519Z"
        ///     }
        /// </remarks>
        /// <param name="id">Id da tarefa</param>        
        /// <param name="obj">Tarefa</param>        
        /// <response code="204">Tarefa alterada com sucesso</response>
        /// <response code="400">ID informado não é válido</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult Put(Guid id, [FromBody]TarefaViewModel obj)
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
                _tarefaAppService.Alterar(obj);
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
        /// Remover tarefa
        /// </summary>
        /// <remarks>
        /// # Remover tarefa
        /// 
        /// Remove uma tarefa da base de dados.
        /// </remarks>
        /// <param name="id">Id da tarefa</param>        
        /// <response code="204">Tarefa removida com sucesso</response>
        /// <response code="404">Tarefa não encontrada</response>
        /// <response code="401">Acesso não autorizado</response>
        /// <response code="500">Erro no processamento da requisição</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(Guid id)
        {
            TarefaViewModel obj = _tarefaAppService.Consultar(id);

            if (obj == null)
            {
                return NotFound();
            }

            _tarefaAppService.Remover(id);

            return NoContent();
        }

        private bool ObjExists(Guid id)
        {
            return _tarefaAppService.Consultar(id) != null;
        }
    }
}
