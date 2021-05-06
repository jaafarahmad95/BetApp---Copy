using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace testtest.Installer
{
    public interface IInstaller
    {
        void InstallServicesAssembly(IServiceCollection services, IConfiguration configuration);
    }
}
