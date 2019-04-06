using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using MvcCookieAuthSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCookieAuthSample.Services
{
    public class ConsentService
    {
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        public ConsentService(
            IClientStore clientStore,
            IResourceStore resourceStore,
            IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }
       
        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resource,InputConsentViewModel inputVm)
        {
            var remember = inputVm?.RememberConsent ?? true;
            var selectedScopes = inputVm?.ScopesConsented ?? Enumerable.Empty<string>();
            var vm = new ConsentViewModel();
            vm.ClientName = client.ClientName;
            vm.ClientLogoUrl = client.LogoUri;
            vm.ClientUrl = client.ClientUri;
            vm.RememberConsent = remember;
            vm.IdentityScopes = resource.IdentityResources.Select(i => CreateScopeViewModel(i,selectedScopes.Contains(i.Name)||inputVm!=null));
            vm.ResourceScopes = resource.ApiResources.SelectMany(i => i.Scopes).Select(x => CreateScopeViewModel(x, selectedScopes.Contains(x.Name)|| inputVm != null));
            return vm;
        }
        private ScopeViewModel CreateScopeViewModel(IdentityResource identity,bool check)
        {
            return new ScopeViewModel
            {
                Name = identity.Name,
                DisplayName = identity.DisplayName,
                Required = identity.Required,
                Checked = check||identity.Required,
                Emphasize = identity.Emphasize,
            };
        }
        private ScopeViewModel CreateScopeViewModel(Scope scope,bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Required = scope.Required,
                Checked = check||scope.Required,
                Emphasize = scope.Emphasize,
            };
        }
        public async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl,InputConsentViewModel inputvm)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request == null)
                return null;

            var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
            var vm = CreateConsentViewModel(request, client, resources,inputvm);
            vm.ReturnUrl = returnUrl;
            return vm;

        }
        public async Task<ProcessConsentResult> ProcessConsent(InputConsentViewModel vm)
        {
            ConsentResponse consentResp = null;
            var result =new ProcessConsentResult();
            if (vm.Button == "no")
            {
                consentResp = ConsentResponse.Denied;
            }
            else if (vm.Button == "yes")
            {
                if (vm.ScopesConsented != null &&
                    vm.ScopesConsented.Any())
                {

                    consentResp = new ConsentResponse
                    {
                        RememberConsent = vm.RememberConsent,
                        ScopesConsented = vm.ScopesConsented
                    };
                }
                result.ValidationError = "请至少选中一个权限";
            }
            if (consentResp != null)
            {
                var req = await _identityServerInteractionService.GetAuthorizationContextAsync(vm.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(req, consentResp);
                result.RedirectUrl = vm.ReturnUrl;
            }
            {
                var consentvm =await BuildConsentViewModel(vm.ReturnUrl,vm);
                result.ConsentViewModel = consentvm;
            }
            return result;
        }
    }
}
