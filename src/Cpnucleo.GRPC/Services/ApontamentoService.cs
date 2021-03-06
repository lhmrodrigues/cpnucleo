﻿using AutoMapper;
using Cpnucleo.Application.Interfaces;
using Cpnucleo.Infra.CrossCutting.Communication.GRPC.Protos;
using Cpnucleo.Infra.CrossCutting.Communication.GRPC.Protos.Apontamento;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cpnucleo.GRPC
{
    public class ApontamentoService : Apontamento.ApontamentoBase
    {
        private readonly IMapper _mapper;
        private readonly IApontamentoAppService _apontamentoAppService;

        public ApontamentoService(IMapper mapper, IApontamentoAppService apontamentoAppService)
        {
            _mapper = mapper;
            _apontamentoAppService = apontamentoAppService;
        }

        public override async Task<BaseReply> Incluir(ApontamentoModel request, ServerCallContext context)
        {
            return await Task.FromResult(new BaseReply
            {
                Id = _apontamentoAppService.Incluir(_mapper.Map<ApontamentoViewModel>(request)).ToString()
            });
        }

        public override async Task<ListarReply> Listar(Empty request, ServerCallContext context)
        {
            ListarReply result = new ListarReply();
            result.Lista.AddRange(_mapper.Map<IEnumerable<ApontamentoModel>>(_apontamentoAppService.Listar()));

            return await Task.FromResult(result);
        }

        public override async Task<ApontamentoModel> Consultar(BaseRequest request, ServerCallContext context)
        {
            Guid id = new Guid(request.Id);
            ApontamentoModel result = _mapper.Map<ApontamentoModel>(_apontamentoAppService.Consultar(id));

            return await Task.FromResult(result);
        }

        public override async Task<BaseReply> Alterar(ApontamentoModel request, ServerCallContext context)
        {
            return await Task.FromResult(new BaseReply
            {
                Sucesso = _apontamentoAppService.Alterar(_mapper.Map<ApontamentoViewModel>(request))
            });
        }

        public override async Task<BaseReply> Remover(BaseRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new BaseReply
            {
                Sucesso = _apontamentoAppService.Remover(new Guid(request.Id))
            });
        }

        public override async Task<ListarPorRecursoReply> ListarPorRecurso(BaseRequest request, ServerCallContext context)
        {
            ListarPorRecursoReply result = new ListarPorRecursoReply();
            result.Lista.AddRange(_mapper.Map<IEnumerable<ApontamentoModel>>(_apontamentoAppService.ListarPorRecurso(new Guid(request.Id))));

            return await Task.FromResult(result);
        }
    }
}
