using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotDns.Middlewares;
using DotDns.Records;
using DotDns.Records.EDns;
using Microsoft.Extensions.DependencyInjection;

namespace DotDns
{
    public class DnsProcessor : IDnsProcessor
    {
        private readonly IList<Type> _middlewareTypes;
        private readonly IServiceProvider _serviceProvider;
        private IDnsMiddleware.MiddlewareDelegate _application;
        private bool _built;

        public DnsProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _middlewareTypes = new List<Type>();
        }

        public void AddMiddleware<T>() where T : IDnsMiddleware
        {
            if (_built) throw new InvalidOperationException("DnsProcessor has been built");

            _middlewareTypes.Add(typeof(T));
        }

        public void Build()
        {
            if (_built) throw new InvalidOperationException("DnsProcessor has already been built");

            _built = true;

            IDnsMiddleware.MiddlewareDelegate nextDelegate = DefaultHandlerAsync;

            foreach (var type in _middlewareTypes.Reverse())
            {
                var middleware =
                    (IDnsMiddleware) ActivatorUtilities.CreateInstance(_serviceProvider, type, nextDelegate);
                nextDelegate = middleware.ProcessAsync;
            }

            _application = nextDelegate;
        }

        public async ValueTask Process(DnsPacket request, DnsPacket response)
        {
            if (!_built) throw new InvalidOperationException("DnsProcessor has not been built");

            // Configure basic response
            response.Id = request.Id;
            response.OpCode = request.OpCode;
            response.IsResponse = true;
            response.ResponseCode = DnsResponseCode.NoError;

            if (request.OpCode == DnsOpcode.StandardQuery)
            {
                response.RecursionDesired = request.RecursionDesired;
                response.CheckingDisabled = request.CheckingDisabled;
            }

            // Ensure there is only a single question and zero flag isn't set
            if (request.Questions.Count != 1 || request.Zero)
            {
                response.ResponseCode = DnsResponseCode.Refused;
                return;
            }

            var question = request.Questions[0];
            response.AddQuestion(question);

            // Ensure question is an internet request
            if (question.Class != DnsClass.Internet)
            {
                response.ResponseCode = DnsResponseCode.Refused;
                return;
            }

            // Configure EDns
            var reqOpt = request.OptRecord;
            OptRecord? resOpt = null;
            if (reqOpt != null)
            {
                response.AddAdditional(resOpt = new OptRecord(0));

                if (reqOpt.Version != 0)
                {
                    response.ResponseCode = DnsResponseCode.BadVersion;
                    return;
                }
            }

            try
            {
                await _application(request, response);
            }
            catch
            {
                response.Reset();
                response.Id = request.Id;
                response.OpCode = request.OpCode;
                response.IsResponse = true;
                response.ResponseCode = DnsResponseCode.ServerFailure;

                // TODO: Remove
                throw;
            }
        }

        private ValueTask DefaultHandlerAsync(DnsPacket request, DnsPacket response)
        {
            return default;
        }
    }
}