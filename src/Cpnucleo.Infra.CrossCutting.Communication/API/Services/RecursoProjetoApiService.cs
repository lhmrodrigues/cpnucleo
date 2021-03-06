﻿using Cpnucleo.Infra.CrossCutting.Communication.API.Interfaces;
using Cpnucleo.Infra.CrossCutting.Util.Interfaces;
using Cpnucleo.Infra.CrossCutting.Util.ViewModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Cpnucleo.Infra.CrossCutting.Communication.API.Services
{
    internal class RecursoProjetoApiService : BaseApiService<RecursoProjetoViewModel>, IRecursoProjetoApiService
    {
        private const string actionRoute = "recursoProjeto";

        public RecursoProjetoApiService(ISystemConfiguration systemConfiguration)
            : base(systemConfiguration)
        {
        }

        public async Task<bool> IncluirAsync(string token, RecursoProjetoViewModel obj)
        {
            return await PostAsync(token, actionRoute, obj);
        }

        public async Task<IEnumerable<RecursoProjetoViewModel>> ListarAsync(string token, bool getDependencies = false)
        {
            return await GetAsync(token, actionRoute, getDependencies);
        }

        public async Task<RecursoProjetoViewModel> ConsultarAsync(string token, Guid id)
        {
            return await GetAsync(token, actionRoute, id);
        }

        public async Task<bool> RemoverAsync(string token, Guid id)
        {
            return await DeleteAsync(token, actionRoute, id);
        }

        public async Task<bool> AlterarAsync(string token, RecursoProjetoViewModel obj)
        {
            return await PutAsync(token, actionRoute, obj.Id, obj);
        }

        public async Task<IEnumerable<RecursoProjetoViewModel>> ListarPorProjetoAsync(string token, Guid idProjeto)
        {
            try
            {
                RestRequest request = new RestRequest($"api/v2/{actionRoute}/getbyprojeto/{idProjeto}", Method.GET);
                request.AddHeader("Authorization", $"Bearer {token}");

                IRestResponse response = await _client.ExecuteAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (!string.IsNullOrWhiteSpace(response.Content))
                    {
                        throw new Exception(response.Content);
                    }
                    else
                    {
                        throw new Exception("Falha ao se comunicar com a api de dados.");
                    }
                }

                return JsonConvert.DeserializeObject<IEnumerable<RecursoProjetoViewModel>>(response.Content);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
